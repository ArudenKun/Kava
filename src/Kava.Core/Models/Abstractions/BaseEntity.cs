using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Kava.Core.Models.Abstractions;

public abstract class BaseEntity : ITimestampEntity
{
    public Ulid Id { get; init; } = Ulid.NewUlid();

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
