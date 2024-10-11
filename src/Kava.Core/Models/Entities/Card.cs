using JetBrains.Annotations;
using Kava.Core.Models.Entities.Abstractions;

namespace Kava.Core.Models.Entities;

public class Card : BaseEntity
{
    public required Ulid CategoryId { get; init; }
    public Category Category { get; init; } = null!;
    public required string Name { get; set; }

    [PublicAPI]
    public ICollection<Attachment> Attachments { get; } = [];
}
