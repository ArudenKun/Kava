using Kava.Models.Abstractions;

namespace Kava.Models;

public class Comment : BaseEntity
{
    public Comment(string content)
    {
        Content = content;
    }

    public string Content { get; set; }

    public Ulid CardId { get; set; }
    public Card Card { get; set; } = null!;
}
