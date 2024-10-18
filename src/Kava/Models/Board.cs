using JetBrains.Annotations;
using Kava.Models.Abstractions;

namespace Kava.Models;

public class Board : BaseEntity
{
    public Board(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

    [UsedImplicitly]
    public ICollection<Category> Categories { get; } = [];
}
