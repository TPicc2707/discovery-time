namespace Activity.Application.Tests.Activities.Commands.DeleteActivity;

public class DeleteActivityHandlerTest
{
    private readonly IMockNSubstituteMethods _mockingFramework;
    private readonly DeleteActivityCommandValidator _deleteActivityCommandValidator;
    private readonly Faker _faker;

    public DeleteActivityHandlerTest()
    {
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
        _deleteActivityCommandValidator = new DeleteActivityCommandValidator();
        _faker = new Faker();

    }

    #region DeleteActivityHandler

    [Fact]
    public async Task DeleteActivityHandler_Handle_WhenCalled_Should_Return_Expected_Result()
    {
        //Arrange
        var id = Guid.NewGuid();
        var command = new DeleteActivityCommand(id);
        var activityDetails = ActivityDetails.Of(_faker.Lorem.Sentence(), _faker.Internet.Url(), _faker.Date.Future());
        var activity = Domain.Models.Activity.Create(id: ActivityId.Of(id), themeId: ThemeId.Of(Guid.NewGuid()), ActivityName.Of(_faker.Lorem.Word()), activityDetails);

        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });

        var handler = new DeleteActivityHandler(mockUnitOfWork);

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetThemeActivityByIdAsync(_mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>() }, activity);
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork.Theme, x => x.DeleteThemeActivity(_mockingFramework.GetObject<Domain.Models.Activity>()), new object[] { _mockingFramework.GetObject<Domain.Models.Activity>() });
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        //Act
        var result = await handler.Handle(command, new CancellationToken());
        var actual = result.IsSuccess;

        //Assert
        _mockingFramework.VerifyMethodRun(mockUnitOfWork.Theme, c => c.DeleteThemeActivity(_mockingFramework.GetObject<Domain.Models.Activity>()), 1);
        _mockingFramework.VerifyMethodRun(mockUnitOfWork, c => c.Complete(_mockingFramework.GetObject<CancellationToken>()), 1);
        Assert.IsType<DeleteActivityResult>(result);
        Assert.True(actual);

    }

    [Fact]
    public async Task DeleteActivityHandler_Handle_WhenCalled_Should_Return_Theme_NotFound_Exception_Result()
    {
        //Arrange
        var expected = "foo";
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), DateTime.UtcNow);
        var activityDto = new ActivityDto(Guid.NewGuid(), Guid.NewGuid(), _faker.Lorem.Word(), activityDetailsDto);
        var id = Guid.NewGuid();
        var command = new DeleteActivityCommand(id);
        Domain.Models.Activity activity = null;
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });

        var handler = new DeleteActivityHandler(mockUnitOfWork);

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetThemeActivityByIdAsync(_mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>() }, activity);
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork.Theme, x => x.DeleteThemeActivity(_mockingFramework.GetObject<Domain.Models.Activity>()), new object[] { _mockingFramework.GetObject<Domain.Models.Activity>() });
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });


        //Act/Assert
        await Assert.ThrowsAsync<ActivityNotFoundException>(() => handler.Handle(command, new CancellationToken()));
    }

    [Fact]
    public async Task DeleteActivityHandler_Handle_WhenCalled_Should_Return_Exception_Result()
    {
        //Arrange
        var expected = "foo";
        var id = Guid.NewGuid();
        var command = new DeleteActivityCommand(id);
        var activityDetails = ActivityDetails.Of(_faker.Lorem.Sentence(), _faker.Internet.Url(), _faker.Date.Future());
        var activity = Domain.Models.Activity.Create(id: ActivityId.Of(id), themeId: ThemeId.Of(Guid.NewGuid()), ActivityName.Of(_faker.Lorem.Word()), activityDetails);
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });

        var handler = new DeleteActivityHandler(mockUnitOfWork);

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetThemeActivityByIdAsync(_mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>() }, activity);
        _mockingFramework.SetupThrowsException(mockUnitOfWork.Theme, x => x.DeleteThemeActivity(_mockingFramework.GetObject<Domain.Models.Activity>()), new Exception(expected));
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        //Act
        Func<Task> act = () => handler.Handle(command, new CancellationToken());

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);
    }

    #endregion

    #region DeleteActivityCommandValidator

    [Fact]
    public async Task DeleteActivityCommandValidator_ValidateActivity_When_DeleteActivityCommand_Object_Called_Should_Return_Expected_Result()
    {
        //Arrange
        var id = Guid.NewGuid();
        var command = new DeleteActivityCommand(id);

        //Act
        var result = await _deleteActivityCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task DeleteActivityCommandValidator_ValidateActivity_When_DeleteActivityCommand_Object_Called_Should_Return_All_Validation_Exceptions()
    {
        //Arrange
        var id = Guid.Empty;
        var command = new DeleteActivityCommand(id);

        //Act
        var result = await _deleteActivityCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    #endregion
}
