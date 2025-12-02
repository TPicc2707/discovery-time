namespace Theme.API.Themes.CreateTheme;

public record CreateThemeRequest(string Name, int Number, string Letter, DateTime StartDate, DateTime EndDate, string CreatedBy, string ModifiedBy);

public record CreateThemeResponse(Guid Id);

public class CreateThemeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/themes", async (CreateThemeRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateThemeCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CreateThemeResponse>();

            return Results.Created($"/themes/{response.Id}", response);
        })
        .WithName("CreateTheme")
        .Produces<CreateThemeResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create Theme")
        .WithDescription("Create Theme");
    }
}
