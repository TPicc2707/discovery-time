namespace Theme.API.Themes.GetThemesByName;

public record GetThemesByNameResponse(IEnumerable<Models.Theme> Themes);

public class GetThemesByNameEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/themes/name/{name}",
            async (string name, ISender sender) =>
            {
                var result = await sender.Send(new GetThemesByNameQuery(name));

                var response = result.Adapt<GetThemesByNameResponse>();

                return Results.Ok(response);
            })
            .WithName("GetThemesByName")
            .Produces<GetThemesByNameResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Themes By Name")
            .WithDescription("Get Themes By Name");
    }
}
