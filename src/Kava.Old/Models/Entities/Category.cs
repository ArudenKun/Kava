using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Kava.Models.Entities.Abstractions;

namespace Kava.Models.Entities;

public class Category : BaseEntity
{
    public required Ulid BoardId { get; init; }
    public Board Board { get; init; } = null!;
    public required string Name { get; set; }
    public string? Description { get; set; }

    [PublicAPI]
    public ICollection<Card> Cards { get; } = [];
}
