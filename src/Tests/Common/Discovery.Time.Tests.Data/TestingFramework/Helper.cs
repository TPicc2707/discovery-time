namespace Discovery.Time.Tests.Data.TestingFramework;

public static class Helper
{
    public static T GetRequiredService<T>()
    {
        var provider = Services().BuildServiceProvider();
        return provider.GetRequiredService<T>();
    }
    
    public static ServiceCollection Services()
    {
        var services = new ServiceCollection();

        services.AddScoped<IMockNSubstituteMethods, MockingTestingFramework>();

        return services;
    }

}
