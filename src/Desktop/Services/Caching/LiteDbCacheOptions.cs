namespace Desktop.Services.Caching;

public class LiteDbCacheOptions
{
    public LiteDbCacheOptions() { }

    public LiteDbCacheOptions(string collectionName)
    {
        CollectionName = collectionName;
    }

    public required string CollectionName { get; set; } = "Cache";
}
