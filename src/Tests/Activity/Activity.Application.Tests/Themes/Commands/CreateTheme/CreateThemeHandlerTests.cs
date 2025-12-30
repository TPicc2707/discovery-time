namespace Activity.Application.Tests.Themes.Commands.CreateTheme;

public class CreateThemeHandlerTests
{
    private readonly IMockNSubstituteMethods _mockingFramework;
    private readonly CreateThemeCommandValidator _createThemeCommandValidator;
    private readonly Faker _faker;

    public CreateThemeHandlerTests()
    {
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
        _createThemeCommandValidator = new CreateThemeCommandValidator();
        _faker = new Faker();
    }

    #region CreateThemeHandler

    [Fact]
    public async Task CreateThemeHandler_Handle_WhenCalled_Should_Return_Expected_Result()
    {
        //Arrange
        var themeDto = new ThemeDto(_faker.Random.Guid(), _faker.Lorem.Word());
        var command = new CreateThemeCommand(themeDto);
        var expectedId = command.Theme.Id;
        var theme = Domain.Models.Theme.Create(id: ThemeId.Of(command.Theme.Id), name: command.Theme.Name);
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });

        var handler = new CreateThemeHandler(mockUnitOfWork);

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.AddAsync(_mockingFramework.GetObject<Domain.Models.Theme>()), new object[] { _mockingFramework.GetObject<Domain.Models.Theme>() }, theme);
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        //Act
        var result = await handler.Handle(command, new CancellationToken());
        var actualId = result.Id;

        //Assert
        Assert.Equal(expectedId, actualId);
        Assert.IsType<CreateThemeResult>(result);
    }

    [Fact]
    public async Task CreateThemeHandler_Handle_WhenCalled_Should_Return_InvalidOperationException_Result()
    {
        //Arrange
        var expected = "foo";
        var themeDto = new ThemeDto(_faker.Random.Guid(), _faker.Lorem.Word());
        var command = new CreateThemeCommand(themeDto);
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });

        var handler = new CreateThemeHandler(mockUnitOfWork);

        _mockingFramework.SetupThrowsException(mockUnitOfWork.Theme, x => x.AddAsync(_mockingFramework.GetObject<Domain.Models.Theme>()), new object[] { _mockingFramework.GetObject<Domain.Models.Theme>() }, new InvalidOperationException(expected));
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        //Act
        Func<Task> act = () => handler.Handle(command, new CancellationToken());

        //Assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Contains(expected, ex.Message);
    }

    [Fact]
    public async Task CreateThemeHandler_Handle_WhenCalled_Should_Return_Exception_Result()
    {
        //Arrange
        var expected = "foo";
        var themeDto = new ThemeDto(_faker.Random.Guid(), _faker.Lorem.Word());
        var command = new CreateThemeCommand(themeDto);
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });

        var handler = new CreateThemeHandler(mockUnitOfWork);

        _mockingFramework.SetupThrowsException(mockUnitOfWork.Theme, x => x.AddAsync(_mockingFramework.GetObject<Domain.Models.Theme>()), new object[] { _mockingFramework.GetObject<Domain.Models.Theme>() }, new Exception(expected));
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        //Act
        Func<Task> act = () => handler.Handle(command, new CancellationToken());

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);
    }

    #endregion

    #region CreateThemeCommandValidator

    [Fact]
    public async Task CreateThemeCommandValidator_ValidateTheme_When_CreateThemeCommand_Object_Called_Should_Return_Expected_Result()
    {
        //Arrange
        var themeDto = new ThemeDto(_faker.Random.Guid(), _faker.Lorem.Word());
        var command = new CreateThemeCommand(themeDto);

        //Act
        var result = await _createThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Theme.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Theme.Name);
    }

    [Fact]
    public async Task CreateThemeCommandValidator_ValidateTheme_When_CreateThemeCommand_Object_Called_Should_Return_All_Validation_Exceptions()
    {
        //Arrange
        var themeDto = new ThemeDto(Guid.Empty, string.Empty);
        var command = new CreateThemeCommand(themeDto);

        //Act
        var result = await _createThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Theme.Id);
        result.ShouldHaveValidationErrorFor(x => x.Theme.Name);

    }

    [Fact]
    public async Task CreateThemeCommandValidator_ValidateTheme_When_CreateThemeCommand_Object_Called_Should_Return_A_Empty_Validation_On_Id()
    {
        //Arrange
        var themeDto = new ThemeDto(Guid.Empty, _faker.Lorem.Word());
        var command = new CreateThemeCommand(themeDto);

        //Act
        var result = await _createThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Theme.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Theme.Name);

    }

    [Fact]
    public async Task CreateThemeCommandValidator_ValidateTheme_When_CreateThemeCommand_Object_Called_Should_Return_A_Empty_Validation_On_Name()
    {
        //Arrange
        var themeDto = new ThemeDto(_faker.Random.Guid(), string.Empty);
        var command = new CreateThemeCommand(themeDto);

        //Act
        var result = await _createThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Theme.Id);
        result.ShouldHaveValidationErrorFor(x => x.Theme.Name);

    }

    #endregion
}
