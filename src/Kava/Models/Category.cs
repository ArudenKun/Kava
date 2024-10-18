using JetBrains.Annotations;
using Kava.Models.Abstractions;

namespace Kava.Models;

public class Category : BaseEntity
{
    public Category(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; set; }
    public string? Description { get; set; }

    [PublicAPI]
    public ICollection<Card> Cards { get; } = [];

    public Ulid BoardId { get; init; }
    public Board Board { get; init; } = null!;
}
