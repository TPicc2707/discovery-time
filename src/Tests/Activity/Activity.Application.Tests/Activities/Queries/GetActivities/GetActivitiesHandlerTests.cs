namespace Activity.Application.Tests.Activities.Queries.GetActivities;

public class GetActivitiesHandlerTests
{
    private readonly IMockNSubstituteMethods _mockingFramework;
    private readonly Faker _faker;

    public GetActivitiesHandlerTests()
    {
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
        _faker = new Faker();
    }

    #region GetActivitiesHandler

    [Fact]
    public async Task GetActivitiesHandler_Handle_WhenCalled_Should_Return_Expected_Result()
    {
        //Arrange
        int pageIndex = 0;
        int pageSize = 10;
        var paginationRequest = new PaginationRequest(pageIndex, pageSize);
        var query = new GetActivitiesQuery(paginationRequest);
        int totalCount = _faker.Random.Int(10, 20);
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });
        var themes = ThemeFakeData.GenerateActivityThemes(_faker.Random.Int(10, 20)).Select(x => x.Id.Value).ToList();
        var activities = ThemeFakeData.GenerateActivities(themes, _faker).ToList();
        var expected = activities.Count;
        var expectedLongCount = totalCount;

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetThemeActivitiesLongCountAsync(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() }, totalCount);
        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetAllThemeActivitiesAsync(_mockingFramework.GetObject<int>(), _mockingFramework.GetObject<int>()), new object[] { _mockingFramework.GetObject<int>(), _mockingFramework.GetObject<int>() }, activities);

        var handler = new GetActivitiesHandler(mockUnitOfWork);

        //Act
        var result = await handler.Handle(query, new CancellationToken());
        var actual = result.Activities.Data.Count();
        var actualLongCount = result.Activities.Count;

        //Assert
        Assert.Equal(expected, actual);
        Assert.Equal(expectedLongCount, actualLongCount);
        Assert.IsType<GetActivitiesResult>(result);
    }

    [Fact]
    public async Task GetActivitiesHandler_Handle_WhenCalled_Should_Return_Exception_Result()
    {
        //Arrange
        var expected = "foo";
        int pageIndex = 0;
        int pageSize = 10;
        var paginationRequest = new PaginationRequest(pageIndex, pageSize);
        var query = new GetActivitiesQuery(paginationRequest);
        int totalCount = _faker.Random.Int(10, 20);
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });
        var themes = ThemeFakeData.GenerateActivityThemes(_faker.Random.Int(10, 20)).Select(x => x.Id.Value).ToList();
        var activities = ThemeFakeData.GenerateActivities(themes, _faker).ToList();

        var handler = new GetActivitiesHandler(mockUnitOfWork);

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetThemeActivitiesLongCountAsync(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() }, totalCount);
        _mockingFramework.SetupThrowsException(mockUnitOfWork.Theme, x => x.GetAllThemeActivitiesAsync(_mockingFramework.GetObject<int>(), _mockingFramework.GetObject<int>()), new Exception(expected));

        //Act
        Func<Task> act = () => handler.Handle(query, new CancellationToken());

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);
    }

    #endregion
}
