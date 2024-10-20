using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Kava.Data.Abstractions;
using Kava.Utilities.Extensions;

namespace Kava.Data.Json;

public class JsonRepository : IRepository
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly DirectoryInfo _directoryInfo;

    private readonly ConcurrentDictionary<Type, object> _tableCache = [];

    public JsonRepository(string jsonFolderPath, JsonSerializerOptions jsonSerializerOptions)
    {
        _directoryInfo = new DirectoryInfo(jsonFolderPath);
        _jsonSerializerOptions = jsonSerializerOptions;
        _directoryInfo.Create();
    }

    public async IAsyncEnumerable<TEntity> GetAllAsync<TEntity>()
        where TEntity : class, IEntity, new()
    {
        var table = GetTable<TEntity>();
        await foreach (var entity in table.GetAllAsync())
            yield return entity;
    }

    public IEnumerable<TEntity> GetAll<TEntity>()
        where TEntity : class, IEntity, new()
    {
        var table = GetTable<TEntity>();
        return table.GetAll();
    }

    public Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class, IEntity, new()
    {
        var table = GetTable<TEntity>();
        var now = DateTime.UtcNow;
        entity.CreatedAt = now;
        entity.UpdatedAt = now;
        return table.SaveAsync(entity.Id, entity);
    }

    public Task UpdateAsync<TEntity>(TEntity entity)
        where TEntity : class, IEntity, new()
    {
        var table = GetTable<TEntity>();
        entity.UpdatedAt = DateTime.UtcNow;
        return table.SaveAsync(entity.Id, entity);
    }

    public Task DeleteAsync<TEntity>(Ulid id)
        where TEntity : class, IEntity, new()
    {
        var table = GetTable<TEntity>();
        return table.DeleteAsync(id);
    }

    public Task<TEntity?> GetByIdAsync<TEntity>(Ulid id)
        where TEntity : class, IEntity, new()
    {
        var table = GetTable<TEntity>();
        return table.GetAsync(id);
    }

    private JsonTable<TEntity> GetTable<TEntity>()
        where TEntity : class, IEntity, new() =>
        (JsonTable<TEntity>)
            _tableCache.GetOrAdd(
                typeof(TEntity),
                _ => new JsonTable<TEntity>(
                    EnsureDirectoryExists<TEntity>(),
                    _jsonSerializerOptions
                )
            );

    private string EnsureDirectoryExists<T>()
    {
        var directoryPath = Path.Combine(_directoryInfo.FullName, typeof(T).GetDisplayName());
        var directoryInfo = new DirectoryInfo(directoryPath);
        directoryInfo.Create();
        return directoryPath;
    }
}
