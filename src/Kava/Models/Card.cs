using System;
using System.Collections.Generic;
using Kava.Models.Abstractions;

namespace Kava.Models;

public class Card : BaseEntity
{
    public required string Name { get; set; }

    public string Description { get; set; } = string.Empty;

    public List<Entry> Cards { get; set; } = [];

    public required Guid BoardId { get; set; }

    public Board? Board { get; init; }
}
