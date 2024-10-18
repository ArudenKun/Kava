using JetBrains.Annotations;
using Kava.Models.Abstractions;

namespace Kava.Models;

public class Card : BaseEntity
{
    public Card(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

    [PublicAPI]
    public ICollection<Attachment> Attachments { get; } = [];

    [PublicAPI]
    public ICollection<Comment> Comments { get; } = [];

    public Ulid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
