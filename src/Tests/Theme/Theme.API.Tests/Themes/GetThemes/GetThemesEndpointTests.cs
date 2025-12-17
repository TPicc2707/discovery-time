using Discovery.Time.Tests.Data.MockData;

namespace Theme.API.Tests.Themes.GetThemes;

public class GetThemesEndpointTests : IClassFixture<MartenTestWebApplicationFactory<Program>>
{
    public readonly HttpClient _httpClient;
    public readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly Faker _faker;
    private readonly MartenTestWebApplicationFactory<Program> _factory;
    private readonly IDocumentStore _store;

    public GetThemesEndpointTests(MartenTestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
        _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
        _faker = new Faker();
        _store = _factory.Services.GetRequiredService<IDocumentStore>();
    }

    [Fact]
    public async Task GetThemesEndpoint_MapGet_WhenCalled_Should_Return_OK_Result()
    {
        //Arrange
        var expectedStatusCode = System.Net.HttpStatusCode.OK;
        var expected = _faker.Random.Int(1, 20);
        var themes = ThemeFakeData.GenerateThemes(expected).ToArray();

        var request = new GetThemesRequest
        (
            PageNumber: 1,
            PageSize: expected
        );

        using (var session = _store.LightweightSession())
        {
            session.Insert(themes);
            await session.SaveChangesAsync();
        }
        var stopwatch = Stopwatch.StartNew();

        //Act
        var response = await _httpClient.GetAsync($"/themes?pageNumber={request.PageNumber}&pageSize={request.PageSize}");
        stopwatch.Stop();

        var responseContent = await JsonSerializer.DeserializeAsync<GetThemesResponse>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);

        //Assert
        _factory.AssertResponseWithContentAsync(stopwatch, response, expectedStatusCode, responseContent);
        Assert.Equal(expected, responseContent.Themes.Count());
        Assert.IsType<GetThemesResponse>(responseContent);

        await _store.Advanced.Clean.DeleteAllDocumentsAsync(); //Clean up after test
    }

    [Fact]
    public async Task GetThemesEndpoint_MapGet_WhenCalled_Should_Return_OK_Empty_Result()
    {
        //Arrange
        var expectedStatusCode = System.Net.HttpStatusCode.OK;
        var expected = 0;

        var request = new GetThemesRequest
        (
            PageNumber: 1,
            PageSize: 10
        );

        var stopwatch = Stopwatch.StartNew();

        //Act
        var response = await _httpClient.GetAsync($"/themes?pageNumber={request.PageNumber}&pageSize={request.PageSize}");
        stopwatch.Stop();

        var responseContent = await JsonSerializer.DeserializeAsync<GetThemesResponse>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);

        //Assert
        _factory.AssertResponseWithContentAsync(stopwatch, response, expectedStatusCode, responseContent);
        Assert.Equal(expected, responseContent.Themes.Count());
        Assert.IsType<GetThemesResponse>(responseContent);

    }

    [Fact]
    public async Task GetThemesEndpoint_MapGet_When_Postgres_Is_Down_Should_Return_Invalid_Operation_Exception_Result()
    {
        //Arrange
        await _factory.PostgresContainer.StopAsync(); //Simulate a down database
        var request = new GetThemesRequest
        (
            PageNumber: 1,
            PageSize: 10
        );

        //Act/Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _httpClient.GetAsync($"/themes?pageNumber={request.PageNumber}&pageSize={request.PageSize}"));

        await _factory.PostgresContainer.StartAsync(); //restart the database, initianisation only happens once when test file is built
    }

}
