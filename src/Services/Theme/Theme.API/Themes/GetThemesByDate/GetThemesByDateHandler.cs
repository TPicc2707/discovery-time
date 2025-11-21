namespace Theme.API.Themes.GetThemesByDate;

public record GetThemesByDateCommand(DateTime Date) : IQuery<GetThemesByDateResult>;

public record GetThemesByDateResult(IEnumerable<Models.Theme> Themes);

public class GetThemesByDateHandler
    (IDocumentSession documentSession)
    : IQueryHandler<GetThemesByDateCommand, GetThemesByDateResult>
{
    public async Task<GetThemesByDateResult> Handle(GetThemesByDateCommand query, CancellationToken cancellationToken)
    {
        var themes = await documentSession.Query<Models.Theme>()
                .Where(x => x.StartDate != null && x.EndDate != null)
                .ToListAsync();

        themes = themes.Where(x => x.StartDate.ToShortDateString() == query.Date.ToShortDateString() || x.EndDate.ToShortDateString() == query.Date.ToShortDateString()).ToList();

        return new GetThemesByDateResult(themes);
    }
}
