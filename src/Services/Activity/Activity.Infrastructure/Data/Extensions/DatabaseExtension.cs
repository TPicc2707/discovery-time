namespace Activity.Infrastructure.Data.Extensions;

public static class DatabaseExtension
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Database.MigrateAsync().GetAwaiter().GetResult();

        await SeedAsync(context);
    }

    private static async Task SeedAsync(ApplicationDbContext context)
    {
        await SeedThemeAsync(context);
        await SeedActivitiesAsync(context);
    }

    private static async Task SeedThemeAsync(ApplicationDbContext context)
    {
        if(!await context.Themes.AnyAsync())
        {
            await context.Themes.AddRangeAsync(InitialData.Themes);
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedActivitiesAsync(ApplicationDbContext context)
    {
        if(!await context.Activities.AnyAsync())
        {
            await context.Activities.AddRangeAsync(InitialData.Activities);
            await context.SaveChangesAsync();
        }
    }
}
