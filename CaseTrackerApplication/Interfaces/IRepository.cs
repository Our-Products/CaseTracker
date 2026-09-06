namespace CaseTrackerApplication.Interfaces
{
    /// <summary>
    /// Generic repository interface providing base CRUD operations.
    /// All specific repositories should extend this interface with entity-specific queries.
    /// </summary>
    /// <typeparam name="T">The entity type managed by this repository</typeparam>
    public interface IRepository<T> where T : class
    {
        // Query operations
        /// <summary>
        /// Retrieve an entity by its ID asynchronously.
        /// </summary>
        Task<T?> GetByIdAsync(object id);

        /// <summary>
        /// Retrieve all entities asynchronously.
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Find entities matching a predicate asynchronously.
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate);

        /// <summary>
        /// Check if an entity exists by its ID asynchronously.
        /// </summary>
        Task<bool> ExistsAsync(object id);

        // Command operations
        /// <summary>
        /// Add a new entity asynchronously.
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Add multiple entities asynchronously.
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Update an existing entity asynchronously.
        /// </summary>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Delete an entity by its ID asynchronously.
        /// </summary>
        Task DeleteAsync(object id);

        /// <summary>
        /// Delete an entity asynchronously.
        /// </summary>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Delete multiple entities asynchronously.
        /// </summary>
        Task DeleteRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Save all changes to the database asynchronously.
        /// </summary>
        Task SaveChangesAsync();

        /// <summary>
        /// Get the total count of entities asynchronously.
        /// </summary>
        Task<int> CountAsync();
    }
}
