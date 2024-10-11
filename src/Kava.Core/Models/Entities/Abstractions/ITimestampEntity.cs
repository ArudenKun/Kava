namespace Kava.Core.Models.Entities.Abstractions;

public interface ITimestampEntity
{
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
