namespace Activity.API.Endpoints;

public record CreateActivityRequest(ActivityDto Activity);

public record CreateActivityResponse(Guid Id);

public class CreateActivity : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/activities", async (CreateActivityRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateActivityCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CreateActivityResponse>();

            return Results.Created($"/activities/{response.Id}", response);
        })
        .WithName("CreateActivity")
        .Produces<CreateActivityResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create Activity")
        .WithDescription("Create Activity");
    }
}
