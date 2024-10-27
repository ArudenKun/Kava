using System.Collections.Generic;
using Kava.Models.Abstractions;

namespace Kava.Models;

public class Board : BaseEntity
{
    public required string Name { get; set; }

    public List<Card> Categories { get; set; } = [];
}
