namespace Activity.Application.Activities.Queries.GetActivitiesByThemeId;

public class GetActivitiesByThemeIdHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetActivitiesByThemeIdQuery, GetActivitiesByThemeIdResult>
{
    public async Task<GetActivitiesByThemeIdResult> Handle(GetActivitiesByThemeIdQuery query, CancellationToken cancellationToken)
    {
        var themeId = ThemeId.Of(query.ThemeId);
        var theme = await unitOfWork.Theme.GetByIdAsync(themeId, cancellationToken);

        if (theme is null)
            throw new ThemeNotFoundException(query.ThemeId);

        var activities = await unitOfWork.Theme.GetAllThemeActivitiesByThemeIdAsync(themeId, cancellationToken);

        return new GetActivitiesByThemeIdResult(activities.ToActivityDtoList());
    }
}
