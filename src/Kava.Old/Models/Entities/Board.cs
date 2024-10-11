using System.Collections.Generic;
using JetBrains.Annotations;
using Kava.Models.Entities.Abstractions;

namespace Kava.Models.Entities;

public class Board : BaseEntity
{
    public required string Name { get; set; }

    [PublicAPI]
    public ICollection<Category> Categories { get; } = [];
}
