using System;
using Kava.Data.Abstractions;

namespace Kava.Models.Abstractions;

public abstract class BaseEntity : IEntity
{
    /// <inheritdoc />
    public Guid Id { get; set; }

    /// <inheritdoc />
    public DateTime CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTime UpdatedAt { get; set; }
}
