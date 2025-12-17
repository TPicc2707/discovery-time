using Discovery.Time.Tests.Data.MockData;

namespace Theme.API.Tests.Themes.GetThemesByDate;

public class GetThemesByDateEndpointTest : IClassFixture<MartenTestWebApplicationFactory<Program>>
{
    public readonly HttpClient _httpClient;
    public readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly Faker _faker;
    private readonly MartenTestWebApplicationFactory<Program> _factory;
    private readonly IDocumentStore _store;

    public GetThemesByDateEndpointTest(MartenTestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
        _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
        _faker = new Faker();
        _store = _factory.Services.GetRequiredService<IDocumentStore>();
    }

    [Fact]
    public async Task GetThemesByDateEndpoint_MapGet_WhenCalled_Should_Return_OK_Result()
    {
        //Arrange
        var expectedStatusCode = System.Net.HttpStatusCode.OK;
        var amount = _faker.Random.Int(1, 20);
        var themes = ThemeFakeData.GenerateThemes(amount).ToArray();
        var dateSearch = _faker.Date.Future();
        var expected = themes.Where(x => x.StartDate.ToShortDateString() == dateSearch.ToShortDateString() || x.EndDate.ToShortDateString() == dateSearch.ToShortDateString()).Count();

        using (var session = _store.LightweightSession())
        {
            session.Insert(themes);
            await session.SaveChangesAsync();
        }
        var stopwatch = Stopwatch.StartNew();

        //Act
        var response = await _httpClient.GetAsync($"/themes/date/{dateSearch.ToString("yyyy-MM-dd")}");
        stopwatch.Stop();

        var responseContent = await JsonSerializer.DeserializeAsync<GetThemesByDateResponse>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);

        //Assert
        _factory.AssertResponseWithContentAsync(stopwatch, response, expectedStatusCode, responseContent);
        Assert.Equal(expected, responseContent.Themes.Count());
        Assert.IsType<GetThemesByDateResponse>(responseContent);

        await _store.Advanced.Clean.DeleteAllDocumentsAsync(); //Clean up after test
    }

    [Fact]
    public async Task GetThemesByDateEndpoint_MapGet_WhenCalled_Should_Return_OK_Empty_Result()
    {
        //Arrange
        var expectedStatusCode = System.Net.HttpStatusCode.OK;
        var dateSearch = _faker.Date.Future();
        var expected = 0;

        var stopwatch = Stopwatch.StartNew();

        //Act
        var response = await _httpClient.GetAsync($"/themes/date/{dateSearch.ToString("yyyy-MM-dd")}");
        stopwatch.Stop();

        var responseContent = await JsonSerializer.DeserializeAsync<GetThemesByDateResponse>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);

        //Assert
        _factory.AssertResponseWithContentAsync(stopwatch, response, expectedStatusCode, responseContent);
        Assert.Equal(expected, responseContent.Themes.Count());
        Assert.IsType<GetThemesByDateResponse>(responseContent);

    }

    [Fact]
    public async Task GetThemesByNameEndpoint_MapGet_When_Postgres_Is_Down_Should_Return_Invalid_Operation_Exception_Result()
    {
        //Arrange
        await _factory.PostgresContainer.StopAsync(); //Simulate a down database
        var dateSearch = _faker.Date.Future();


        //Act/Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _httpClient.GetAsync($"/themes/date/{dateSearch.ToString("yyyy-MM-dd")}"));

        await _factory.PostgresContainer.StartAsync(); //restart the database, initianisation only happens once when test file is built
    }
}
