using Discovery.Time.Tests.Data.MockData;

namespace Theme.API.Tests.Themes.GetThemesByDate;

public class GetThemesByDateHandlerTests : IClassFixture<MartenTestWebApplicationFactory<Program>>
{
    private readonly IMockNSubstituteMethods _mockingFramework;
    private readonly MartenTestWebApplicationFactory<Program> _factory;
    private readonly IDocumentStore _store;
    private readonly Faker _faker;

    public GetThemesByDateHandlerTests(MartenTestWebApplicationFactory<Program> factory)
    {
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
        _factory = factory;
        _store = _factory.Services.GetRequiredService<IDocumentStore>();
        _faker = new Faker();
    }

    #region GetThemesByDateHandler Tests

    [Fact]
    public async Task GetThemesByDateHandler_Handle_WhenCalled_Should_Return_Expected_Result()
    {
        //Arrange
        var themeCreate = _faker.Random.Int(1, 20);
        var themes = ThemeFakeData.GenerateThemes(themeCreate).ToArray();
        var dateToSearch = _faker.Date.Future().Date;
        var expected = themes.Where(x => x.StartDate.ToShortDateString() == dateToSearch.ToShortDateString() || x.EndDate.ToShortDateString() == dateToSearch.ToShortDateString()).Count();
        var query = new GetThemesByDateQuery
        (
            Date: dateToSearch
        );

        using (var session = _store.LightweightSession())
        {
            session.Insert(themes);
            await session.SaveChangesAsync();
        }

        var handler = new GetThemesByDateHandler(_store.LightweightSession());

        //Act
        var result = await handler.Handle(query, new CancellationToken());
        var actual = result.Themes.Count();

        //Assert
        Assert.Equal(expected, actual);
        Assert.IsType<GetThemesByDateResult>(result);

        await _store.Advanced.Clean.DeleteAllDocumentsAsync(); //Clean up after test

    }

    [Fact]
    public async Task GetThemesByDateHandler_Handle_WhenCalled_Should_Return_Empty_Result()
    {
        //Arrange
        var expected = 0;
        var dateToSearch = _faker.Date.Future().Date;

        var query = new GetThemesByDateQuery
        (
            Date: dateToSearch
        );

        var handler = new GetThemesByDateHandler(_store.LightweightSession());

        //Act
        var result = await handler.Handle(query, new CancellationToken());
        var actual = result.Themes.Count();

        //Assert
        Assert.Equal(expected, actual);
        Assert.IsType<GetThemesByDateResult>(result);
    }

    [Fact]
    public async Task GetThemesByDateHandler_Handle_WhenCalled_Should_Return_Exception_Result()
    {
        //Arrange
        var dateToSearch = _faker.Date.Future().Date;

        var query = new GetThemesByDateQuery
        (
            Date: dateToSearch
        );
        var expected = "Foo";
        var mockDocumentSession = _mockingFramework.InitializeMockedClass<IDocumentSession>(new object[] { });

        _mockingFramework.SetupThrowsException(mockDocumentSession, x => x.Query<Models.Theme>(), new Exception(expected));

        var handler = new GetThemesByDateHandler(mockDocumentSession);

        //Act/Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, new CancellationToken()));
    }

    #endregion
}
