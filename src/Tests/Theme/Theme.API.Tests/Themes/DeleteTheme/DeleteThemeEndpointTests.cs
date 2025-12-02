namespace Theme.API.Tests.Themes.DeleteTheme;

public class DeleteThemeEndpointTests : IClassFixture<MartenTestWebApplicationFactory<Program>>
{
    public readonly HttpClient _httpClient;
    public readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly RandomGenerator randomGenerator;
    private readonly MartenTestWebApplicationFactory<Program> _factory;
    private readonly IDocumentStore _store;

    public DeleteThemeEndpointTests(MartenTestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
        _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
        randomGenerator = new RandomGenerator();
        _store = _factory.Services.GetRequiredService<IDocumentStore>();
    }

    [Fact]
    public async Task DeleteThemeEndpoint_MapDelete_WhenCalled_Should_Return_OK_Result()
    {
        //Arrange
        var expectedStatusCode = System.Net.HttpStatusCode.OK;
        var createTheme = new Models.Theme
        {
            Id = Guid.NewGuid(),
            Name = randomGenerator.RandomString(20),
            Number = 1,
            Letter = randomGenerator.RandomString(2),
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            CreatedBy = "Tester",
            ModifiedBy = "Tester"
        };

        using (var session = _store.LightweightSession())
        {
            session.Store(createTheme);
            await session.SaveChangesAsync();
        }

        var request = createTheme.Id;
        var stopwatch = Stopwatch.StartNew();

        //Act
        var response = await _httpClient.DeleteAsync($"/themes/{request}");
        stopwatch.Stop();

        var responseContent = await JsonSerializer.DeserializeAsync<DeleteThemeResponse>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);
        //Assert
        _factory.AssertResponseWithContentAsync(stopwatch, response, expectedStatusCode, responseContent);
        Assert.True(responseContent.IsSuccess);
    }

    [Fact]
    public async Task DeleteThemeEndpoint_MapDelete_WhenCalled_Should_Return_Bad_Request_Result()
    {
        //Arrange
        var expectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        int request = 0;
        var stopwatch = Stopwatch.StartNew();

        //Act
        var response = await _httpClient.DeleteAsync($"/themes/{request}");
        stopwatch.Stop();

        //Assert
        _factory.AssertCommonResponseParts(stopwatch, response, expectedStatusCode);
    }

    [Fact]
    public async Task DeleteThemeEndpoint_MapDelete_WhenCalled_With_Validation_Error_Should_Return_Invalid_Operation_Exception_Result()
    {
        //Arrange
        Guid request = Guid.Empty;


        //Act/Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _httpClient.DeleteAsync($"/themes/{request}"));
    }

    [Fact]
    public async Task DeleteThemeEndpoint_MapDelete_When_Postgres_Is_Down_Should_Return_Invalid_Operation_Exception_Result()
    {
        //Arrange
        await _factory.PostgresContainer.StopAsync(); //Simulate a down database
        var request = Guid.NewGuid();
        //Act/Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _httpClient.DeleteAsync($"/themes/{request}"));

        await _factory.PostgresContainer.StartAsync(); //restart the database, initianisation only happens once when test file is built
    }
}
