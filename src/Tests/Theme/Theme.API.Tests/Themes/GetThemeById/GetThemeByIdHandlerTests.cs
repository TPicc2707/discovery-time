namespace Theme.API.Tests.Themes.GetThemeById;

public class GetThemeByIdHandlerTests
{
    private readonly IMockNSubstituteMethods _mockingFramework;

    public GetThemeByIdHandlerTests()
    {
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
    }

    #region GetThemeByIdHandler Tests

    [Fact]
    public async Task GetThemeByIdHandler_Handle_WhenCalled_Should_Return_Expected_Result()
    {
        //Arrange
        var themes = ThemeFakeData.GenerateThemes(1);
        var expectedTheme = themes.First();
        var query = new GetThemeByIdQuery(expectedTheme.Id);
        var mockDocumentSession = _mockingFramework.InitializeMockedClass<IDocumentSession>(Array.Empty<object>());

        _mockingFramework.SetupReturnsResult(mockDocumentSession, x => x.LoadAsync<Models.Theme>(_mockingFramework.GetObject<Guid>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<Guid>(), _mockingFramework.GetObject<CancellationToken>() }, expectedTheme);
        
        var handler = new GetThemeByIdHandler(mockDocumentSession);

        //Act
        var result = await handler.Handle(query, new CancellationToken());
        var actualThemeId = result.Theme.Id;
        var actualThemeName = result.Theme.Name;

        //Assert
        Assert.Equal(expectedTheme.Id, actualThemeId);
        Assert.Equal(expectedTheme.Name, actualThemeName);
        Assert.IsType<GetThemeByIdResult>(result);
    }

    [Fact]
    public async Task GetThemeByIdHandler_Handle_WhenCalled_Should_Return_Theme_NotFound_Exception()
    {
        //Arrange
        var query = new GetThemeByIdQuery(Guid.NewGuid());
        Models.Theme theme = null;

        var mockDocumentSession = _mockingFramework.InitializeMockedClass<IDocumentSession>(new object[] { });
        _mockingFramework.SetupReturnsResult(mockDocumentSession, x => x.LoadAsync<Models.Theme>(_mockingFramework.GetObject<Guid>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<Guid>(), _mockingFramework.GetObject<CancellationToken>() }, theme);
        var handler = new GetThemeByIdHandler(mockDocumentSession);

        //Act/Assert
        await Assert.ThrowsAsync<ThemeNotFoundException>(() => handler.Handle(query, new CancellationToken()));
    }

    [Fact]
    public async Task GetThemeByIdHandler_Handle_WhenCalled_Should_Return_Exception_Result()
    {
        //Arrange
        var query = new GetThemeByIdQuery(Guid.NewGuid());
        var mockDocumentSession = _mockingFramework.InitializeMockedClass<IDocumentSession>(Array.Empty<object>());
        var expected = "Foo";
        _mockingFramework.SetupThrowsException(mockDocumentSession, x => x.LoadAsync<Models.Theme>(_mockingFramework.GetObject<Guid>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<Guid>(), _mockingFramework.GetObject<CancellationToken>() }, new Exception(expected));

        var handler = new GetThemeByIdHandler(mockDocumentSession);

        //Act/Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, new CancellationToken()));
    }

    #endregion
}
