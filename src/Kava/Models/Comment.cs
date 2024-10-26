using System;
using FreeSql.DataAnnotations;
using Kava.Models.Abstractions;

namespace Kava.Models;

public class Comment : BaseEntity
{
    [Column(StringLength = -1)]
    public string Value { get; set; } = string.Empty;

    public required Guid EntryId { get; set; }

    [Navigate(nameof(EntryId))]
    public Entry? Entry { get; init; }
}
