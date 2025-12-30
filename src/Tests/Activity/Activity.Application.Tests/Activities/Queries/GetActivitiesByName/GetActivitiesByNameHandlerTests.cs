namespace Activity.Application.Tests.Activities.Queries.GetActivitiesByName;

public class GetActivitiesByNameHandlerTests
{
    private readonly IMockNSubstituteMethods _mockingFramework;
    private readonly Faker _faker;

    public GetActivitiesByNameHandlerTests()
    {
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
        _faker = new Faker();
    }

    #region GetActivitiesByNameHandler

    [Fact]
    public async Task GetActivitiesByNameHandler_Handle_WhenCalled_Should_Return_Expected_Result()
    {
        // Arrange
        var nameToSearch = _faker.Random.String(1).ToLower();
        var query = new GetActivitiesByNameQuery(nameToSearch);
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });
        var themes = ThemeFakeData.GenerateActivityThemes(_faker.Random.Int(10, 20)).Select(x => x.Id.Value).ToList();
        var activities = ThemeFakeData.GenerateActivities(themes, _faker).ToList();
        var expected = activities.Count;

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetAllThemeActivitiesByNameAsync(_mockingFramework.GetObject<ActivityName>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ActivityName>(), _mockingFramework.GetObject<CancellationToken>() }, activities);

        var handler = new GetActivitiesByNameHandler(mockUnitOfWork);

        //Act
        var result = await handler.Handle(query, CancellationToken.None);
        var actual = result.Activities.Count();

        //Assert
        Assert.Equal(expected, actual);
        Assert.IsType<GetActivitiesByNameResult>(result);
    }

    [Fact]
    public async Task GetActivitiesByNameHandler_Handle_WhenCalled_Should_Return_Exception_Result()
    {
        //Arrange
        var expected = "foo";
        var nameToSearch = _faker.Random.String(1).ToLower();
        var query = new GetActivitiesByNameQuery(nameToSearch);
        int totalCount = _faker.Random.Int(10, 20);
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });
        var themes = ThemeFakeData.GenerateActivityThemes(_faker.Random.Int(10, 20)).Select(x => x.Id.Value).ToList();
        var activities = ThemeFakeData.GenerateActivities(themes, _faker).ToList();

        var handler = new GetActivitiesByNameHandler(mockUnitOfWork);

        _mockingFramework.SetupThrowsException(mockUnitOfWork.Theme, x => x.GetAllThemeActivitiesByNameAsync(_mockingFramework.GetObject<ActivityName>(), _mockingFramework.GetObject<CancellationToken>()), new Exception(expected));

        //Act
        Func<Task> act = () => handler.Handle(query, new CancellationToken());

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);
    }

    #endregion
}
