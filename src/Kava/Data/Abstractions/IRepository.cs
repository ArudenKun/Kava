using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kava.Data.Abstractions;

/// <summary>
/// A generic repository interface for data access.
/// </summary>
public interface IRepository
{
    /// <summary>
    /// Retrieves all entities of type T from the data source.
    /// </summary>
    /// <typeparam name="TEntity">The type of entities to retrieve.</typeparam>
    /// <returns>An IAsyncEnumerable collection of entities.</returns>
    IAsyncEnumerable<TEntity> GetAllAsync<TEntity>()
        where TEntity : class, IEntity, new();

    /// <summary>
    /// Retrieves all entities of type T from the data source.
    /// </summary>
    /// <typeparam name="TEntity">The type of entities to retrieve.</typeparam>
    /// <returns>An IEnumerable collection of entities.</returns>
    IEnumerable<TEntity> GetAll<TEntity>()
        where TEntity : class, IEntity, new();

    /// <summary>
    /// Adds a new entity of type T to the collection.
    /// </summary>
    /// <param name="entity">The entity to be added.</param>
    Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class, IEntity, new();

    /// <summary>
    /// Updates the specified entity in the database.
    /// </summary>
    /// <param name="entity">The entity to be updated.</param>
    Task UpdateAsync<TEntity>(TEntity entity)
        where TEntity : class, IEntity, new();

    /// <summary>
    /// Deletes an item with the specified id.
    /// </summary>
    /// <param name="id">The unique identifier of the item to be deleted.</param>
    /// <remarks>
    /// The Delete method removes an item from the system based on its id.
    /// This operation is irreversible and cannot be undone.
    /// </remarks>
    Task DeleteAsync<TEntity>(Ulid id)
        where TEntity : class, IEntity, new();

    /// <summary>
    /// Retrieves an object by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the object.</param>
    /// <returns>The object with the given identifier, or null if not found.</returns>
    Task<TEntity?> GetByIdAsync<TEntity>(Ulid id)
        where TEntity : class, IEntity, new();
}
