namespace Theme.API.Themes.UpdateTheme;

public record UpdateThemeRequest(Guid Id, string Name, int Number, string Letter, DateTime StartDate, DateTime EndDate, string ModifiedBy);

public record UpdateThemeResponse(bool IsSuccess);

public class UpdateThemeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/themes", async (UpdateThemeRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateThemeCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<UpdateThemeResponse>();

            return Results.Ok(response);

        })
        .WithName("UpdateTheme")
        .Produces<UpdateThemeResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithSummary("Update Theme")
        .WithDescription("Update Theme");

    }
}
