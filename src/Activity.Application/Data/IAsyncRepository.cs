namespace Activity.Application.Data;

public interface IAsyncRepository<T> where T : class
{
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<T> GetByIdAsync<TValue>(TValue id, CancellationToken cancellationToken);
    Task<T> AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
