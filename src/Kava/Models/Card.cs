using System;
using Kava.Data.Abstractions;

namespace Kava.Models;

public class Card : BaseEntity
{
    public Card() { }

    public Card(string name)
    {
        Name = name;
    }

    public string Name { get; set; } = string.Empty;

    public Ulid CategoryId { get; set; }
}
