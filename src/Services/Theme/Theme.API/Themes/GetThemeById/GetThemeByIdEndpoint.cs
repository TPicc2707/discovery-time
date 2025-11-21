namespace Theme.API.Themes.GetThemeById;

public record GetThemeByIdResponse(Models.Theme Theme);

public class GetThemeByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/themes/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetThemeByIdQuery(id));

            var response = result.Adapt<GetThemeByIdResponse>();

            return Results.Ok(response);
        })
        .WithName("GetLeagueById")
        .Produces<GetThemeByIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get League By Id")
        .WithDescription("Get League By Id");
    }
}
