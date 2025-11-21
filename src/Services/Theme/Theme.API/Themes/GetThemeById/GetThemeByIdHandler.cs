namespace Theme.API.Themes.GetThemeById;

public record GetThemeByIdQuery(Guid Id) : IQuery<GetThemeByIdResult>;

public record GetThemeByIdResult(Models.Theme Theme);

public class GetThemeByIdHandler
    (IDocumentSession documentSession)
    : IQueryHandler<GetThemeByIdQuery, GetThemeByIdResult>
{
    public async Task<GetThemeByIdResult> Handle(GetThemeByIdQuery query, CancellationToken cancellationToken)
    {
        var theme = await documentSession.LoadAsync<Models.Theme>(query.Id, cancellationToken);

        if (theme is null)
            throw new ThemeNotFoundException(query.Id);

        return new GetThemeByIdResult(theme);
    }
}
