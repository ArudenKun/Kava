using JetBrains.Annotations;
using Kava.Core.Models.Abstractions;

namespace Kava.Core.Models;

public class Card : BaseEntity
{
    public required Ulid CategoryId { get; init; }
    public Category Category { get; init; } = null!;
    public required string Name { get; set; }

    [PublicAPI]
    public ICollection<Attachment> Attachments { get; } = [];

    [PublicAPI]
    public ICollection<Comment> Comments { get; } = [];
}
