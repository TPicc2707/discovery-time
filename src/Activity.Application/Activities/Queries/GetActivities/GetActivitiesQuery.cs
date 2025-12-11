using BuildingBlocks.Pagination;

namespace Activity.Application.Activities.Queries.GetActivities;

public record GetActivitiesQuery(PaginationRequest PaginationRequest)
    : IQuery<GetActivitiesResult>;

public record GetActivitiesResult(PaginatedResult<ActivityDto> Activities);
