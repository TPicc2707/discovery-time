namespace Activity.API.Endpoints;

public record UpdateActivityRequest(ActivityDto Activity);

public record UpdateActivityResponse(bool IsSuccess);

public class UpdateActivity : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/activities", async(UpdateActivityRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateActivityCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<UpdateActivityResponse>();

            return Results.Ok(response);
        })
        .WithName("UpdateActivity")
        .Produces<UpdateActivityResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithSummary("Update Activity")
        .WithDescription("Update Activity");
    }
}
