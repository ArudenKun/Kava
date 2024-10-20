using System;
using JetBrains.Annotations;

namespace Kava.Data.Abstractions;

public abstract class BaseEntity : IEntity
{
    public Ulid Id { get; set; } = Ulid.NewUlid();

    [UsedImplicitly]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [UsedImplicitly]
    public DateTime UpdatedAt { get; set; }
}
