using JetBrains.Annotations;
using Kava.Core.Models.Entities.Abstractions;

namespace Kava.Core.Models.Entities;

public class Board : BaseEntity
{
    public required string Name { get; set; }

    [PublicAPI]
    public ICollection<Category> Categories { get; } = [];
}
