using System;

namespace Kava.Data.Abstractions;

/// <summary>
/// Represents an entity with a unique identifier.
/// </summary>
public interface IKeyed
{
    /// <summary>
    /// Gets or sets the unique identifier for the property.
    /// </summary>
    Ulid Id { get; set; }
}
