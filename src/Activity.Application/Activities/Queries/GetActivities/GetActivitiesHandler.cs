namespace Activity.Application.Activities.Queries.GetActivities;

public class GetActivitiesHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetActivitiesQuery, GetActivitiesResult>
{
    public async Task<GetActivitiesResult> Handle(GetActivitiesQuery query, CancellationToken cancellationToken)
    {
        var pageIndex = query.PaginationRequest.PageIndex;
        var pageSize = query.PaginationRequest.PageSize;

        var totalCount = await unitOfWork.Theme.GetThemeActivitiesLongCountAsync(cancellationToken);

        var activities = await unitOfWork.Theme.GetAllThemeActivitiesAsync(pageIndex, pageSize);

        return new GetActivitiesResult(new PaginatedResult<ActivityDto>(pageIndex, pageSize, totalCount, activities.ToActivityDtoList()));
    }
}
