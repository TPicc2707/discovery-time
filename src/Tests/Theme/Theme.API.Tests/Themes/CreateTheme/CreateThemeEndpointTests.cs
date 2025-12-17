namespace Theme.API.Tests.Themes.CreateTheme;

public class CreateThemeEndpointTests : IClassFixture<MartenTestWebApplicationFactory<Program>>
{
    public readonly HttpClient _httpClient;
    public readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly Faker _faker;
    private readonly MartenTestWebApplicationFactory<Program> _factory;

    public CreateThemeEndpointTests(MartenTestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
        _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
        _faker = new Faker();
    }

    [Fact]
    public async Task CreateThemeEndpoint_MapPost_WhenCalled_Should_Return_Created_Result()
    {
        //Arrange
        var expectedStatusCode = System.Net.HttpStatusCode.Created;
        var request = new CreateThemeRequest(_faker.Random.String(20), 1, _faker.Random.String(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester", "Tester");
        var stopwatch = Stopwatch.StartNew();

        //Act
        var response = await _httpClient.PostAsync("/themes", _factory.GetJsonStringContent(request));
        stopwatch.Stop();

        var responseContent = await JsonSerializer.DeserializeAsync<CreateThemeResponse>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);
        //Assert
        _factory.AssertResponseWithContentAsync(stopwatch, response, expectedStatusCode, responseContent);
        Assert.IsType<Guid>(responseContent.Id);
    }

    [Fact]
    public async Task CreateThemeEndpoint_MapPost_WhenCalled_Should_Return_Bad_Request_Result()
    {
        //Arrange
        var expectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        CreateThemeRequest request = null;
        var stopwatch = Stopwatch.StartNew();

        //Act
        var response = await _httpClient.PostAsync("/themes", _factory.GetJsonStringContent(request));
        stopwatch.Stop();

        //Assert
        _factory.AssertCommonResponseParts(stopwatch, response, expectedStatusCode);
    }

    [Fact]
    public async Task CreateThemeEndpoint_MapPost_WhenCalled_With_Validation_Error_Should_Return_Invalid_Operation_Exception_Result()
    {
        //Arrange
        var request = new CreateThemeRequest(_faker.Random.String(20), 1, _faker.Random.String(3), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester", "Tester"); //bad data


        //Act/Assert
        var exception = await Record.ExceptionAsync(() => _httpClient.PostAsync("/themes", _factory.GetJsonStringContent(request)));
        
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async Task CreateThemeEndpoint_MapPost_When_Postgres_Is_Down_Should_Return_Invalid_Operation_Exception_Result()
    {
        //Arrange
        await _factory.PostgresContainer.StopAsync(); //Simulate a down database
        var request = new CreateThemeRequest(_faker.Random.String(20), 1, _faker.Random.String(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester", "Tester");

        //Act/Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _httpClient.PostAsync("/themes", _factory.GetJsonStringContent(request)));

        await _factory.PostgresContainer.StartAsync(); //restart the database, initianisation only happens once when test file is built
    }
}
