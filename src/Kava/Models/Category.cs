using System;
using Kava.Data.Abstractions;

namespace Kava.Models;

public class Category : BaseEntity
{
    public Category() { }

    public Category(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Ulid BoardId { get; set; }
}
