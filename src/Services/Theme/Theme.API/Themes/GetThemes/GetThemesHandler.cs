namespace Theme.API.Themes.GetThemes;

public record GetThemesQuery(int? PageNumber = 1, int? PageSize = 10) : IQuery<GetThemesResult>;

public record GetThemesResult(IEnumerable<Models.Theme> Themes);

public class GetThemesHandler
    (IDocumentSession documentSession)
    : IQueryHandler<GetThemesQuery, GetThemesResult>
{
    public async Task<GetThemesResult> Handle(GetThemesQuery query, CancellationToken cancellationToken)
    {
        var themes = await documentSession.Query<Models.Theme>()
            .ToPagedListAsync(query.PageNumber ?? 1, query.PageSize ?? 10, cancellationToken);

        return new GetThemesResult(themes);
    }
}
