namespace Kava.Core.Models.Abstractions;

public interface ITimestampEntity
{
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
