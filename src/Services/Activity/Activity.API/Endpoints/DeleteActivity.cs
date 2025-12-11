namespace Activity.API.Endpoints;

public record DeleteActivityResponse(bool IsSuccess);

public class DeleteActivity : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/activities/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteActivityCommand(id));

            var response = result.Adapt<DeleteActivityResponse>();

            return Results.Ok(response);
        })
        .WithName("DeleteActivity")
        .Produces<DeleteActivityResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithSummary("Delete Activity")
        .WithDescription("Delete Activity");
    }
}
