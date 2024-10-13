using JetBrains.Annotations;
using Kava.Core.Models.Abstractions;

namespace Kava.Core.Models;

public class Category : BaseEntity
{
    public required Ulid BoardId { get; init; }
    public Board Board { get; init; } = null!;
    public required string Name { get; set; }
    public string? Description { get; set; }

    [PublicAPI]
    public ICollection<Card> Cards { get; } = [];
}
