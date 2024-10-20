using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Kava.Data.Abstractions;

namespace Kava.Data.Json;

public class JsonTable<TEntity>
    where TEntity : class, IEntity, new()
{
    private readonly SemaphoreSlim _semaphoreSlim = new(1);
    private readonly string _folderPath;
    private readonly JsonTypeInfo _jsonTypeInfo;

    public JsonTable(string folderPath, JsonSerializerOptions jsonSerializerOptions)
    {
        _folderPath = folderPath;
        _jsonTypeInfo = jsonSerializerOptions.GetTypeInfo(typeof(TEntity));
    }

    public JsonTable(string folderPath, IJsonTypeInfoResolver jsonTypeInfoResolver)
        : this(
            folderPath,
            new JsonSerializerOptions
            {
                WriteIndented = true,
                TypeInfoResolver = jsonTypeInfoResolver,
            }
        ) { }

    private string GetFilePath(Ulid id) => Path.Combine(_folderPath, $"{id}.json");

    public async Task<TEntity?> GetAsync(Ulid id)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            var filePath = GetFilePath(id);
            if (!File.Exists(filePath))
                return null;
            await using var stream = File.OpenRead(filePath);
            using var document = await JsonDocument.ParseAsync(
                stream,
                new JsonDocumentOptions
                {
                    AllowTrailingCommas = true,
                    CommentHandling = JsonCommentHandling.Skip,
                }
            );

            var entity = new TEntity();

            foreach (var jsonProperty in document.RootElement.EnumerateObject())
            {
                var property = _jsonTypeInfo.Properties.FirstOrDefault(p =>
                    string.Equals(p.Name, jsonProperty.Name, StringComparison.Ordinal)
                );

                if (property is null)
                    continue;

                // HACK: Use custom converter specified on the property.
                // This will also apply the converter to any other nested properties of the same type,
                // but unfortunately there's no way to avoid that for now.
                var propertyOptions = new JsonSerializerOptions(property.Options);
                if (property.CustomConverter is not null)
                    propertyOptions.Converters.Add(property.CustomConverter);

                property.Set?.Invoke(
                    entity,
                    jsonProperty.Value.Deserialize(
                        propertyOptions.GetTypeInfo(property.PropertyType)
                    )
                );
            }

            return entity;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async Task SaveAsync(Ulid id, TEntity entity)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            var filePath = GetFilePath(id);
            var data = JsonSerializer.SerializeToUtf8Bytes(entity, _jsonTypeInfo);
            await File.WriteAllBytesAsync(filePath, data);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async IAsyncEnumerable<TEntity> GetAllAsync()
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            foreach (var filePath in Directory.EnumerateFiles(_folderPath))
            {
                await using var stream = File.OpenRead(filePath);
                using var document = await JsonDocument.ParseAsync(
                    stream,
                    new JsonDocumentOptions
                    {
                        AllowTrailingCommas = true,
                        CommentHandling = JsonCommentHandling.Skip,
                    }
                );

                var entity = new TEntity();

                foreach (var jsonProperty in document.RootElement.EnumerateObject())
                {
                    var property = _jsonTypeInfo.Properties.FirstOrDefault(p =>
                        string.Equals(p.Name, jsonProperty.Name, StringComparison.Ordinal)
                    );

                    if (property is null)
                        continue;

                    // HACK: Use custom converter specified on the property.
                    // This will also apply the converter to any other nested properties of the same type,
                    // but unfortunately there's no way to avoid that for now.
                    var propertyOptions = new JsonSerializerOptions(property.Options);
                    if (property.CustomConverter is not null)
                        propertyOptions.Converters.Add(property.CustomConverter);

                    property.Set?.Invoke(
                        entity,
                        jsonProperty.Value.Deserialize(
                            propertyOptions.GetTypeInfo(property.PropertyType)
                        )
                    );
                }

                yield return entity;
            }
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public IEnumerable<TEntity> GetAll()
    {
        _semaphoreSlim.Wait();
        try
        {
            foreach (var filePath in Directory.EnumerateFiles(_folderPath))
            {
                using var stream = File.OpenRead(filePath);
                using var document = JsonDocument.Parse(
                    stream,
                    new JsonDocumentOptions
                    {
                        AllowTrailingCommas = true,
                        CommentHandling = JsonCommentHandling.Skip,
                    }
                );

                var entity = new TEntity();

                foreach (var jsonProperty in document.RootElement.EnumerateObject())
                {
                    var property = _jsonTypeInfo.Properties.FirstOrDefault(p =>
                        string.Equals(p.Name, jsonProperty.Name, StringComparison.Ordinal)
                    );

                    if (property is null)
                        continue;

                    // HACK: Use custom converter specified on the property.
                    // This will also apply the converter to any other nested properties of the same type,
                    // but unfortunately there's no way to avoid that for now.
                    var propertyOptions = new JsonSerializerOptions(property.Options);
                    if (property.CustomConverter is not null)
                        propertyOptions.Converters.Add(property.CustomConverter);

                    property.Set?.Invoke(
                        entity,
                        jsonProperty.Value.Deserialize(
                            propertyOptions.GetTypeInfo(property.PropertyType)
                        )
                    );
                }

                yield return entity;
            }
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async Task DeleteAsync(Ulid id)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            File.Delete(GetFilePath(id));
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
}
