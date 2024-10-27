using System;
using Kava.Models.Abstractions;

namespace Kava.Models;

public class Comment : BaseEntity
{
    public string Value { get; set; } = string.Empty;

    public required Guid EntryId { get; set; }

    public Entry? Entry { get; init; }
}
