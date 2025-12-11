namespace Activity.Application.Activities.Queries.GetActivityById;

public class GetActivityByIdHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetActivityByIdQuery, GetActivityByIdResult>
{
    public async Task<GetActivityByIdResult> Handle(GetActivityByIdQuery query, CancellationToken cancellationToken)
    {
        var activityId = ActivityId.Of(query.Id);
        var activity = await unitOfWork.Theme.GetThemeActivityByIdAsync(activityId, cancellationToken);

        if (activity is null)
            throw new ActivityNotFoundException(query.Id);

        return new GetActivityByIdResult(activity.ToSingleActivityDto());
    }
}
