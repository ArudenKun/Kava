using Kava.Core.Models.Entities.Abstractions;

namespace Kava.Core.Models.Entities;

public class Attachment : BaseEntity
{
    public required Ulid CardId { get; init; }
    public required string Name { get; set; }
    public required ulong Size { get; set; }
    public required string MimeType { get; set; }
    public Card Card { get; init; } = null!;
}
