using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Kava.Models.Abstractions;

namespace Kava.Models;

public class Card : BaseEntity
{
    public required Ulid CategoryId { get; init; }
    public Category Category { get; init; } = null!;
    public required string Name { get; set; }

    [PublicAPI]
    public ICollection<Attachment> Attachments { get; } = [];
}
