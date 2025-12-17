using Discovery.Time.Tests.Data.MockData;
using MassTransit;

namespace Theme.API.Tests.Themes.GetThemeById;

public class GetThemeByIdEndpointTests : IClassFixture<MartenTestWebApplicationFactory<Program>>
{
    public readonly HttpClient _httpClient;
    public readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly Faker _faker;
    private readonly MartenTestWebApplicationFactory<Program> _factory;
    private readonly IDocumentStore _store;

    public GetThemeByIdEndpointTests(MartenTestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
        _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
        _faker = new Faker();
        _store = _factory.Services.GetRequiredService<IDocumentStore>();
    }

    [Fact]
    public async Task GetThemeByIdEndpoint_MapGet_WhenCalled_Should_Return_OK_Result()
    {
        //Arrange
        var expectedStatusCode = System.Net.HttpStatusCode.OK;
        var themes = ThemeFakeData.GenerateThemes(_faker.Random.Int(1, 10)).ToArray();
        var expectedTheme = themes.First();
        var expectedId = expectedTheme.Id;
        var expectedName = expectedTheme.Name;

        using (var session = _store.LightweightSession())
        {
            session.Insert(themes);
            await session.SaveChangesAsync();
        }
        var stopwatch = Stopwatch.StartNew();

        //Act
        var response = await _httpClient.GetAsync($"/themes/{expectedTheme.Id}");
        stopwatch.Stop();

        var responseContent = await JsonSerializer.DeserializeAsync<GetThemeByIdResponse>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);

        //Arrange
        _factory.AssertResponseWithContentAsync(stopwatch, response, expectedStatusCode, responseContent);
        Assert.Equal(expectedId, responseContent.Theme.Id);
        Assert.Equal(expectedName, responseContent.Theme.Name);
        Assert.IsType<GetThemeByIdResponse>(responseContent);

        await _store.Advanced.Clean.DeleteAllDocumentsAsync(); //Clean up after test

    }

    [Fact]
    public async Task GetThemeByIdEndpoint_MapGet_WhenCalled_With_Invalid_Id_Should_Return_Invalid_Operation_Exception_Result()
    {
        //Arrange
        var themes = ThemeFakeData.GenerateThemes(_faker.Random.Int(1, 10)).ToArray();

        var id = Guid.NewGuid();

        using (var session = _store.LightweightSession())
        {
            session.Insert(themes);
            await session.SaveChangesAsync();
        }

        //Act/Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _httpClient.GetAsync($"/themes/{id}"));

        await _store.Advanced.Clean.DeleteAllDocumentsAsync(); //Clean up after test
    }

    [Fact]
    public async Task GetThemeByIdEndpoint_MapGet_When_Postgres_Is_Down_Should_Return_Invalid_Operation_Exception_Result()
    {
        //Arrange
        await _factory.PostgresContainer.StopAsync(); //Simulate a down database
        var id = Guid.NewGuid();


        //Act/Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _httpClient.GetAsync($"/themes/{id}"));

        await _factory.PostgresContainer.StartAsync(); //restart the database, initianisation only happens once when test file is built
    }
}
