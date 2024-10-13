using Kava.Core.Models.Abstractions;

namespace Kava.Core.Models;

public class Comment : BaseEntity
{
    public required string Content { get; set; }
    public required Ulid CardId { get; init; }
    public Card Card { get; init; } = null!;
}
