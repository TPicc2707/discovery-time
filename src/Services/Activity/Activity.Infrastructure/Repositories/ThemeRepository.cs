namespace Activity.Infrastructure.Repositories;

public class ThemeRepository : BaseRepository<Theme>, IThemeRepository
{
    private readonly ILogger<Theme> _logger;

    public ThemeRepository(IApplicationDbContext dbContext, ILogger<Theme> logger) : base(dbContext, logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public virtual async Task<List<Domain.Models.Activity>> GetAllThemeActivitiesAsync(int pageIndex, int pageSize)
    {
        try
        {
            return await dbContext.Activities.OrderBy(x => x.Name.Value).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }

    public virtual async Task<Domain.Models.Activity> GetThemeActivityByIdAsync(ActivityId activityId, CancellationToken cancellationToken)
    {
        try
        {
            return await dbContext.Activities.FindAsync([activityId], cancellationToken);
        }
        catch(FormatException ex)
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

    public virtual async Task<List<Domain.Models.Activity>> GetAllThemeActivitiesByThemeIdAsync(ThemeId themeId, CancellationToken cancellationToken)
    {
        try
        {
            return await dbContext.Activities.Where(x => x.ThemeId == themeId).OrderBy(x => x.Name.Value).ToListAsync(cancellationToken);
        }
        catch (FormatException ex)
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

    public virtual async Task<List<Domain.Models.Activity>> GetAllThemeActivitiesByNameAsync(ActivityName name, CancellationToken cancellationToken)
    {
        try
        {
            return await dbContext.Activities.AsNoTracking().Where(x => x.Name.Value.ToLower().Contains(name.Value.ToLower())).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }

    public virtual async Task<long> GetThemeActivitiesLongCountAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await dbContext.Activities.LongCountAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }

    }

    public virtual async Task<Domain.Models.Activity> CreateThemeActivityAsync(Domain.Models.Activity activity)
    {
        try
        {
            await dbContext.Activities.AddAsync(activity);

            _logger.LogInformation($"Object {activity.Id} was added to table.");

            return activity;
        }
        catch(InvalidOperationException ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
        catch(NullReferenceException ex)
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

    public virtual void UpdateThemeActivity(Domain.Models.Activity activity)
    {
        try
        {
            dbContext.Activities.Update(activity);

            _logger.LogInformation($"Object {activity.Id} was updated in the table.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
        catch (NullReferenceException ex)
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

    public virtual void DeleteThemeActivity(Domain.Models.Activity activity)
    {
        try
        {
            dbContext.Activities.Remove(activity);

            _logger.LogInformation($"Object {activity.Id} was deleted from table.");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }
}
