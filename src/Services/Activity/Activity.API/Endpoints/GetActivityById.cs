namespace Activity.API.Endpoints;

public record GetActivityByIdResponse(ActivityDto Activity);

public class GetActivityById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("activites/{id}", async(Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetActivityByIdQuery(id));

            var response = result.Adapt<GetActivityByIdResponse>();

            return Results.Ok(response);
        })
        .WithName("GetActivityById")
        .Produces<GetActivityByIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get Activity By Id")
        .WithDescription("Get Activity By Id"); 
    }
}
