using System;
using FreeSql.DataAnnotations;
using Kava.Data.Abstractions;

namespace Kava.Models.Abstractions;

public abstract class BaseEntity : IEntity
{
    /// <inheritdoc />
    [Column(IsPrimary = true)]
    public Guid Id { get; set; }

    /// <inheritdoc />
    [Column(ServerTime = DateTimeKind.Local, CanUpdate = false)]
    public DateTime CreatedAt { get; set; }

    /// <inheritdoc />
    [Column(ServerTime = DateTimeKind.Local)]
    public DateTime UpdatedAt { get; set; }
}
