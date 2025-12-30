namespace Activity.Application.Tests.Activities.Queries.GetActivitiesByThemeId;

public class GetActivitiesByThemeIdHandlerTests
{
    private readonly IMockNSubstituteMethods _mockingFramework;
    private readonly Faker _faker;

    public GetActivitiesByThemeIdHandlerTests()
    {
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
        _faker = new Faker();
    }

    #region GetActivitiesByThemeIdHandler

    [Fact]
    public async Task GetActivitiesByThemeIdHandler_Handle_WhenCalled_Should_Return_Expected_Result()
    {
        //Arrange
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });
        var themes = ThemeFakeData.GenerateActivityThemes(_faker.Random.Int(10, 20));
        var themeIds = themes.Select(x => x.Id.Value).ToList();
        var theme = themes.First();
        var query = new GetActivitiesByThemeIdQuery(theme.Id.Value);
        var activities = ThemeFakeData.GenerateActivities(themeIds, _faker).ToList();
        var themeActivites = activities.Where(x => x.ThemeId == theme.Id).ToList();
        var expected = themeActivites.Count;

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetByIdAsync(_mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>() }, theme);
        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetAllThemeActivitiesByThemeIdAsync(_mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>() }, themeActivites);

        var handler = new GetActivitiesByThemeIdHandler(mockUnitOfWork);

        //Act
        var result = await handler.Handle(query, new CancellationToken());
        var actual = result.Activities.Count();

        //Assert
        Assert.Equal(expected, actual);
        Assert.IsType<GetActivitiesByThemeIdResult>(result);
    }

    [Fact]
    public async Task GetActivitiesByThemeIdHandler_Handle_WhenCalled_Should_Return_ThemeNotFoundExpected_Result()
    {
        //Arrange
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });
        var themes = ThemeFakeData.GenerateActivityThemes(_faker.Random.Int(10, 20));
        var themeIds = themes.Select(x => x.Id.Value).ToList();
        var theme = themes.First();
        var query = new GetActivitiesByThemeIdQuery(theme.Id.Value);
        var activities = ThemeFakeData.GenerateActivities(themeIds, _faker).ToList();
        var themeActivites = activities.Where(x => x.ThemeId == theme.Id).ToList();
        Domain.Models.Theme themeNull = null;

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetByIdAsync(_mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>() }, themeNull);

        var handler = new GetActivitiesByThemeIdHandler(mockUnitOfWork);

        //Act/Assert
        await Assert.ThrowsAsync<ThemeNotFoundException>(() => handler.Handle(query, new CancellationToken()));

    }

    [Fact]
    public async Task GetActivitiesByThemeIdHandler_Handle_WhenCalled_Should_Return_Exception_Result()
    {
        //Arrange
        var expected = "Foo";
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });
        var themes = ThemeFakeData.GenerateActivityThemes(_faker.Random.Int(10, 20));
        var themeIds = themes.Select(x => x.Id.Value).ToList();
        var theme = themes.First();
        var query = new GetActivitiesByThemeIdQuery(theme.Id.Value);
        var activities = ThemeFakeData.GenerateActivities(themeIds, _faker).ToList();
        var themeActivites = activities.Where(x => x.ThemeId == theme.Id).ToList();

        var handler = new GetActivitiesByThemeIdHandler(mockUnitOfWork);

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetByIdAsync(_mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>() }, theme);
        _mockingFramework.SetupThrowsException(mockUnitOfWork.Theme, x => x.GetAllThemeActivitiesByThemeIdAsync(_mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>() }, new Exception(expected));

        //Act
        Func<Task> act = () => handler.Handle(query, new CancellationToken());

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);



    }

    #endregion

}