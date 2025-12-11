namespace Activity.API.Endpoints;

public record GetActivitiesResponse(PaginatedResult<ActivityDto> Activities);

public class GetActivities : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/activities", async ([AsParameters] PaginationRequest request, ISender sender) =>
        {
            var result = await sender.Send(new GetActivitiesQuery(request));

            var response = result.Adapt<GetActivitiesResponse>();

            return Results.Ok(response);
        })
        .WithName("GetActivities")
        .Produces<GetActivitiesResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Activities")
        .WithDescription("Get Activities");
    }
}
