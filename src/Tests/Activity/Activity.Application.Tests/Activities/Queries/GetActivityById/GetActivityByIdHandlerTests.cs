namespace Activity.Application.Tests.Activities.Queries.GetActivityById;

public class GetActivityByIdHandlerTests
{
    private readonly IMockNSubstituteMethods _mockingFramework;
    private readonly Faker _faker;

    public GetActivityByIdHandlerTests()
    {
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
        _faker = new Faker();

    }

    #region GetActivityByIdHandler

    [Fact]
    public async Task GetActivityByIdHandler_Handle_WhenCalled_Should_Return_Expected_Result()
    {
        //Arrange
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });
        var themes = ThemeFakeData.GenerateActivityThemes(_faker.Random.Int(10, 20)).Select(x => x.Id.Value).ToList();
        var activities = ThemeFakeData.GenerateActivities(themes, _faker).ToList();
        var activity = activities.First();
        var query = new GetActivityByIdQuery(activity.Id.Value);
        var expectedId = activity.Id.Value;
        var expectedName = activity.Name.Value;

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetThemeActivityByIdAsync(_mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>() }, activity);

        var handler = new GetActivityByIdHandler(mockUnitOfWork);

        //Act
        var result = await handler.Handle(query, new CancellationToken());
        var actualId = result.Activity.Id;
        var actualName = result.Activity.Name;

        //Assert
        Assert.Equal(expectedId, actualId);
        Assert.Equal(expectedName, actualName);
        Assert.IsType<GetActivityByIdResult>(result);
    }

    [Fact]
    public async Task GetActivityByIdHandler_Handle_WhenCalled_Should_Return_ActivityNotFoundExpected_Result()
    {
        //Arrange
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });
        var themes = ThemeFakeData.GenerateActivityThemes(_faker.Random.Int(10, 20)).Select(x => x.Id.Value).ToList();
        var activities = ThemeFakeData.GenerateActivities(themes, _faker).ToList();
        var activityQuery = activities.First();
        var query = new GetActivityByIdQuery(activityQuery.Id.Value);
        Domain.Models.Activity activity = null;

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetThemeActivityByIdAsync(_mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>() }, activity);

        var handler = new GetActivityByIdHandler(mockUnitOfWork);

        //Act/Assert
        await Assert.ThrowsAsync<ActivityNotFoundException>(() => handler.Handle(query, new CancellationToken()));

    }

    [Fact]
    public async Task GetActivityByIdHandler_Handle_WhenCalled_Should_Return_Exception_Result()
    {
        //Arrange
        var expected = "foo";
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });
        var themes = ThemeFakeData.GenerateActivityThemes(_faker.Random.Int(10, 20)).Select(x => x.Id.Value).ToList();
        var activities = ThemeFakeData.GenerateActivities(themes, _faker).ToList();
        var activity = activities.First();
        var query = new GetActivityByIdQuery(activity.Id.Value);

        var handler = new GetActivityByIdHandler(mockUnitOfWork);

        _mockingFramework.SetupThrowsException(mockUnitOfWork.Theme, x => x.GetThemeActivityByIdAsync(_mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>()), new Exception(expected));

        //Act
        Func<Task> act = () => handler.Handle(query, new CancellationToken());

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);
    }

    #endregion
}
