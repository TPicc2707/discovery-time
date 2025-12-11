namespace Activity.Application.Activities.Queries.GetActivitiesByName;

public record GetActivitiesByNameQuery(string Name) : IQuery<GetActivitiesByNameResult>;

public record GetActivitiesByNameResult(IEnumerable<ActivityDto> Activities);
