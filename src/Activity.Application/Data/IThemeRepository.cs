namespace Activity.Application.Data;

public interface IThemeRepository : IAsyncRepository<Theme>
{
    Task<List<Domain.Models.Activity>> GetAllThemeActivitiesAsync(int pageIndex, int pageSize);
    Task<Domain.Models.Activity> GetThemeActivityByIdAsync(ActivityId activityId, CancellationToken cancellationToken);
    Task<List<Domain.Models.Activity>> GetAllThemeActivitiesByThemeIdAsync(ThemeId themeId, CancellationToken cancellationToken);
    Task<List<Domain.Models.Activity>> GetAllThemeActivitiesByNameAsync(ActivityName name, CancellationToken cancellationToken);
    Task<Domain.Models.Activity> CreateThemeActivityAsync(Domain.Models.Activity activity);
    Task<long> GetThemeActivitiesLongCountAsync(CancellationToken cancellationToken);
    void UpdateThemeActivity(Domain.Models.Activity activity);
    void RemoveThemeActivity(Domain.Models.Activity activity);
}
