using System.Collections.Generic;
using JetBrains.Annotations;
using Kava.Models.Abstractions;

namespace Kava.Models;

public class Board : BaseEntity
{
    public required string Name { get; set; }

    [PublicAPI]
    public ICollection<Category> Categories { get; } = [];
}
