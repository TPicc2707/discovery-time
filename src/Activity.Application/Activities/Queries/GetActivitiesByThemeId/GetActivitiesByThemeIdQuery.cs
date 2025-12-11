namespace Activity.Application.Activities.Queries.GetActivitiesByThemeId;

public record GetActivitiesByThemeIdQuery(Guid ThemeId) : IQuery<GetActivitiesByThemeIdResult>;

public record GetActivitiesByThemeIdResult(IEnumerable<ActivityDto> Activities);