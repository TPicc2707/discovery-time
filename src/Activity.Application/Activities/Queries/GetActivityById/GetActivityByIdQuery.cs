namespace Activity.Application.Activities.Queries.GetActivityById;

public record GetActivityByIdQuery(Guid Id) : IQuery<GetActivityByIdResult>;

public record GetActivityByIdResult(ActivityDto Activity);
