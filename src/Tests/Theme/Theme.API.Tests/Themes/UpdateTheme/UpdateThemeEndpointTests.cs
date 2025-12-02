namespace Theme.API.Tests.Themes.UpdateTheme;

public class UpdateThemeEndpointTests : IClassFixture<MartenTestWebApplicationFactory<Program>>
{
    public readonly HttpClient _httpClient;
    public readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly RandomGenerator randomGenerator;
    private readonly MartenTestWebApplicationFactory<Program> _factory;
    private readonly IDocumentStore _store;

    public UpdateThemeEndpointTests(MartenTestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
        _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
        randomGenerator = new RandomGenerator();
        _store = _factory.Services.GetRequiredService<IDocumentStore>();
    }

    [Fact]
    public async Task UpdateThemeEndpoint_MapPut_WhenCalled_Should_Return_OK_Result()
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

        using(var session = _store.LightweightSession())
        {
            session.Store(createTheme);
            await session.SaveChangesAsync();
        }

        var request = new UpdateThemeRequest(createTheme.Id, randomGenerator.RandomString(20), 1, randomGenerator.RandomString(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester");
        var stopwatch = Stopwatch.StartNew();

        //Act
        var response = await _httpClient.PutAsync("/themes", _factory.GetJsonStringContent(request));
        stopwatch.Stop();

        var responseContent = await JsonSerializer.DeserializeAsync<UpdateThemeResponse>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);
        //Assert
        _factory.AssertResponseWithContentAsync(stopwatch, response, expectedStatusCode, responseContent);
        Assert.True(responseContent.IsSuccess);
    }

    [Fact]
    public async Task UpdateThemeEndpoint_MapPut_WhenCalled_Should_Return_Not_Found_Result()
    {
        //Arrange
        var request = new UpdateThemeRequest(Guid.NewGuid(), randomGenerator.RandomString(20), 1, randomGenerator.RandomString(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester");

        //Act/Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _httpClient.PutAsync("/themes", _factory.GetJsonStringContent(request)));
    }

    [Fact]
    public async Task UpdateThemeEndpoint_MapPut_WhenCalled_Should_Return_Bad_Request_Result()
    {
        //Arrange
        var expectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        UpdateThemeRequest request = null;
        var stopwatch = Stopwatch.StartNew();

        //Act
        var response = await _httpClient.PutAsync("/themes", _factory.GetJsonStringContent(request));
        stopwatch.Stop();

        //Assert
        _factory.AssertCommonResponseParts(stopwatch, response, expectedStatusCode);
    }

    [Fact]
    public async Task UpdateThemeEndpoint_MapPut_WhenCalled_With_Validation_Error_Should_Return_Invalid_Operation_Exception_Result()
    {
        //Arrange
        var request = new UpdateThemeRequest(Guid.NewGuid(), randomGenerator.RandomString(20), 1, randomGenerator.RandomString(3), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester"); //bad data


        //Act/Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _httpClient.PutAsync("/themes", _factory.GetJsonStringContent(request)));
    }

    [Fact]
    public async Task UpdateThemeEndpoint_MapPut_When_Postgres_Is_Down_Should_Return_Invalid_Operation_Exception_Result()
    {
        //Arrange
        await _factory.PostgresContainer.StopAsync(); //Simulate a down database
        var request = new UpdateThemeRequest(Guid.NewGuid(), randomGenerator.RandomString(20), 1, randomGenerator.RandomString(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester"); //bad data

        //Act/Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _httpClient.PutAsync("/themes", _factory.GetJsonStringContent(request)));

        await _factory.PostgresContainer.StartAsync(); //restart the database, initianisation only happens once when test file is built
    }
}
