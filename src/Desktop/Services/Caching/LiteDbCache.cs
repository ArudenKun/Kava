using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZLogger;

namespace Desktop.Services.Caching;

public interface ILiteDbCache : IDistributedCache, IDisposable
{
    int CacheItemCount();
    Task<int> CacheCountAsync(CancellationToken cancellationToken = default);

    int RemoveExpired();

    Task<int> RemoveExpiredAsync(CancellationToken cancellationToken = default);

    void Clear();
    Task ClearAsync(CancellationToken cancellationToke = default);

    ulong CacheSizeInBytes();
    Task<ulong> CacheSizeInBytesAsync();
}

public sealed class LiteDbCache : ILiteDbCache
{
    private readonly ILiteDatabase _db;
    private readonly ILiteCollection<LiteDbCacheEntry> _collection;
    private readonly ILogger<LiteDbCache> _logger;

    private readonly Timer _cleanupTimer;
    private readonly Timer _saveTimer;

    public LiteDbCache(
        ILiteDatabase db,
        IOptions<LiteDbCacheOptions> options,
        ILogger<LiteDbCache> logger
    )
    {
        _db = db;
        _collection = db.GetCollection<LiteDbCacheEntry>(options.Value.CollectionName);
        _logger = logger;

        _cleanupTimer = new Timer(
            static state =>
            {
                var @this = (LiteDbCache)state!;

                @this._logger.ZLogInformation(
                    $"Beginning background cleanup of expired SQLiteCache items"
                );
                @this.RemoveExpired();
            },
            this,
            TimeSpan.Zero,
            TimeSpan.FromMinutes(1)
        );

        _saveTimer = new Timer(
            static state =>
            {
                var @this = (LiteDbCache)state!;
                @this._logger.ZLogInformation($"Beginning background save of cache to disk");
                @this._db.Checkpoint();
                @this._logger.ZLogInformation($"Successful background save of cache to disk");
            },
            this,
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(30)
        );
    }

    public byte[]? Get(string key)
    {
        var now = DateTime.UtcNow;

        var entry = _collection.Query().Where(e => e.Key == key).SingleOrDefault();

        if (entry is null)
            return null;

        if (IsExpired(entry, now))
        {
            Remove(key);
            return null;
        }

        RefreshInternal(key, now, entry);

        _logger.ZLogInformation($"Successfully retrieved cache entry with key {key}");

        return entry.Value;
    }

    public Task<byte[]?> GetAsync(string key, CancellationToken token = new()) =>
        Task.FromResult(Get(key));

    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        var now = DateTimeOffset.Now;
        var oldItem = _collection.Query().Where(c => c.Key == key).SingleOrDefault();

        if (oldItem is not null)
            Remove(key);

        DateTimeOffset? expiry = null;
        TimeSpan? renewal = null;

        if (options.AbsoluteExpiration.HasValue)
        {
            expiry = options.AbsoluteExpiration.Value.ToUniversalTime();
        }
        else if (options.AbsoluteExpirationRelativeToNow.HasValue)
        {
            expiry = now.Add(options.AbsoluteExpirationRelativeToNow.Value);
        }

        if (options.SlidingExpiration.HasValue)
        {
            renewal = options.SlidingExpiration.Value;
            expiry = (expiry ?? now) + renewal;
        }

        _collection.Insert(new LiteDbCacheEntry(key, value, expiry, renewal));
        _logger.ZLogInformation($"Inserted new cache entry with key {key}");
    }

    public Task SetAsync(
        string key,
        byte[] value,
        DistributedCacheEntryOptions options,
        CancellationToken token = new()
    )
    {
        Set(key, value, options);
        return Task.CompletedTask;
    }

    public void Refresh(string key)
    {
        var now = DateTime.UtcNow;

        var entry = _collection.Query().Where(e => e.Key == key).SingleOrDefault();

        RefreshInternal(key, now, entry);
    }

    public Task RefreshAsync(string key, CancellationToken token = new())
    {
        Refresh(key);

        return Task.CompletedTask;
    }

    public void Remove(string key)
    {
        _collection.DeleteMany(e => e.Key == key);
        _logger.ZLogInformation($"Removed cache entry with key {key}");
    }

    public Task RemoveAsync(string key, CancellationToken token = new())
    {
        Remove(key);
        return Task.CompletedTask;
    }

    public int RemoveExpired()
    {
        var removed = _collection.DeleteMany(entry =>
            entry.Expiry == null || !(entry.Expiry >= DateTimeOffset.Now)
        );

        _logger.ZLogInformation($"Removed {removed} expired entries from cache");

        return removed;
    }

    public Task<int> RemoveExpiredAsync(CancellationToken cancellationToke = default) =>
        Task.FromResult(RemoveExpired());

    public int CacheItemCount() => _collection.Query().Count();

    public Task<int> CacheCountAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(CacheItemCount());

    public void Clear() => _collection.DeleteAll();

    public Task ClearAsync(CancellationToken cancellationToke = default)
    {
        Clear();
        return Task.CompletedTask;
    }

    public ulong CacheSizeInBytes() =>
        _collection
            .Query()
            .ToEnumerable()
            .Aggregate(0UL, static (sum, next) => sum + (ulong)next.Value.Length);

    public Task<ulong> CacheSizeInBytesAsync() => Task.FromResult(CacheSizeInBytes());

    public void Dispose()
    {
        _cleanupTimer.Dispose();
        _saveTimer.Dispose();
        _db.Checkpoint();
    }

    private static bool IsExpired(LiteDbCacheEntry entry, DateTimeOffset now) =>
        entry.Expiry.HasValue && now >= entry.Expiry;

    private void RefreshInternal(string key, DateTimeOffset now, LiteDbCacheEntry entry)
    {
        if (!entry.Renewal.HasValue)
            return;

        if (IsExpired(entry, now))
        {
            Remove(key);
            return;
        }

        entry.Expiry = now + entry.Renewal;
        _collection.Update(entry);
        _logger.ZLogInformation($"Refreshing cache entry with key {key}");
    }
}
