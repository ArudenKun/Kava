using System;

namespace Kava.Data.Abstractions;

public interface ITimeStamp
{
    /// <summary>
    /// Gets or sets the date when this entity was created.
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the data then this entity was updated.
    /// </summary>
    DateTime UpdatedAt { get; set; }
}
