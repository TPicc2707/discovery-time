namespace Activity.Application.Tests.Activities.Commands.CreateActivity;

public class CreateActivityHandlerTests
{
    private readonly IMockNSubstituteMethods _mockingFramework;
    private readonly CreateActivityCommandValidator _createActivityCommandValidator;
    private readonly Faker _faker;

    public CreateActivityHandlerTests()
    {
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
        _createActivityCommandValidator = new CreateActivityCommandValidator();
        _faker = new Faker();
    }

    #region CreateActivityHandler

    [Fact]
    public async Task CreateActivityHandler_Handle_WhenCalled_Should_Return_Expected_Result()
    {
        //Arrange
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), _faker.Date.Future());
        var activityDto = new ActivityDto(Guid.Empty, _faker.Random.Guid(), _faker.Lorem.Word(), activityDetailsDto);
        var command = new CreateActivityCommand(activityDto);
        var activityDetails = ActivityDetails.Of(activityDto.Details.Description, activityDto.Details.Url, activityDto.Details.Date);
        var activity = Domain.Models.Activity.Create(id: ActivityId.Of(Guid.NewGuid()), themeId: ThemeId.Of(activityDto.ThemeId), ActivityName.Of(activityDto.Name), activityDetails);
        var expectedId = activity.Id.Value;
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });

        var handler = new CreateActivityHandler(mockUnitOfWork);

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.CreateThemeActivityAsync(_mockingFramework.GetObject<Domain.Models.Activity>()), new object[] { _mockingFramework.GetObject<Domain.Models.Activity>() }, activity);
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        //Act
        var result = await handler.Handle(command, new CancellationToken());
        var actualId = result.Id;

        //Assert
        Assert.Equal(expectedId, actualId);
        Assert.IsType<CreateActivityResult>(result);
    }

    [Fact]
    public async Task CreateActivityHandler_Handle_WhenCalled_Should_Return_InvalidOperationException_Result()
    {
        //Arrange
        var expected = "foo";
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), _faker.Date.Future());
        var activityDto = new ActivityDto(Guid.Empty, _faker.Random.Guid(), _faker.Lorem.Word(), activityDetailsDto);
        var command = new CreateActivityCommand(activityDto);
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });

        var handler = new CreateActivityHandler(mockUnitOfWork);

        _mockingFramework.SetupThrowsException(mockUnitOfWork.Theme, x => x.CreateThemeActivityAsync(_mockingFramework.GetObject<Domain.Models.Activity>()), new object[] { _mockingFramework.GetObject<Domain.Models.Activity>() }, new InvalidOperationException(expected));
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        //Act
        Func<Task> act = () => handler.Handle(command, new CancellationToken());

        //Assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Contains(expected, ex.Message);
    }

    [Fact]
    public async Task CreateActivityHandler_Handle_WhenCalled_Should_Return_NullReference_Result()
    {
        //Arrange
        var expected = "foo";
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), _faker.Date.Future());
        var activityDto = new ActivityDto(Guid.Empty, _faker.Random.Guid(), _faker.Lorem.Word(), activityDetailsDto);
        var command = new CreateActivityCommand(activityDto);
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });

        var handler = new CreateActivityHandler(mockUnitOfWork);

        _mockingFramework.SetupThrowsException(mockUnitOfWork.Theme, x => x.CreateThemeActivityAsync(_mockingFramework.GetObject<Domain.Models.Activity>()), new object[] { _mockingFramework.GetObject<Domain.Models.Activity>() }, new NullReferenceException(expected));
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        //Act
        Func<Task> act = () => handler.Handle(command, new CancellationToken());

        //Assert
        NullReferenceException ex = await Assert.ThrowsAsync<NullReferenceException>(act);
        Assert.Contains(expected, ex.Message);
    }

    [Fact]
    public async Task CreateActivityHandler_Handle_WhenCalled_Should_Return_Exception_Result()
    {
        //Arrange
        var expected = "foo";
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), _faker.Date.Future());
        var activityDto = new ActivityDto(Guid.Empty, _faker.Random.Guid(), _faker.Lorem.Word(), activityDetailsDto);
        var command = new CreateActivityCommand(activityDto);
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });

        var handler = new CreateActivityHandler(mockUnitOfWork);

        _mockingFramework.SetupThrowsException(mockUnitOfWork.Theme, x => x.CreateThemeActivityAsync(_mockingFramework.GetObject<Domain.Models.Activity>()), new object[] { _mockingFramework.GetObject<Domain.Models.Activity>() }, new Exception(expected));
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        //Act
        Func<Task> act = () => handler.Handle(command, new CancellationToken());

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);
    }

    #endregion

    #region CreateActivityCommandValidator

    [Fact]
    public async Task CreateActivityCommandValidator_ValidateActivity_When_CreateActivityCommand_Object_Called_Should_Return_Expected_Result()
    {
        //Arrange
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), _faker.Date.Future());
        var activityDto = new ActivityDto(Guid.Empty, _faker.Random.Guid(), _faker.Lorem.Word(), activityDetailsDto);
        var command = new CreateActivityCommand(activityDto);

        //Act
        var result = await _createActivityCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.ThemeId);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Url);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Date);
    }

    [Fact]
    public async Task CreateActivityCommandValidator_ValidateActivity_When_CreateActivityCommand_Object_Called_Should_Return_All_Validation_Exceptions()
    {
        //Arrange
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), string.Empty, new DateTime());
        var activityDto = new ActivityDto(Guid.Empty, Guid.Empty, string.Empty, activityDetailsDto);
        var command = new CreateActivityCommand(activityDto);

        //Act
        var result = await _createActivityCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Activity.Name);
        result.ShouldHaveValidationErrorFor(x => x.Activity.ThemeId);
        result.ShouldHaveValidationErrorFor(x => x.Activity.Details.Url);
        result.ShouldHaveValidationErrorFor(x => x.Activity.Details.Date);

    }

    [Fact]
    public async Task CreateActivityCommandValidator_ValidateActivity_When_CreateActivityCommand_Object_Called_Should_Return_Name_Is_Not_Valid_Length()
    {
        //Arrange
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), _faker.Date.Future());
        var activityDto = new ActivityDto(Guid.Empty, _faker.Random.Guid(), _faker.Random.String(1), activityDetailsDto);
        var activityDetailsDto1 = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), _faker.Date.Future());
        var activityDto1 = new ActivityDto(Guid.Empty, _faker.Random.Guid(), _faker.Random.String(51), activityDetailsDto);

        var command = new CreateActivityCommand(activityDto);
        var command1 = new CreateActivityCommand(activityDto1);

        //Act
        var result = await _createActivityCommandValidator.TestValidateAsync(command);
        var result1 = await _createActivityCommandValidator.TestValidateAsync(command1);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Activity.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.ThemeId);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Url);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Date);

        result1.ShouldHaveValidationErrorFor(x => x.Activity.Name);
        result1.ShouldNotHaveValidationErrorFor(x => x.Activity.ThemeId);
        result1.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Url);
        result1.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Date);

    }


    [Fact]
    public async Task CreateActivityCommandValidator_ValidateActivity_When_CreateActivityCommand_Object_Called_Should_Return_ThemeId_Empty_Validation_Exceptions()
    {
        //Arrange
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), _faker.Date.Future());
        var activityDto = new ActivityDto(Guid.Empty, Guid.Empty, _faker.Lorem.Word(), activityDetailsDto);

        var command = new CreateActivityCommand(activityDto);

        //Act
        var result = await _createActivityCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Name);
        result.ShouldHaveValidationErrorFor(x => x.Activity.ThemeId);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Url);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Date);

    }

    [Fact]
    public async Task CreateActivityCommandValidator_ValidateActivity_When_CreateActivityCommand_Object_Called_Should_Return_Url_Is_Not_Valid_Length()
    {
        //Arrange
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Random.String(501), _faker.Date.Future());
        var activityDto = new ActivityDto(Guid.Empty, _faker.Random.Guid(), _faker.Lorem.Word(), activityDetailsDto);

        var command = new CreateActivityCommand(activityDto);

        //Act
        var result = await _createActivityCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.ThemeId);
        result.ShouldHaveValidationErrorFor(x => x.Activity.Details.Url);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Date);

    }

    [Fact]
    public async Task CreateActivityCommandValidator_ValidateActivity_When_CreateActivityCommand_Object_Called_Should_Return_Date_Empty_Validation_Exceptions()
    {
        //Arrange
        var activityDetailsDto = new ActivityDetailsDto(_faker.Lorem.Sentence(), _faker.Internet.Url(), new DateTime());
        var activityDto = new ActivityDto(Guid.Empty, _faker.Random.Guid(), _faker.Lorem.Word(), activityDetailsDto);
        var command = new CreateActivityCommand(activityDto);

        //Act
        var result = await _createActivityCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.ThemeId);
        result.ShouldNotHaveValidationErrorFor(x => x.Activity.Details.Url);
        result.ShouldHaveValidationErrorFor(x => x.Activity.Details.Date);

    }

    #endregion
}
