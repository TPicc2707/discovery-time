namespace Activity.Application.Tests.Themes.Integrations;

public class ThemeActivityUpdatedEventTests
{
    private readonly IMockNSubstituteMethods _mockingFramework;

    public ThemeActivityUpdatedEventTests()
    {
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
    }

    #region Consume

    [Fact]
    public async Task ThemeActivityUpdatedEventHandler_Consume_When_Called_Should_Return_Expected_Results()
    {
        //Arrange
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<ThemeActivityUpdatedEventHandler>>(new object[] { });
        var mockSender = _mockingFramework.InitializeMockedClass<ISender>(new object[] { });

        var consumer = new ThemeActivityUpdatedEventHandler(mockSender, mockLoggingObject);

        var theme = new ThemeUpdatedEvent();
        theme.Id = Guid.NewGuid();
        theme.Name = "New Theme";

        var updateTheme = _mockingFramework.InitializeMockedClass<ConsumeContext<ThemeUpdatedEvent>>(new object[] { });
        updateTheme.Message.Returns(theme);

        //Act
        await consumer.Consume(updateTheme);

        //Assert
        _mockingFramework.VerifyMethodRun(mockSender, x => x.Send(_mockingFramework.GetObject<UpdateThemeCommand>(), _mockingFramework.GetObject<CancellationToken>()), 1);
    }


    [Fact]
    public async Task ThemeActivityUpdatedEventHandler_Consume_When_Called_Should_Return_Logging_Exception()
    {
        //Arrange
        var expected = "foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<ThemeActivityUpdatedEventHandler>>(new object[] { });
        var mockSender = _mockingFramework.InitializeMockedClass<ISender>(new object[] { });

        var consumer = new ThemeActivityUpdatedEventHandler(mockSender, mockLoggingObject);

        var theme = new ThemeUpdatedEvent();
        theme.Id = Guid.NewGuid();
        theme.Name = "New Theme";

        var updateTheme = _mockingFramework.InitializeMockedClass<ConsumeContext<ThemeUpdatedEvent>>(new object[] { });
        updateTheme.Message.Returns(theme);

        _mockingFramework.SetupThrowsException(mockSender, x => x.Send(_mockingFramework.GetObject<UpdateThemeCommand>(), _mockingFramework.GetObject<CancellationToken>()), new Exception(expected));

        //Act
        Func<Task> act = () => consumer.Consume(updateTheme);

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);
    }

    #endregion
}
