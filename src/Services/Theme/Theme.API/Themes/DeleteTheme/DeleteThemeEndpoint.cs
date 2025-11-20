namespace Theme.API.Themes.DeleteTheme;

public record DeleteThemeResponse(bool IsSuccess);

public class DeleteThemeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/themes/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteThemeCommand(id));

            var response = result.Adapt<DeleteThemeResponse>();

            return Results.Ok(response);
        })
        .WithName("DeleteTheme")
        .Produces<DeleteThemeResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithSummary("Delete Theme")
        .WithDescription("Delete Theme");
    }
}
