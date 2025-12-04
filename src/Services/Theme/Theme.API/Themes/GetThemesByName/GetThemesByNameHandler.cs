
namespace Theme.API.Themes.GetThemesByName;

public record GetThemesByNameQuery(string Name) : IQuery<GetThemesByNameResult>;

public record GetThemesByNameResult(IEnumerable<Models.Theme> Themes);

public class GetThemesByNameHandler
    (IDocumentSession documentSession)
    : IQueryHandler<GetThemesByNameQuery, GetThemesByNameResult>
{
    public async Task<GetThemesByNameResult> Handle(GetThemesByNameQuery query, CancellationToken cancellationToken)
    {
        var themes = await documentSession.Query<Models.Theme>()
            .Where(t => t.Name.ToLower().Contains(query.Name.ToLower()))
            .ToListAsync();

        return new GetThemesByNameResult(themes);
    }
}
