using System;
using Kava.Data.Abstractions;

namespace Kava.Models;

public class Comment : BaseEntity
{
    public Comment() { }

    public Comment(string value)
    {
        Value = value;
    }

    public string Value { get; set; } = string.Empty;

    public Ulid CardId { get; set; }
}
