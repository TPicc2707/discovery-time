namespace Activity.Application.Tests.Themes.Commands.UpdateTheme;

public class UpdateThemeHandlerTests
{
    private readonly IMockNSubstituteMethods _mockingFramework;
    private readonly UpdateThemeCommandValidator _updateThemeCommandValidator;
    private readonly Faker _faker;

    public UpdateThemeHandlerTests()
    {
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
        _updateThemeCommandValidator = new UpdateThemeCommandValidator();
        _faker = new Faker();
    }

    #region UpdateThemeHandler

    [Fact]
    public async Task UpdateThemeHandler_Handle_WhenCalled_Should_Return_Expected_Result()
    {
        //Arrange
        var themeDto = new ThemeDto(_faker.Random.Guid(), _faker.Lorem.Word());
        var command = new UpdateThemeCommand(themeDto);
        var theme = Domain.Models.Theme.Create(id: ThemeId.Of(command.Theme.Id), name: command.Theme.Name);

        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });
        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetByIdAsync(_mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>() }, theme);
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork.Theme, x => x.Update(_mockingFramework.GetObject<Domain.Models.Theme>()), new object[] { _mockingFramework.GetObject<Domain.Models.Theme>() });
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        var handler = new UpdateThemeHandler(mockUnitOfWork);

        var expected = true;

        //Act
        var result = await handler.Handle(command, new CancellationToken());
        var actual = result.IsSuccess;

        //Assert
        _mockingFramework.VerifyMethodRun(mockUnitOfWork.Theme, c => c.Update(_mockingFramework.GetObject<Domain.Models.Theme>()), 1);
        _mockingFramework.VerifyMethodRun(mockUnitOfWork, c => c.Complete(_mockingFramework.GetObject<CancellationToken>()), 1);
        Assert.IsType<UpdateThemeResult>(result);
        Assert.True(actual);

    }

    [Fact]
    public async Task UpdateThemeHandler_Handle_WhenCalled_Should_Return_Theme_NotFound_Exception()
    {
        //Arrange
        var themeDto = new ThemeDto(_faker.Random.Guid(), _faker.Lorem.Word());
        var command = new UpdateThemeCommand(themeDto);
        Domain.Models.Theme theme = null;

        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });
        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetByIdAsync(_mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>() }, theme);
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork.Theme, x => x.Update(_mockingFramework.GetObject<Domain.Models.Theme>()), new object[] { _mockingFramework.GetObject<Domain.Models.Theme>() });
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        var handler = new UpdateThemeHandler(mockUnitOfWork);

        //Act/Assert
        await Assert.ThrowsAsync<ThemeNotFoundException>(() => handler.Handle(command, new CancellationToken()));
    }

    [Fact]
    public async Task UpdateThemeHandler_Handle_WhenCalled_Should_Return_InvalidOperationException_Result()
    {
        //Arrange
        var expected = "foo";
        var themeDto = new ThemeDto(_faker.Random.Guid(), _faker.Lorem.Word());
        var command = new UpdateThemeCommand(themeDto);
        var theme = Domain.Models.Theme.Create(id: ThemeId.Of(command.Theme.Id), name: command.Theme.Name);
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });

        var handler = new UpdateThemeHandler(mockUnitOfWork);

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetByIdAsync(_mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>() }, theme);
        _mockingFramework.SetupThrowsException(mockUnitOfWork.Theme, x => x.Update(_mockingFramework.GetObject<Domain.Models.Theme>()), new InvalidOperationException(expected));
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        //Act
        Func<Task> act = () => handler.Handle(command, new CancellationToken());

        //Assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Contains(expected, ex.Message);
    }

    [Fact]
    public async Task UpdateThemeHandler_Handle_WhenCalled_Should_Return_Exception_Result()
    {
        //Arrange
        var expected = "foo";
        var themeDto = new ThemeDto(_faker.Random.Guid(), _faker.Lorem.Word());
        var command = new UpdateThemeCommand(themeDto);
        var theme = Domain.Models.Theme.Create(id: ThemeId.Of(command.Theme.Id), name: command.Theme.Name);
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });

        var handler = new UpdateThemeHandler(mockUnitOfWork);

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetByIdAsync(_mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>() }, theme);
        _mockingFramework.SetupThrowsException(mockUnitOfWork.Theme, x => x.Update(_mockingFramework.GetObject<Domain.Models.Theme>()), new Exception(expected));
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        //Act
        Func<Task> act = () => handler.Handle(command, new CancellationToken());

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);
    }
    #endregion

    #region UpdateThemeCommandValidator

    [Fact]
    public async Task UpdateThemeCommandValidator_ValidateTheme_When_UpdateThemeCommand_Object_Called_Should_Return_Expected_Result()
    {
        //Arrange
        var themeDto = new ThemeDto(_faker.Random.Guid(), _faker.Lorem.Word());
        var command = new UpdateThemeCommand(themeDto);

        //Act
        var result = await _updateThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Theme.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Theme.Name);
    }

    [Fact]
    public async Task UpdateThemeCommandValidator_ValidateTheme_When_UpdateThemeCommand_Object_Called_Should_Return_All_Validation_Exceptions()
    {
        //Arrange
        var themeDto = new ThemeDto(Guid.Empty, string.Empty);
        var command = new UpdateThemeCommand(themeDto);

        //Act
        var result = await _updateThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Theme.Id);
        result.ShouldHaveValidationErrorFor(x => x.Theme.Name);

    }

    [Fact]
    public async Task UpdateThemeCommandValidator_ValidateTheme_When_CreateThemeCommand_Object_Called_Should_Return_A_Empty_Validation_On_Id()
    {
        //Arrange
        var themeDto = new ThemeDto(Guid.Empty, _faker.Lorem.Word());
        var command = new UpdateThemeCommand(themeDto);

        //Act
        var result = await _updateThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Theme.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Theme.Name);

    }

    [Fact]
    public async Task UpdateThemeCommandValidator_ValidateTheme_When_CreateThemeCommand_Object_Called_Should_Return_A_Empty_Validation_On_Name()
    {
        //Arrange
        var themeDto = new ThemeDto(_faker.Random.Guid(), string.Empty);
        var command = new UpdateThemeCommand(themeDto);

        //Act
        var result = await _updateThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Theme.Id);
        result.ShouldHaveValidationErrorFor(x => x.Theme.Name);

    }

    #endregion
}
