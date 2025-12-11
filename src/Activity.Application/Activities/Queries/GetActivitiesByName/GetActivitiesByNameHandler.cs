namespace Activity.Application.Activities.Queries.GetActivitiesByName;

public class GetActivitiesByNameHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetActivitiesByNameQuery, GetActivitiesByNameResult>
{
    public async Task<GetActivitiesByNameResult> Handle(GetActivitiesByNameQuery query, CancellationToken cancellationToken)
    {
        var name = ActivityName.Of(query.Name);

        var activities = await unitOfWork.Theme.GetAllThemeActivitiesByNameAsync(name, cancellationToken);

        return new GetActivitiesByNameResult(activities.ToActivityDtoList());
    }
}
