using Activity.Infrastructure.Data;
using Bogus;
using Discovery.Time.Tests.Data.MockData;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;

namespace Activity.API.Tests;

public class CustomTestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
{
    internal readonly MsSqlContainer MsSqlContainer = new MsSqlBuilder().WithImage("mcr.microsoft.com/mssql/server:2022-latest").WithPassword("Password123").Build();
    internal readonly RabbitMqContainer RabbitMQContainer = new RabbitMqBuilder().WithImage("rabbitmq:3.11-management-alpine").Build();
    public const string _jsonMediaType = "application/json";
    public const int _expectedMaxElaspedMilliseconds = 20000;
    public string ConnectionString => MsSqlContainer.GetConnectionString();
    private readonly Faker _faker = new Faker();


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Staging");

        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if(descriptor is not null)
                services.Remove(descriptor);

            var massTransitService = services.FirstOrDefault(d => d.ServiceType == typeof(IBusControl));

            if (massTransitService is not null)
                services.Remove(massTransitService);

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(ConnectionString);
            });

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

            var sp = services.BuildServiceProvider();

            using(var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                db.Database.EnsureDeleted();

                db.Database.EnsureCreated();

                try
                {
                    var themeTestData = ThemeFakeData.GenerateActivityThemes(_faker.Random.Int(1, 10));
                    var themeIds = themeTestData.Select(d => d.Id.Value).ToList();
                    var activityTestData = ThemeFakeData.GenerateActivities(themeIds, _faker);

                    db.Themes.AddRange(themeTestData);
                    db.Activities.AddRange(activityTestData);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
        });
    }

    public async Task InitializeAsync()
    {
        await MsSqlContainer.StartAsync();
        await RabbitMQContainer.StartAsync().ConfigureAwait(false);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await MsSqlContainer.StartAsync();
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
