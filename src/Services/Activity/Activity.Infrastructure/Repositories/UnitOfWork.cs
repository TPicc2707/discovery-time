namespace Activity.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private IApplicationDbContext _dbContext;
    private ILogger<UnitOfWork> _logger;
    private readonly ILogger<Theme> _themeLogger;

    public UnitOfWork(IApplicationDbContext dbContext, ILogger<UnitOfWork> logger, ILogger<Theme> themeLogger)
    {
        _dbContext = dbContext;
        _logger = logger;
        _themeLogger = themeLogger;
        InitializeRepositories();
    }
    public IThemeRepository Theme { get; private set; } = default!;

    public async Task<int> Complete(CancellationToken cancellationToken)
    {
        try
        {
            var saved = await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Data was saved to database.");
            return saved;
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                if(_dbContext is DbContext dbContext)
                    dbContext.Dispose();
            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void InitializeRepositories()
    {
        Theme = new ThemeRepository(_dbContext, _themeLogger);
    }
}
