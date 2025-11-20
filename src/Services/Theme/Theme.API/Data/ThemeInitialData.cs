namespace Theme.API.Data;

public class ThemeInitialData : IInitialData
{
    public async Task Populate(IDocumentStore store, CancellationToken cancellationToken)
    {
        using var session = store.LightweightSession();

        if (await session.Query<Models.Theme>().AnyAsync())
            return;

        session.Store(GetPreconfiguredThemes());

        await session.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<Models.Theme> GetPreconfiguredThemes() => new List<Models.Theme>()
    {
        new()
        {
            Id = new Guid("2f7fb473-031e-44f1-b97b-dbb48a5aa99f"),
            Name = "Dummy Theme",
            Number = 1,
            Letter = "Aa",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(7),
            CreatedBy = "tony.pic",
            CreatedDate = DateTime.Now,
            ModifiedBy = "tony.pic",
            ModifiedDate = DateTime.Now,
        }
    };
}
