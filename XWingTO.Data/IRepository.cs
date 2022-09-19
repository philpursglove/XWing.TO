namespace XWingTO.Data
{
    public interface IRepository<T, TKey> where T : class
    {
        Task<T> Get(TKey id);

        Task<IQueryable<T>> Query();

        Task Add(T entity);

        Task Update(T entity);

        Task Delete(T entity);
    }
}
