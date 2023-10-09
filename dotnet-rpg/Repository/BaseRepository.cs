using dotnet_rpg.Specifications.Interfaces;

namespace dotnet_rpg.Repository;

public abstract class BaseRepository<T> : IRepository<T> where T : class, IEntityWithId
{
    protected readonly DataContext _dbContext;

    public BaseRepository(DataContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual async Task<List<T>> ListAsync(ISpecification<T> spec)
    {
        var query = ApplySpecification(spec);
        return await query.ToListAsync();
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        var query = _dbContext.Set<T>().AsQueryable();

        if (spec.Criteria != null)
        {
            query = query.Where(spec.Criteria);
        }

        query = spec.Includes.Aggregate(query,
            (current, include) => current.Include(include));

        if (spec.OrderBy != null)
        {
            query = spec.OrderBy(query);
        }

        return query;
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public virtual async Task<List<T>> GetAllAsync()
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    public virtual void Add(T entity)
    {
        _dbContext.Set<T>().Add(entity);
    }

    public virtual void Delete(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
    }

    public virtual void Update(T entity)
    {
        _dbContext.Set<T>().Update(entity);
    }

    public virtual async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public virtual async Task<List<T>> GetByIdsAsync(List<int> ids)
    {
        return await _dbContext.Set<T>().Where(entity => ids.Contains(entity.Id)).ToListAsync();
    }
}
