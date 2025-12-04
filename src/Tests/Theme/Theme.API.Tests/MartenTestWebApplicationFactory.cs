namespace Theme.API.Tests;

public class MartenTestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
{
    internal readonly PostgreSqlContainer PostgresContainer = new PostgreSqlBuilder().WithDatabase("testdb").WithUsername("marten_user").WithPassword("marten_password").Build();
    public const string _jsonMediaType = "application/json";
    public const int _expectedMaxElaspedMilliseconds = 20000;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Staging");

        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDocumentStore));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddMarten(x =>
            {
                x.Connection(PostgresContainer.GetConnectionString());
                x.AutoCreateSchemaObjects = JasperFx.AutoCreate.All;
            }).ApplyAllDatabaseChangesOnStartup();
        });
    }

    public async Task InitializeAsync()
    {
        await PostgresContainer.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await PostgresContainer.DisposeAsync();
    }

    public void AssertResponseWithContentAsync<T>(Stopwatch stopwatch, HttpResponseMessage response, System.Net.HttpStatusCode expectedStatusCode, T content)
    {
        AssertCommonResponseParts(stopwatch, response, expectedStatusCode);
        Assert.Equal(_jsonMediaType, response.Content.Headers.ContentType?.MediaType);
        Assert.IsType<T>(content);
    }

    public void AssertCommonResponseParts(Stopwatch stopwatch, HttpResponseMessage response, System.Net.HttpStatusCode expectedStatusCode)
    {
        Assert.Equal(expectedStatusCode, response.StatusCode);
        Assert.True(stopwatch.ElapsedMilliseconds < _expectedMaxElaspedMilliseconds);
    }

    public StringContent GetJsonStringContent<T>(T model) => new(JsonSerializer.Serialize(model), Encoding.UTF8, _jsonMediaType);


}
