namespace Activity.API.Endpoints;

public record GetActivitiesByThemeIdResponse(IEnumerable<ActivityDto> Activities);

public class GetActivitiesByThemeId : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("activities/theme/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetActivitiesByThemeIdQuery(id));

            var response = result.Adapt<GetActivitiesByThemeIdResponse>();

            return Results.Ok(response);
        })
        .WithName("GetActivitiesByThemeId")
        .Produces<GetActivitiesByThemeIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get Activities By Theme Id")
        .WithDescription("Get Activities By Theme Id");
    }
}
