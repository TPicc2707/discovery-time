using Activity.Application.Data;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace Discovery.Time.Tests.Data.MockData;

public class SetupSqliteDb
{
    public static async Task SetupDb(IApplicationDbContext context, Faker faker)
    {
        var themeTestData = ThemeFakeData.GenerateActivityThemes(faker.Random.Int(1, 20));
        var activityTestData = ThemeFakeData.GenerateActivities(themeTestData.Select(x => x.Id.Value).ToList(), faker);

        await context.Themes.AddRangeAsync(themeTestData);
        await context.Activities.AddRangeAsync(activityTestData);

        await context.SaveChangesAsync(CancellationToken.None);
    } 
    
    public static async Task DeleteData(IApplicationDbContext context)
    {
        if (context is DbContext dbContext)
        {
            dbContext.Database.EnsureDeleted();
            await dbContext.DisposeAsync();
        }

    }
}
