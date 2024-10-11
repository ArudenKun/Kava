using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Kava.Models.Entities.Abstractions;

public abstract class BaseEntity : ITimestampEntity
{
    [PublicAPI]
    [Key]
    public Ulid Id { get; init; } = Ulid.NewUlid();

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
