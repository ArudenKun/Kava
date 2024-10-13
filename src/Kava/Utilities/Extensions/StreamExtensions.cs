using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Gress;

namespace Kava.Utilities.Extensions;

public static class StreamExtensions
{
    public const int DefaultBufferSize = 4096;

    public static void CopyTo(
        this Stream source,
        Stream destination,
        long totalLength = 0L,
        int bufferSize = DefaultBufferSize,
        IProgress<Percentage>? progress = null
    ) => source.CopyTo(destination, totalLength, bufferSize, progress?.ToDoubleBased());

    public static void CopyTo(
        this Stream source,
        Stream destination,
        long totalLength = 0L,
        int bufferSize = DefaultBufferSize,
        IProgress<double>? progress = null
    )
    {
        using var buffer = MemoryPool<byte>.Shared.Rent(bufferSize);

        var totalBytesRead = 0L;
        while (true)
        {
            var bytesRead = source.Read(buffer.Memory.Span);
            if (bytesRead <= 0)
                break;

            destination.Write(buffer.Memory.Span[..bytesRead]);

            totalBytesRead += bytesRead;
            progress?.Report(1.0 * totalBytesRead / totalLength);
        }
    }

    public static ValueTask CopyToAsync(
        this Stream source,
        Stream destination,
        long totalLength = 0L,
        int bufferSize = DefaultBufferSize,
        IProgress<Percentage>? progress = null,
        CancellationToken cancellationToken = default
    ) =>
        source.CopyToAsync(
            destination,
            totalLength,
            bufferSize,
            progress?.ToDoubleBased(),
            cancellationToken
        );

    public static async ValueTask CopyToAsync(
        this Stream source,
        Stream destination,
        long totalLength = 0L,
        int bufferSize = DefaultBufferSize,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default
    )
    {
        using var buffer = MemoryPool<byte>.Shared.Rent(bufferSize);

        var totalBytesRead = 0L;
        while (true)
        {
            var bytesRead = await source.ReadAsync(buffer.Memory, cancellationToken);
            if (bytesRead <= 0)
                break;

            await destination.WriteAsync(buffer.Memory[..bytesRead], cancellationToken);

            totalBytesRead += bytesRead;
            progress?.Report(1.0 * totalBytesRead / totalLength);
        }
    }
}
