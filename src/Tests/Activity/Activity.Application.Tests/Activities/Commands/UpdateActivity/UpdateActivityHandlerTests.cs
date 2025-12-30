namespace Activity.Application.Tests.Activities.Commands.UpdateActivity;

public class UpdateActivityHandlerTests
{
    private readonly IMockNSubstituteMethods _mockingFramework;
    private readonly UpdateActivityCommandValidator _updateActivityCommandValidator;
    private readonly Faker _faker;

    public UpdateActivityHandlerTests()
    {
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
        _updateActivityCommandValidator = new UpdateActivityCommandValidator();
        _faker = new Faker();
    }

    #region UpdateActivityHandler

    [Fact]
    public async Task UpdateActivityHandler_Handle_WhenCalled_Should_Return_Expected_Result()
    {
        //Arrange
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), DateTime.UtcNow);
        var activityDto = new ActivityDto(Guid.NewGuid(), Guid.NewGuid(), _faker.Lorem.Word(), activityDetailsDto);
        var command = new UpdateActivityCommand(activityDto);
        var activityDetails = ActivityDetails.Of(activityDetailsDto.Description, activityDetailsDto.Url, activityDetailsDto.Date);
        var activity = Domain.Models.Activity.Create(id: ActivityId.Of(Guid.NewGuid()), themeId: ThemeId.Of(activityDto.ThemeId), ActivityName.Of(activityDto.Name), activityDetails);
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });

        var handler = new UpdateActivityHandler(mockUnitOfWork);

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetThemeActivityByIdAsync(_mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>() }, activity);
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork.Theme, x => x.UpdateThemeActivity(_mockingFramework.GetObject<Domain.Models.Activity>()), new object[] { _mockingFramework.GetObject<Domain.Models.Activity>() });
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        var expected = true;

        //Act
        var result = await handler.Handle(command, new CancellationToken());
        var actual = result.IsSuccess;

        //Assert
        _mockingFramework.VerifyMethodRun(mockUnitOfWork.Theme, c => c.UpdateThemeActivity(_mockingFramework.GetObject<Domain.Models.Activity>()), 1);
        _mockingFramework.VerifyMethodRun(mockUnitOfWork, c => c.Complete(_mockingFramework.GetObject<CancellationToken>()), 1);
        Assert.IsType<UpdateActivityResult>(result);
        Assert.True(actual);

    }

    [Fact]
    public async Task UpdateActivityHandler_Handle_WhenCalled_Should_Return_Theme_NotFound_Exception_Result()
    {
        //Arrange
        var expected = "foo";
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), DateTime.UtcNow);
        var activityDto = new ActivityDto(Guid.NewGuid(), Guid.NewGuid(), _faker.Lorem.Word(), activityDetailsDto);
        var command = new UpdateActivityCommand(activityDto);
        Domain.Models.Activity activity = null;
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });

        var handler = new UpdateActivityHandler(mockUnitOfWork);

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetThemeActivityByIdAsync(_mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>() }, activity);
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork.Theme, x => x.UpdateThemeActivity(_mockingFramework.GetObject<Domain.Models.Activity>()), new object[] { _mockingFramework.GetObject<Domain.Models.Activity>() });
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });
       
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
        //Act/Assert
        await Assert.ThrowsAsync<ActivityNotFoundException>(() => handler.Handle(command, new CancellationToken()));
    }

    [Fact]
    public async Task UpdateActivityHandler_Handle_WhenCalled_Should_Return_InvalidOperationException_Result()
    {
        //Arrange
        var expected = "foo";
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), _faker.Date.Future());
        var activityDto = new ActivityDto(Guid.NewGuid(), _faker.Random.Guid(), _faker.Lorem.Word(), activityDetailsDto);
        var activityDetails = ActivityDetails.Of(activityDetailsDto.Description, activityDetailsDto.Url, activityDetailsDto.Date);
        var activity = Domain.Models.Activity.Create(id: ActivityId.Of(activityDto.Id), themeId: ThemeId.Of(activityDto.ThemeId), ActivityName.Of(activityDto.Name), activityDetails);
        var command = new UpdateActivityCommand(activityDto);
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });

        var handler = new UpdateActivityHandler(mockUnitOfWork);

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetThemeActivityByIdAsync(_mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>() }, activity);
        _mockingFramework.SetupThrowsException(mockUnitOfWork.Theme, x => x.UpdateThemeActivity(_mockingFramework.GetObject<Domain.Models.Activity>()), new InvalidOperationException(expected));
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        //Act
        Func<Task> act = () => handler.Handle(command, new CancellationToken());

        //Assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Contains(expected, ex.Message);
    }

    [Fact]
    public async Task UpdateActivityHandler_Handle_WhenCalled_Should_Return_NullReferenceException_Result()
    {
        //Arrange
        var expected = "foo";
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), _faker.Date.Future());
        var activityDto = new ActivityDto(Guid.NewGuid(), _faker.Random.Guid(), _faker.Lorem.Word(), activityDetailsDto);
        var activityDetails = ActivityDetails.Of(activityDetailsDto.Description, activityDetailsDto.Url, activityDetailsDto.Date);
        var activity = Domain.Models.Activity.Create(id: ActivityId.Of(activityDto.Id), themeId: ThemeId.Of(activityDto.ThemeId), ActivityName.Of(activityDto.Name), activityDetails);
        var command = new UpdateActivityCommand(activityDto);
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });

        var handler = new UpdateActivityHandler(mockUnitOfWork);

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetThemeActivityByIdAsync(_mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>() }, activity);
        _mockingFramework.SetupThrowsException(mockUnitOfWork.Theme, x => x.UpdateThemeActivity(_mockingFramework.GetObject<Domain.Models.Activity>()), new NullReferenceException(expected));
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        //Act
        Func<Task> act = () => handler.Handle(command, new CancellationToken());

        //Assert
        NullReferenceException ex = await Assert.ThrowsAsync<NullReferenceException>(act);
        Assert.Contains(expected, ex.Message);
    }


    [Fact]
    public async Task UpdateActivityHandler_Handle_WhenCalled_Should_Return_Exception_Result()
    {
        //Arrange
        var expected = "foo";
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), _faker.Date.Future());
        var activityDto = new ActivityDto(Guid.NewGuid(), _faker.Random.Guid(), _faker.Lorem.Word(), activityDetailsDto);
        var activityDetails = ActivityDetails.Of(activityDetailsDto.Description, activityDetailsDto.Url, activityDetailsDto.Date);
        var activity = Domain.Models.Activity.Create(id: ActivityId.Of(activityDto.Id), themeId: ThemeId.Of(activityDto.ThemeId), ActivityName.Of(activityDto.Name), activityDetails);
        var command = new UpdateActivityCommand(activityDto);
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });

        var handler = new UpdateActivityHandler(mockUnitOfWork);

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetThemeActivityByIdAsync(_mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>() }, activity);
        _mockingFramework.SetupThrowsException(mockUnitOfWork.Theme, x => x.UpdateThemeActivity(_mockingFramework.GetObject<Domain.Models.Activity>()), new Exception(expected));
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        //Act
        Func<Task> act = () => handler.Handle(command, new CancellationToken());

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);
    }


    #endregion

    #region UpdateActivityCommandValidator

    [Fact]
    public async Task UpdateActivityCommandValidator_ValidateActivity_When_UpdateActivityCommand_Object_Called_Should_Return_Expected_Result()
    {
        //Arrange
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), _faker.Date.Future());
        var activityDto = new ActivityDto(_faker.Random.Guid(), _faker.Random.Guid(), _faker.Lorem.Word(), activityDetailsDto);
        var command = new UpdateActivityCommand(activityDto);

        //Act
        var result = await _updateActivityCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.ThemeId);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Url);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Date);
    }

    [Fact]
    public async Task UpdateActivityCommandValidator_ValidateActivity_When_UpdateActivityCommand_Object_Called_Should_Return_All_Validation_Exceptions()
    {
        //Arrange
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), string.Empty, new DateTime());
        var activityDto = new ActivityDto(Guid.Empty, Guid.Empty, string.Empty, activityDetailsDto);
        var command = new UpdateActivityCommand(activityDto);

        //Act
        var result = await _updateActivityCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Activity.Id);
        result.ShouldHaveValidationErrorFor(x => x.Activity.Name);
        result.ShouldHaveValidationErrorFor(x => x.Activity.ThemeId);
        result.ShouldHaveValidationErrorFor(x => x.Activity.Details.Url);
        result.ShouldHaveValidationErrorFor(x => x.Activity.Details.Date);

    }

    [Fact]
    public async Task UpdateActivityCommandValidator_ValidateActivity_When_UpdateActivityCommand_Object_Called_Should_Return_Id_Empty_Validation_Exceptions()
    {
        //Arrange
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), _faker.Date.Future());
        var activityDto = new ActivityDto(Guid.Empty, _faker.Random.Guid(), _faker.Lorem.Word(), activityDetailsDto);
        var command = new UpdateActivityCommand(activityDto);

        //Act
        var result = await _updateActivityCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Activity.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.ThemeId);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Url);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Date);

    }

    [Fact]
    public async Task UpdateActivityCommandValidator_ValidateActivity_When_UpdateActivityCommand_Object_Called_Should_Return_Name_Empty_And_Length_Validation_Exceptions()
    {
        //Arrange
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), _faker.Date.Future());
        var activityDto = new ActivityDto(_faker.Random.Guid(), _faker.Random.Guid(), string.Empty, activityDetailsDto);
        var command = new UpdateActivityCommand(activityDto);

        var activityDetailsDto1 = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), _faker.Date.Future());
        var activityDto1 = new ActivityDto(_faker.Random.Guid(), _faker.Random.Guid(), _faker.Random.String(1), activityDetailsDto);
        var command1 = new UpdateActivityCommand(activityDto);

        var activityDetailsDto2 = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), _faker.Date.Future());
        var activityDto2 = new ActivityDto(_faker.Random.Guid(), _faker.Random.Guid(), _faker.Random.String(51), activityDetailsDto);
        var command2 = new UpdateActivityCommand(activityDto);


        //Act
        var result = await _updateActivityCommandValidator.TestValidateAsync(command);
        var result1 = await _updateActivityCommandValidator.TestValidateAsync(command1);
        var result2 = await _updateActivityCommandValidator.TestValidateAsync(command2);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Id);
        result.ShouldHaveValidationErrorFor(x => x.Activity.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.ThemeId);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Url);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Date);

        result1.ShouldNotHaveValidationErrorFor(x => x.Activity.Id);
        result1.ShouldHaveValidationErrorFor(x => x.Activity.Name);
        result1.ShouldNotHaveValidationErrorFor(x => x.Activity.ThemeId);
        result1.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Url);
        result1.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Date);

        result2.ShouldNotHaveValidationErrorFor(x => x.Activity.Id);
        result2.ShouldHaveValidationErrorFor(x => x.Activity.Name);
        result2.ShouldNotHaveValidationErrorFor(x => x.Activity.ThemeId);
        result2.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Url);
        result2.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Date);

    }

    [Fact]
    public async Task UpdateActivityCommandValidator_ValidateActivity_When_UpdateActivityCommand_Object_Called_Should_Return_ThemeId_Empty_Validation_Exceptions()
    {
        //Arrange
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), _faker.Date.Future());
        var activityDto = new ActivityDto(_faker.Random.Guid(), Guid.Empty, _faker.Lorem.Word(), activityDetailsDto);
        var command = new UpdateActivityCommand(activityDto);

        //Act
        var result = await _updateActivityCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Name);
        result.ShouldHaveValidationErrorFor(x => x.Activity.ThemeId);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Url);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Date);

    }


    [Fact]
    public async Task CreateActivityCommandValidator_ValidateActivity_When_CreateActivityCommand_Object_Called_Return_Url_Empty_And_Length_Validation_Exceptions()
    {
        //Arrange
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), string.Empty, _faker.Date.Future());
        var activityDto = new ActivityDto(_faker.Random.Guid(), _faker.Random.Guid(), _faker.Lorem.Word(), activityDetailsDto);
        var command = new UpdateActivityCommand(activityDto);

        var activityDetailsDto1 = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Random.String(501), _faker.Date.Future());
        var activityDto1 = new ActivityDto(_faker.Random.Guid(), _faker.Random.Guid(), _faker.Lorem.Word(), activityDetailsDto);
        var command1 = new UpdateActivityCommand(activityDto);


        //Act
        var result = await _updateActivityCommandValidator.TestValidateAsync(command);
        var result1 = await _updateActivityCommandValidator.TestValidateAsync(command1);
        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.ThemeId);
        result.ShouldHaveValidationErrorFor(x => x.Activity.Details.Url);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Date);

        result1.ShouldNotHaveValidationErrorFor(x => x.Activity.Id);
        result1.ShouldNotHaveValidationErrorFor(x => x.Activity.Name);
        result1.ShouldNotHaveValidationErrorFor(x => x.Activity.ThemeId);
        result1.ShouldHaveValidationErrorFor(x => x.Activity.Details.Url);
        result1.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Date);

    }

    [Fact]
    public async Task UpdateActivityCommandValidator_ValidateActivity_When_UpdateActivityCommand_Object_Called_Should_Return_Date_Empty_Validation_Exceptions()
    {
        //Arrange
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), new DateTime());
        var activityDto = new ActivityDto(_faker.Random.Guid(), _faker.Random.Guid(), _faker.Lorem.Word(), activityDetailsDto);
        var command = new UpdateActivityCommand(activityDto);

        //Act
        var result = await _updateActivityCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.ThemeId);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Url);
        result.ShouldHaveValidationErrorFor(x => x.Activity.Details.Date);

    }

    #endregion
}
