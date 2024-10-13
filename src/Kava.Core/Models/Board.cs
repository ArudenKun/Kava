using JetBrains.Annotations;
using Kava.Core.Models.Abstractions;

namespace Kava.Core.Models;

public class Board : BaseEntity
{
    public required string Name { get; set; }

    [PublicAPI]
    public ICollection<Category> Categories { get; } = [];
}
