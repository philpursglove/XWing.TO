namespace XWingTO.Data
{
    public interface IRepository<T, TKey> where T : class
    {
        T Get(TKey id);

        IQueryable<T> Query();

        void Add(T entity);

        void Update(T entity);

        void Delete(T entity);
    }

    public class Repository<T, TKey> : IRepository<T, TKey> where T : class
    {
        private readonly DbContext _dbContext;
        public Repository(DbContext context)
        {
            _dbContext = context;
        }

        public void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(T entity)
        {
            _dbContext.Remove(entity);
            _dbContext.SaveChanges();
        }

        public T Get(TKey id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public IQueryable<T> Query()
        {
            return _dbContext.Set<T>().AsQueryable();
        }

        public void Update(T entity)
        {
            _dbContext.Update(entity);
            _dbContext.SaveChanges();
        }
    }
}
