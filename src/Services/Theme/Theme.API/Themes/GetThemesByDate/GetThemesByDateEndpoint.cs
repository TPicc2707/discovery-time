namespace Theme.API.Themes.GetThemesByDate;

public record GetThemesByDateResponse(IEnumerable<Models.Theme> Themes);

public class GetThemesByDateEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/themes/date/{date}", async (DateTime date, ISender sender) =>
        {
            var result = await sender.Send(new GetThemesByDateQuery(date));

            var response = result.Adapt<GetThemesByDateResponse>();

            return Results.Ok(response);
        })
        .WithName("GetThemesByDate")
        .Produces<GetThemesByDateResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get Themes By Date")
        .WithDescription("Get Themes By Date");
    }
}
