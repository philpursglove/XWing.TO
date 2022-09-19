namespace XWingTO.Data;

public class Repository<T, TKey> : IRepository<T, TKey> where T : class
{
    private readonly DbContext _dbContext;
    public Repository(DbContext context)
    {
        _dbContext = context;
    }

    public async Task Add(T entity)
    {
        _dbContext.Set<T>().Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(T entity)
    {
        _dbContext.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<T> Get(TKey id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task<IQueryable<T>> Query()
    {
        return _dbContext.Set<T>().AsQueryable();
    }

    public async Task Update(T entity)
    {
        _dbContext.Update(entity);
        await _dbContext.SaveChangesAsync();
    }
}