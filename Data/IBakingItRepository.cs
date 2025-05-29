public interface IBakingItRepository<T> where T : class
{
    // Best practices:
    // Use Generics:  this prevents duplication and makes the repo reusable for multiple entities
    // Asynchronous methods:  keeps performance optimized in web apps
    // Nullable Return Types(T?):  Explicitly shows that GetByIdAsync() might return null
    
    // Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? queryModifier = null);
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}