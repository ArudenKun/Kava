using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using ZiggyCreatures.Caching.Fusion.Serialization;

namespace Desktop.Services.Caching;

public class LiteDbCacheSerializer : IFusionCacheSerializer
{
    private static readonly BsonMapper Mapper = BsonMapper.Global;

    public byte[] Serialize<T>(T? obj)
    {
        var doc = Mapper.ToDocument(obj);
        return BsonSerializer.Serialize(doc);
    }

    public T? Deserialize<T>(byte[] data)
    {
        var doc = BsonSerializer.Deserialize(data);
        return Mapper.ToObject<T>(doc);
    }

    public ValueTask<byte[]> SerializeAsync<T>(T? obj, CancellationToken token = new()) =>
        new(Serialize(obj));

    public ValueTask<T?> DeserializeAsync<T>(byte[] data, CancellationToken token = new()) =>
        new(Deserialize<T>(data));
}
