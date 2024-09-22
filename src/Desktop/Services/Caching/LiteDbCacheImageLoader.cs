using System;
using System.IO;
using System.Threading.Tasks;
using AsyncImageLoader;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Core.Helpers;
using Flurl.Http;
using LiteDB;
using Microsoft.Extensions.Logging;

namespace Desktop.Services.Caching;

public sealed class LiteDbCacheImageLoader : IAsyncImageLoader
{
    private readonly ILiteStorage<string> _fs;
    private readonly ILogger<LiteDbCacheImageLoader> _logger;

    public LiteDbCacheImageLoader(ILiteDatabase db, ILogger<LiteDbCacheImageLoader> logger)
    {
        _fs = db.FileStorage;
        _logger = logger;
    }

    public async Task<Bitmap?> ProvideImageAsync(string url)
    {
        var internalOrLocalBitmap =
            await LoadFromInternalAsync(url).ConfigureAwait(false)
            ?? await LoadFromLocalAsync(url).ConfigureAwait(false);

        if (internalOrLocalBitmap is not null)
            return internalOrLocalBitmap;

        var hashId = MD5HashHelper.ComputeHash(url);

        try
        {
            Bitmap bitmap;
            if (!_fs.Exists(hashId))
            {
                var externalBytes = await LoadDataFromExternalAsync(url).ConfigureAwait(false);
                if (externalBytes is null)
                    return null;

                using var memoryStream = new MemoryStream(externalBytes);
                _fs.Upload(hashId, $"image-cache-{hashId}", memoryStream);
                bitmap = new Bitmap(memoryStream);
                return bitmap;
            }

            await using var liteFileStream = _fs.OpenRead(hashId);
            bitmap = new Bitmap(liteFileStream);
            return bitmap;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public void Dispose() { }

    /// <summary>
    ///     the url maybe is local file url,so if file exists ,we got a Bitmap
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private static Task<Bitmap?> LoadFromLocalAsync(string url) =>
        Task.FromResult(File.Exists(url) ? new Bitmap(url) : null);

    /// <summary>
    ///     Receives image bytes from an internal source (for example, from the disk).
    ///     This data will be NOT cached globally (because it is assumed that it is already in internal source us and does not
    ///     require global caching)
    /// </summary>
    /// <param name="url">Target url</param>
    /// <returns>Bitmap</returns>
    private Task<Bitmap?> LoadFromInternalAsync(string url)
    {
        try
        {
            var uri = url.StartsWith('/')
                ? new Uri(url, UriKind.Relative)
                : new Uri(url, UriKind.RelativeOrAbsolute);

            if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                return Task.FromResult<Bitmap?>(null);

            if (uri is { IsAbsoluteUri: true, IsFile: true })
                return Task.FromResult(new Bitmap(uri.LocalPath))!;

            return Task.FromResult(new Bitmap(AssetLoader.Open(uri)))!;
        }
        catch (Exception e)
        {
            _logger.LogError(
                "Failed to resolve image from request with uri: {RequestUri}\nException: {Exception}",
                url,
                e
            );
            return Task.FromResult<Bitmap?>(null);
        }
    }

    /// <summary>
    ///     Receives image bytes from an external source (for example, from the Internet).
    ///     This data will be cached globally (if required by the current implementation)
    /// </summary>
    /// <param name="url">Target url</param>
    /// <returns>Image bytes</returns>
    private static async Task<byte[]?> LoadDataFromExternalAsync(string url)
    {
        try
        {
            return await url.GetBytesAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
