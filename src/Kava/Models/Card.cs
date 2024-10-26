using System;
using System.Collections.Generic;
using FreeSql.DataAnnotations;
using Kava.Models.Abstractions;

namespace Kava.Models;

public class Card : BaseEntity
{
    [Column(StringLength = 64)]
    public required string Name { get; set; }

    [Column(StringLength = -1)]
    public string Description { get; set; } = string.Empty;

    [Navigate(nameof(Entry.CardId))]
    public List<Entry> Cards { get; set; } = [];

    public required Guid BoardId { get; set; }

    [Navigate(nameof(BoardId))]
    public Board? Board { get; init; }
}
