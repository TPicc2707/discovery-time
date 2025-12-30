namespace Activity.Application.Tests.Themes.Integrations;

public class ThemeActivityDeletionEventTests
{
    private readonly IMockNSubstituteMethods _mockingFramework;

    public ThemeActivityDeletionEventTests()
    {
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
    }

    #region Consume

    [Fact]
    public async Task ThemeActivityDeletionEventHandler_Consume_When_Called_Should_Return_Expected_Results()
    {
        //Arrange
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<ThemeActivityDeletionEventHandler>>(new object[] { });
        var mockSender = _mockingFramework.InitializeMockedClass<ISender>(new object[] { });

        var consumer = new ThemeActivityDeletionEventHandler(mockSender, mockLoggingObject);

        var theme = new ThemeDeletionEvent();
        theme.Id = Guid.NewGuid();
 
        var deleteTheme = _mockingFramework.InitializeMockedClass<ConsumeContext<ThemeDeletionEvent>>(new object[] { });
        deleteTheme.Message.Returns(theme);

        //Act
        await consumer.Consume(deleteTheme);

        //Assert
        _mockingFramework.VerifyMethodRun(mockSender, x => x.Send(_mockingFramework.GetObject<DeleteThemeCommand>(), _mockingFramework.GetObject<CancellationToken>()), 1);
    }

    [Fact]
    public async Task ThemeActivityDeletionEventHandler_Consume_When_Called_Should_Return_Logging_Exception()
    {
        //Arrange
        var expected = "foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<ThemeActivityDeletionEventHandler>>(new object[] { });
        var mockSender = _mockingFramework.InitializeMockedClass<ISender>(new object[] { });

        var consumer = new ThemeActivityDeletionEventHandler(mockSender, mockLoggingObject);

        var theme = new ThemeDeletionEvent();
        theme.Id = Guid.NewGuid();

        var deleteTheme = _mockingFramework.InitializeMockedClass<ConsumeContext<ThemeDeletionEvent>>(new object[] { });
        deleteTheme.Message.Returns(theme);

        _mockingFramework.SetupThrowsException(mockSender, x => x.Send(_mockingFramework.GetObject<DeleteThemeCommand>(), _mockingFramework.GetObject<CancellationToken>()), new Exception(expected));

        //Act
        Func<Task> act = () => consumer.Consume(deleteTheme);

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);
    }


    #endregion
}
