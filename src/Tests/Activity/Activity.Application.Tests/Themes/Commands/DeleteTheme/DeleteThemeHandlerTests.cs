namespace Activity.Application.Tests.Themes.Commands.DeleteTheme;

public class DeleteThemeHandlerTests
{
    private readonly IMockNSubstituteMethods _mockingFramework;
    private readonly DeleteThemeCommandValidator _deleteThemeCommandValidator;
    private readonly Faker _faker;

    public DeleteThemeHandlerTests()
    {
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
        _deleteThemeCommandValidator = new DeleteThemeCommandValidator();
        _faker = new Faker();
    }

    #region DeleteThemeHandler

    [Fact]
    public async Task DeleteThemeHandler_Handle_WhenCalled_Should_Return_Expected_Result()
    {
        //Arrange
        var command = new DeleteThemeCommand(Guid.NewGuid());
        var theme = Domain.Models.Theme.Create(id: ThemeId.Of(command.Id), name: _faker.Lorem.Word());
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });
        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetByIdAsync(_mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>() }, theme);
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork.Theme, x => x.Delete(_mockingFramework.GetObject<Domain.Models.Theme>()), new object[] { _mockingFramework.GetObject<Domain.Models.Theme>() });
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        var handler = new DeleteThemeHandler(mockUnitOfWork);
        var expected = true;

        //Act
        var result = await handler.Handle(command, new CancellationToken());
        var actual = result.IsSuccess;

        //Assert
        _mockingFramework.VerifyMethodRun(mockUnitOfWork.Theme, c => c.Delete(_mockingFramework.GetObject<Domain.Models.Theme>()), 1);
        _mockingFramework.VerifyMethodRun(mockUnitOfWork, c => c.Complete(_mockingFramework.GetObject<CancellationToken>()), 1);
        Assert.IsType<DeleteThemeResult>(result);
        Assert.True(actual);

    }

    [Fact]
    public async Task DeleteThemeHandler_Handle_WhenCalled_Should_Return_Theme_NotFound_Exception()
    {
        //Arrange
        var command = new DeleteThemeCommand(Guid.NewGuid());
        Domain.Models.Theme theme = null;

        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });
        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetByIdAsync(_mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>() }, theme);
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork.Theme, x => x.Delete(_mockingFramework.GetObject<Domain.Models.Theme>()), new object[] { _mockingFramework.GetObject<Domain.Models.Theme>() });
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        var handler = new DeleteThemeHandler(mockUnitOfWork);

        //Act/Assert
        await Assert.ThrowsAsync<ThemeNotFoundException>(() => handler.Handle(command, new CancellationToken()));
    }

    [Fact]
    public async Task DeleteThemeHandler_Handle_WhenCalled_Should_Return_InvalidOperationException_Result()
    {
        //Arrange
        var expected = "foo";
        var command = new DeleteThemeCommand(Guid.NewGuid());
        var theme = Domain.Models.Theme.Create(id: ThemeId.Of(command.Id), name: _faker.Lorem.Word());
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });

        var handler = new DeleteThemeHandler(mockUnitOfWork);

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetByIdAsync(_mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>() }, theme);
        _mockingFramework.SetupThrowsException(mockUnitOfWork.Theme, x => x.Delete(_mockingFramework.GetObject<Domain.Models.Theme>()), new InvalidOperationException(expected));
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        //Act
        Func<Task> act = () => handler.Handle(command, new CancellationToken());

        //Assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Contains(expected, ex.Message);
    }

    [Fact]
    public async Task DeleteThemeHandler_Handle_WhenCalled_Should_Return_Exception_Result()
    {
        //Arrange
        var expected = "foo";
        var command = new DeleteThemeCommand(Guid.NewGuid());
        var theme = Domain.Models.Theme.Create(id: ThemeId.Of(command.Id), name: _faker.Lorem.Word());
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<IUnitOfWork>(new object[] { });

        var handler = new DeleteThemeHandler(mockUnitOfWork);

        _mockingFramework.SetupReturnsResult(mockUnitOfWork.Theme, x => x.GetByIdAsync(_mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>() }, theme);
        _mockingFramework.SetupThrowsException(mockUnitOfWork.Theme, x => x.Delete(_mockingFramework.GetObject<Domain.Models.Theme>()), new Exception(expected));
        _mockingFramework.SetupReturnNoneResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() });

        //Act
        Func<Task> act = () => handler.Handle(command, new CancellationToken());

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);
    }

    #endregion

    #region DeleteThemeCommandValidator

    [Fact]
    public async Task DeleteThemeCommandValidator_ValidateTheme_When_DeleteThemeCommand_Object_Called_Should_Return_Expected_Result()
    {
        //Arrange
        var command = new DeleteThemeCommand(Guid.NewGuid());

        //Act
        var result = await _deleteThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task DeleteThemeCommandValidator_ValidateTheme_When_DeleteThemeCommand_Object_Called_Should_Return_Empty_Guid_Validation_Exception()
    {
        //Arrange
        var command = new DeleteThemeCommand(Guid.Empty);

        //Act
        var result = await _deleteThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);

    }

    #endregion
}
