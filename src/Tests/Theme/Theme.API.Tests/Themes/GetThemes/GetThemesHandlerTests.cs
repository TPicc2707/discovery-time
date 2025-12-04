namespace Theme.API.Tests.Themes.GetThemes;

public class GetThemesHandlerTests : IClassFixture<MartenTestWebApplicationFactory<Program>>
{
    private readonly IMockNSubstituteMethods _mockingFramework;
    private readonly MartenTestWebApplicationFactory<Program> _factory;
    private readonly IDocumentStore _store;
    private readonly Faker _faker;
    public GetThemesHandlerTests(MartenTestWebApplicationFactory<Program> factory)
    {
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
        _factory = factory;
        _store = _factory.Services.GetRequiredService<IDocumentStore>();
        _faker = new Faker();
    }

    #region GetThemesHandler Tests

    [Fact]
    public async Task GetThemesHandler_Handle_WhenCalled_Should_Return_Expected_Result()
    {
        //Arrange
        var expected = _faker.Random.Int(1, 20);
        var themes = ThemeFakeData.GenerateThemes(expected).ToArray();

        var query = new GetThemesQuery
        (
            PageNumber: 1,
            PageSize: expected
        );

        using (var session = _store.LightweightSession())
        {
            session.Insert(themes);
            await session.SaveChangesAsync();
        }

        var handler = new GetThemesHandler(_store.LightweightSession());

        //Act
        var result = await handler.Handle(query, new CancellationToken());
        var actual = result.Themes.Count();

        //Assert
        Assert.Equal(expected, actual);
        Assert.IsType<GetThemesResult>(result);

        await _store.Advanced.Clean.DeleteAllDocumentsAsync(); //Clean up after test
    }

    [Fact]
    public async Task GetThemesHandler_Handle_WhenCalled_Should_Return_Empty_Result()
    {
        //Arrange
        var expected = 0;

        var query = new GetThemesQuery
        (
            PageNumber: 1,
            PageSize: 10
        );

        var handler = new GetThemesHandler(_store.LightweightSession());

        //Act
        var result = await handler.Handle(query, new CancellationToken());
        var actual = result.Themes.Count();

        //Assert
        Assert.Equal(expected, actual);
        Assert.IsType<GetThemesResult>(result);
    }

    [Fact]
    public async Task GetThemesHandler_Handle_WhenCalled_Should_Return_Exception_Result()
    {
        //Arrange
        var query = new GetThemesQuery
        (
            PageNumber: 1,
            PageSize: 10
        );
        var expected = "Foo";
        var mockDocumentSession = _mockingFramework.InitializeMockedClass<IDocumentSession>(new object[] { });

        _mockingFramework.SetupThrowsException(mockDocumentSession, x => x.Query<Models.Theme>(), new Exception(expected));

        var handler = new GetThemesHandler(mockDocumentSession);

        //Act/Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, new CancellationToken()));
    }

    #endregion
}