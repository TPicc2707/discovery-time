using Activity.Domain.Events;
using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace Theme.API.Tests.Themes.DeleteTheme;

public class DeleteThemeHandlerTests
{
    private readonly IMockNSubstituteMethods _mockingFramework;
    private readonly DeleteThemeCommandValidator _deleteThemeCommandValidator;

    public DeleteThemeHandlerTests()
    {
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
        _deleteThemeCommandValidator = new DeleteThemeCommandValidator();
    }

    #region DeleteThemeHandler Tests

    [Fact]
    public async Task DeleteThemeHandler_Handle_WhenCalled_Should_Return_Expected_Result()
    {
        //Arrange
        var command = new DeleteThemeCommand(Guid.NewGuid());

        var mockDocumentSession = _mockingFramework.InitializeMockedClass<IDocumentSession>(new object[] { });
        var mockPublishEndpoint = _mockingFramework.InitializeMockedClass<IPublishEndpoint>(new object[] { });
        _mockingFramework.SetupReturnNoneResult(mockPublishEndpoint, x => x.Publish(_mockingFramework.GetObject<ThemeDeletionEvent>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ThemeDeletionEvent>(), _mockingFramework.GetObject<CancellationToken>() });

        var handler = new DeleteThemeHandler(mockDocumentSession, mockPublishEndpoint);
        var expected = true;

        //Act
        var result = await handler.Handle(command, new CancellationToken());
        var actual = result.IsSuccess;

        //Assert
        _mockingFramework.VerifyMethodRun(mockDocumentSession, c => c.Delete(_mockingFramework.GetObject<Guid>()), 1);
        _mockingFramework.VerifyMethodRun(mockDocumentSession, c => c.SaveChangesAsync(_mockingFramework.GetObject<CancellationToken>()), 1);
        Assert.IsType<DeleteThemeResult>(result);
        Assert.True(actual);

    }

    [Fact]
    public async Task DeleteThemeHandler_Handle_WhenCalled_Should_Return_Exception_Result()
    {
        //Arrange
        var command = new DeleteThemeCommand(Guid.NewGuid());

        var mockDocumentSession = _mockingFramework.InitializeMockedClass<IDocumentSession>(new object[] { });
        var mockPublishEndpoint = _mockingFramework.InitializeMockedClass<IPublishEndpoint>(new object[] { });
        _mockingFramework.SetupReturnNoneResult(mockPublishEndpoint, x => x.Publish(_mockingFramework.GetObject<ThemeDeletionEvent>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ThemeDeletionEvent>(), _mockingFramework.GetObject<CancellationToken>() });

        var expected = "Foo";
        _mockingFramework.SetupThrowsException(mockDocumentSession, x => x.SaveChangesAsync(_mockingFramework.GetObject<CancellationToken>()), new Exception(expected));

        var handler = new DeleteThemeHandler(mockDocumentSession, mockPublishEndpoint);

        //Act/Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, new CancellationToken()));
    }


    #endregion

    #region DeleteThemeCommandValidation Tests

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
