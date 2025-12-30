namespace Activity.Application.Tests.Themes.Integrations;

public class ThemeActivityCreationEventTests
{
    private readonly IMockNSubstituteMethods _mockingFramework;

    public ThemeActivityCreationEventTests()
    {
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
    }

    #region Consume

    [Fact]
    public async Task ThemeActivityCreationEventHandler_Consume_When_Called_Should_Return_Expected_Results()
    {
        //Arrange
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<ThemeCreationEventHandler>>(new object[] { });
        var mockSender = _mockingFramework.InitializeMockedClass<ISender>(new object[] { });

        var consumer = new ThemeCreationEventHandler(mockSender, mockLoggingObject);

        var theme = new ThemeCreationEvent();
        theme.Id = Guid.NewGuid();
        theme.Name = "New Theme";

        var createTheme = _mockingFramework.InitializeMockedClass<ConsumeContext<ThemeCreationEvent>>(new object[] { });
        createTheme.Message.Returns(theme);

        //Act
        await consumer.Consume(createTheme);

        //Assert
        _mockingFramework.VerifyMethodRun(mockSender, x => x.Send(_mockingFramework.GetObject<CreateThemeCommand>(), _mockingFramework.GetObject<CancellationToken>()), 1);
    }


    [Fact]
    public async Task ThemeActivityCreationEventHandler_Consume_When_Called_Should_Return_Logging_Exception()
    {
        //Arrange
        var expected = "foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<ThemeCreationEventHandler>>(new object[] { });
        var mockSender = _mockingFramework.InitializeMockedClass<ISender>(new object[] { });

        var consumer = new ThemeCreationEventHandler(mockSender, mockLoggingObject);

        var theme = new ThemeCreationEvent();
        theme.Id = Guid.NewGuid();
        theme.Name = "New Theme";

        var createTheme = _mockingFramework.InitializeMockedClass<ConsumeContext<ThemeCreationEvent>>(new object[] { });
        createTheme.Message.Returns(theme);

        _mockingFramework.SetupThrowsException(mockSender, x => x.Send(_mockingFramework.GetObject<CreateThemeCommand>(), _mockingFramework.GetObject<CancellationToken>()), new Exception(expected));

        //Act
        Func<Task> act = () => consumer.Consume(createTheme);

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);
    }

    #endregion
}
