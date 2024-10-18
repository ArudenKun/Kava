using JetBrains.Annotations;

namespace Kava.Models.Abstractions;

public abstract class BaseEntity
{
    public Ulid Id { get; init; } = Ulid.NewUlid();

    [UsedImplicitly]
    public DateTime? CreatedAt { get; set; }

    [UsedImplicitly]
    public DateTime? UpdatedAt { get; set; }
}
