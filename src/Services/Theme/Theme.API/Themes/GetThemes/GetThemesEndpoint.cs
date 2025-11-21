namespace Theme.API.Themes.GetThemes;

public record GetThemesRequest(int? PageNumber = 1, int? PageSize = 10);

public record GetThemesResponse(IEnumerable<Models.Theme> Themes);

public class GetThemesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/themes", async ([AsParameters] GetThemesRequest request, ISender sender) =>
        {
            var query = request.Adapt<GetThemesQuery>();

            var result = await sender.Send(query);

            var response = result.Adapt<GetThemesResponse>();

            return Results.Ok(response);
        })
        .WithName("GetThemes")
        .Produces<GetThemesResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Themes")
        .WithDescription("Get Themes");
    }
}
