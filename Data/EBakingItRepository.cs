using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

public class EBakingItRepository<T> : IBakingItRepository<T> where T : class
{
    // Best practices:
    // Use _context.Set<T>():  make the repository work generically for all entities
    // Check for null before deleting:  prevents runtime errors when an entity doesn't exist
    // Separate concerns:  this keeps data logic out of controllers and services.
    private readonly BakingItContext _context;

    public EBakingItRepository(BakingItContext context)
    {
        _context = context;
    }

    // public async Task<IEnumerable<T>> GetAllAsync()
    // {
    //     return await _context.Set<T>().ToListAsync();
    // }

    public async Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? queryModifier = null)
    {
        IQueryable<T> query = _context.Set<T>();

        // Apply the modifier if provided (e.g., Includes)
        if (queryModifier != null)
        {
            query = queryModifier(query);
        }

        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().AnyAsync(predicate);
    }
}