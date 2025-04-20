using System.Linq.Expressions;

namespace RO.DevTest.Application.Contracts.Persistance.Repositories;

public interface IBaseRepository<T> where T : class 
{
    /// <summary>
    /// Creates a new entity in the database
    /// </summary>
    /// <param name="entity"> The entity to be create </param>
    /// <param name="cancellationToken"> Cancellation token </param>
    /// <returns> The created entity </returns>
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds the first entity that matches with the <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">
    /// The <see cref="Expression"/> to be used while
    /// looking for the entity
    /// </param>
    /// <returns>
    /// The <typeparamref name="T"/> entity, if found. Null otherwise. </returns>
    Task<T?> GetAsync(CancellationToken cancellationToken, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Updates the specified entity in the database context
    /// and persists the changes.
    /// </summary>
    /// <param name="entity">The entity to update</param>
    Task UpdateAsync(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Returns an <see cref="IQueryable{T}"/> for further query composition,
    /// with optional includes for navigation properties.
    /// </summary>
    /// <param name="predicate">The filter expression to apply.</param>
    /// <param name="includes">
    /// Optional related entities to include in the query results.
    /// Useful for eager loading of navigation properties.
    /// </param>
    /// <returns>
    /// A queryable collection of <typeparamref name="T"/> that matches the given predicate,
    /// with optional navigation properties included.
    /// </returns>

    IQueryable<T> GetQueryable(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Deletes one entry from the database
    /// </summary>
    /// <param name="entity"> The entity to be deleted </param>
    Task DeleteAsync(T entity, CancellationToken cancellationToken);
}