namespace Activity.API.Endpoints;

public record GetActivitiesByNameResponse(IEnumerable<ActivityDto> Activities);

public class GetActivitiesByName : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("activities/name/{name}", async(string name, ISender sender) =>
        {
            var result = await sender.Send(new GetActivitiesByNameQuery(name));

            var response = result.Adapt<GetActivitiesByNameResponse>();

            return Results.Ok(response);
        })
        .WithName("GetActivitiesByName")
        .Produces<GetActivitiesByNameResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Activities By Name")
        .WithDescription("Get Activities By Name");
    }
}
