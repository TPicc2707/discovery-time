namespace Theme.API.Tests.Themes.GetThemesByName;

public class GetThemesByNameHandlerTests : IClassFixture<MartenTestWebApplicationFactory<Program>>
{
    private readonly IMockNSubstituteMethods _mockingFramework;
    private readonly MartenTestWebApplicationFactory<Program> _factory;
    private readonly IDocumentStore _store;
    private readonly Faker _faker;
    public GetThemesByNameHandlerTests(MartenTestWebApplicationFactory<Program> factory)
    {
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
        _factory = factory;
        _store = _factory.Services.GetRequiredService<IDocumentStore>();
        _faker = new Faker();
    }

    #region GetThemesByNameHandler Tests

    [Fact]
    public async Task GetThemesByNameHandler_Handle_WhenCalled_Should_Return_Expected_Result()
    {
        //Arrange
        var themeCreate = _faker.Random.Int(1, 20);
        var themes = ThemeFakeData.GenerateThemes(themeCreate).ToArray();
        var nameToSearch = _faker.Random.String(1).ToLower();
        var expected = themes.Where(x => x.Name.ToLower().Contains(nameToSearch)).Count();
        var query = new GetThemesByNameQuery
        (
            Name : nameToSearch
        );

        using (var session = _store.LightweightSession())
        {
            session.Insert(themes);
            await session.SaveChangesAsync();
        }

        var handler = new GetThemesByNameHandler(_store.LightweightSession());

        //Act
        var result = await handler.Handle(query, new CancellationToken());
        var actual = result.Themes.Count();

        //Assert
        Assert.Equal(expected, actual);
        Assert.IsType<GetThemesByNameResult>(result);

        await _store.Advanced.Clean.DeleteAllDocumentsAsync(); //Clean up after test
    }

    [Fact]
    public async Task GetThemesByNameHandler_Handle_WhenCalled_Should_Return_Empty_Result()
    {
        //Arrange
        var expected = 0;
        var nameToSearch = _faker.Random.String(1).ToLower();

        var query = new GetThemesByNameQuery
        (
            Name: nameToSearch
        );

        var handler = new GetThemesByNameHandler(_store.LightweightSession());

        //Act
        var result = await handler.Handle(query, new CancellationToken());
        var actual = result.Themes.Count();

        //Assert
        Assert.Equal(expected, actual);
        Assert.IsType<GetThemesByNameResult>(result);
    }

    [Fact]
    public async Task GetThemesByNameHandler_Handle_WhenCalled_Should_Return_Exception_Result()
    {
        //Arrange
        var nameToSearch = _faker.Random.String(1).ToLower();

        var query = new GetThemesByNameQuery
        (
            Name: nameToSearch
        );
        var expected = "Foo";
        var mockDocumentSession = _mockingFramework.InitializeMockedClass<IDocumentSession>(new object[] { });

        _mockingFramework.SetupThrowsException(mockDocumentSession, x => x.Query<Models.Theme>(), new Exception(expected));

        var handler = new GetThemesByNameHandler(mockDocumentSession);

        //Act/Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, new CancellationToken()));
    }

    #endregion
}
