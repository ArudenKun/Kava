using Kava.Data.Abstractions;

namespace Kava.Models;

public class Board : BaseEntity
{
    public Board() { }

    public Board(string name)
    {
        Name = name;
    }

    public string Name { get; set; } = string.Empty;
}
