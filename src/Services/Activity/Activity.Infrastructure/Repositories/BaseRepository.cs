namespace Activity.Infrastructure.Repositories;

public class BaseRepository<T> : IAsyncRepository<T> where T : class
{
    internal IApplicationDbContext dbContext;
    private readonly ILogger<T> logger;

    public BaseRepository(IApplicationDbContext dbContext, ILogger<T> logger)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        if(dbContext is DbContext context)
        {
            try
            {
                await context.Set<T>().AddAsync(entity);
                logger.LogInformation("Entity of type {EntityType} added to the database.", typeof(T).Name);
                return entity;
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }
        else
        {
            throw new InvalidOperationException("The provided context does not inherit from DbContext.");
        }

    }

    public virtual void Delete(T entity)
    {
        if (dbContext is DbContext context)
        {
            try
            {
                context.Set<T>().Remove(entity);
                logger.LogInformation("Entity of type {EntityType} removed to the database.", typeof(T).Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }
        else
        {
            throw new InvalidOperationException("The provided context does not inherit from DbContext.");
        }
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync()
    {
        if (dbContext is DbContext context)
        {
            try
            {
                return await context.Set<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }
        else
        {
            throw new InvalidOperationException("The provided context does not inherit from DbContext.");
        }
    }

    public virtual async Task<T> GetByIdAsync<TValue>(TValue id, CancellationToken cancellationToken)
    {
        if (dbContext is DbContext context)
        {
            try
            {
                return await context.Set<T>().FindAsync([id], cancellationToken);
            }
            catch(FormatException ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }
        else
        {
            throw new InvalidOperationException("The provided context does not inherit from DbContext.");
        }
    }

    public virtual void Update(T entity)
    {
        if (dbContext is DbContext context)
        {
            try
            {
                context.Set<T>().Attach(entity);
                logger.LogInformation("Entity of type {EntityType} updated to the database.", typeof(T).Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }
        else
        {
            throw new InvalidOperationException("The provided context does not inherit from DbContext.");
        }
    }
}
