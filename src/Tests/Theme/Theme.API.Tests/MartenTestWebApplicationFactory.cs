using BuildingBlocks.Exceptions.Handler;
using MassTransit;
using RabbitMQ.Client;
using Testcontainers.RabbitMq;

namespace Theme.API.Tests;

public class MartenTestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
{
    internal readonly PostgreSqlContainer PostgresContainer = new PostgreSqlBuilder().WithDatabase("testdb").WithUsername("marten_user").WithPassword("marten_password").Build();
    internal readonly RabbitMqContainer RabbitMQContainer = new RabbitMqBuilder().WithImage("rabbitmq:3.11-management-alpine").Build();
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

            var massTransitService = services.FirstOrDefault(d => d.ServiceType == typeof(IBusControl));

            if (massTransitService != null)
                services.Remove(massTransitService);

            services.AddMarten(x =>
            {
                x.Connection(PostgresContainer.GetConnectionString());
                x.AutoCreateSchemaObjects = JasperFx.AutoCreate.All;
            }).ApplyAllDatabaseChangesOnStartup();

            services.AddMassTransitTestHarness(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    var connectionFactory = new ConnectionFactory
                    {
                        Uri = new Uri(RabbitMQContainer.GetConnectionString())
                    };

                    cfg.Host(connectionFactory.Uri);
                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddExceptionHandler<CustomExceptionHandler>();
        });
    }

    public async Task InitializeAsync()
    {
        await PostgresContainer.StartAsync();
        await RabbitMQContainer.StartAsync().ConfigureAwait(false);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await PostgresContainer.DisposeAsync();
        await RabbitMQContainer.DisposeAsync().ConfigureAwait(false);

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
