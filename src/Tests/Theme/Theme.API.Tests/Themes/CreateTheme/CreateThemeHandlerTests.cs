namespace Theme.API.Tests.Themes.CreateTheme;

public class CreateThemeHandlerTests
{
    private readonly IMockNSubstituteMethods _mockingFramework;
    private readonly CreateThemeCommandValidator _createThemeCommandValidator;
    private readonly RandomGenerator randomGenerator;

    public CreateThemeHandlerTests()
    {
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
        _createThemeCommandValidator = new CreateThemeCommandValidator();
        randomGenerator = new RandomGenerator();
    }

    #region CreateThemeHandler Tests

    [Fact]
    public async Task CreateThemeHandler_Handle_WhenCalled_Should_Return_Expected_Result()
    {
        //Arrange
        var command = new CreateThemeCommand(randomGenerator.RandomString(20), 1, randomGenerator.RandomString(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester", "Tester");

        var mockDocumentSession = _mockingFramework.InitializeMockedClass<IDocumentSession>(new object[] { });

        var handler = new CreateThemeHandler(mockDocumentSession);

        //Act
        var result = await handler.Handle(command, new CancellationToken());
        var actual = result.Id;

        //Assert
        _mockingFramework.VerifyMethodRun(mockDocumentSession, c => c.Store(_mockingFramework.GetObject<Models.Theme>()), 1);
        _mockingFramework.VerifyMethodRun(mockDocumentSession, c => c.SaveChangesAsync(_mockingFramework.GetObject<CancellationToken>()), 1);
        Assert.IsType<CreateThemeResult>(result);
        Assert.IsType<Guid>(actual);
    }

    [Fact]
    public async Task CreateThemeHandler_Handle_WhenCalled_Should_Return_ArgumentNullException_Result()
    {
        //Arrange
        var command = new CreateThemeCommand(randomGenerator.RandomString(20), 1, randomGenerator.RandomString(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester", "Tester");

        var mockDocumentSession = _mockingFramework.InitializeMockedClass<IDocumentSession>(new object[] { });

        _mockingFramework.SetupThrowsException(mockDocumentSession, x => x.Store(_mockingFramework.GetObject<Models.Theme>()), new ArgumentNullException());

        var handler = new CreateThemeHandler(mockDocumentSession);

        //Act/Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(command, new CancellationToken()));
    }

    [Fact]
    public async Task CreateThemeHandler_Handle_WhenCalled_Should_Return_Exception_Result()
    {
        //Arrange
        var command = new CreateThemeCommand(randomGenerator.RandomString(20), 1, randomGenerator.RandomString(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester", "Tester");
        var expected = "Foo";
        var mockDocumentSession = _mockingFramework.InitializeMockedClass<IDocumentSession>(new object[] { });

        _mockingFramework.SetupThrowsException(mockDocumentSession, x => x.SaveChangesAsync(_mockingFramework.GetObject<CancellationToken>()), new Exception(expected));

        var handler = new CreateThemeHandler(mockDocumentSession);

        //Act/Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, new CancellationToken()));
    }

    #endregion

    #region CreateThemeCommandValidation Tests

    [Fact]
    public async Task CreateThemeCommandValidator_ValidateTheme_When_CreateThemeCommand_Object_Called_Should_Return_Expected_Result()
    {
        //Arrange
        var command = new CreateThemeCommand(randomGenerator.RandomString(20), 1, randomGenerator.RandomString(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester", "Tester");

        //Act
        var result = await _createThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Number);
        result.ShouldNotHaveValidationErrorFor(x => x.Letter);
        result.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.CreatedBy);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);
    }

    [Fact]
    public async Task CreateThemeCommandValidator_ValidateTheme_When_CreateThemeCommand_Object_Called_Should_Return_All_Validation_Exceptions()
    {
        //Arrange
        var command = new CreateThemeCommand(null, -1, randomGenerator.RandomString(1), new DateTime(), new DateTime(), null, null);

        //Act
        var result = await _createThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Number);
        result.ShouldHaveValidationErrorFor(x => x.Letter);
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
        result.ShouldHaveValidationErrorFor(x => x.CreatedBy);
        result.ShouldHaveValidationErrorFor(x => x.ModifiedBy);

    }

    [Fact]
    public async Task CreateThemeCommandValidator_ValidateTheme_When_CreateThemeCommand_Object_Called_Should_Return_Name_Is_Not_Valid_Length()
    {
        //Arrange
        var command = new CreateThemeCommand(randomGenerator.RandomString(1), 1, randomGenerator.RandomString(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester", "Tester");
        var command1 = new CreateThemeCommand(randomGenerator.RandomString(151), 1, randomGenerator.RandomString(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester", "Tester");

        //Act
        var result = await _createThemeCommandValidator.TestValidateAsync(command);
        var result1 = await _createThemeCommandValidator.TestValidateAsync(command1);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Number);
        result.ShouldNotHaveValidationErrorFor(x => x.Letter);
        result.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.CreatedBy);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);

        result1.ShouldHaveValidationErrorFor(x => x.Name);
        result1.ShouldNotHaveValidationErrorFor(x => x.Number);
        result1.ShouldNotHaveValidationErrorFor(x => x.Letter);
        result1.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result1.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.CreatedBy);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);

    }

    [Fact]
    public async Task CreateThemeCommandValidator_ValidateTheme_When_CreateThemeCommand_Object_Called_Should_Return_Number_Validation()
    {
        //Arrange
        var command = new CreateThemeCommand(randomGenerator.RandomString(20), -1, randomGenerator.RandomString(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester", "Tester");
        var command1 = new CreateThemeCommand(randomGenerator.RandomString(20), 101, randomGenerator.RandomString(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester", "Tester");

        //Act
        var result = await _createThemeCommandValidator.TestValidateAsync(command);
        var result1 = await _createThemeCommandValidator.TestValidateAsync(command1);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Number);
        result.ShouldNotHaveValidationErrorFor(x => x.Letter);
        result.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.CreatedBy);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);


        result1.ShouldNotHaveValidationErrorFor(x => x.Name);
        result1.ShouldHaveValidationErrorFor(x => x.Number);
        result1.ShouldNotHaveValidationErrorFor(x => x.Letter);
        result1.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result1.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.CreatedBy);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);

    }

    [Fact]
    public async Task CreateThemeCommandValidator_ValidateTheme_When_CreateThemeCommand_Object_Called_Should_Return_Letter_Length_Validation()
    {
        //Arrange
        var command = new CreateThemeCommand(randomGenerator.RandomString(20), 1, randomGenerator.RandomString(1), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester", "Tester");
        var command1 = new CreateThemeCommand(randomGenerator.RandomString(20), 1, randomGenerator.RandomString(3), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester", "Tester");

        //Act
        var result = await _createThemeCommandValidator.TestValidateAsync(command);
        var result1 = await _createThemeCommandValidator.TestValidateAsync(command1);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Number);
        result.ShouldHaveValidationErrorFor(x => x.Letter);
        result.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.CreatedBy);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);

        result1.ShouldNotHaveValidationErrorFor(x => x.Name);
        result1.ShouldNotHaveValidationErrorFor(x => x.Number);
        result1.ShouldHaveValidationErrorFor(x => x.Letter);
        result1.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result1.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.CreatedBy);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);

    }

    [Fact]
    public async Task CreateThemeCommandValidator_ValidateTheme_When_CreateThemeCommand_Object_Called_Should_Return_StartDate_Validation()
    {
        //Arrange
        var command = new CreateThemeCommand(randomGenerator.RandomString(20), 1, randomGenerator.RandomString(2), new DateTime(), DateTime.UtcNow.AddDays(10), "Tester", "Tester");

        //Act
        var result = await _createThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Number);
        result.ShouldNotHaveValidationErrorFor(x => x.Letter);
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.CreatedBy);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);


    }

    [Fact]
    public async Task CreateThemeCommandValidator_ValidateTheme_When_CreateThemeCommand_Object_Called_Should_Return_EndDate_Validation()
    {
        //Arrange
        var command = new CreateThemeCommand(randomGenerator.RandomString(20), 1, randomGenerator.RandomString(2), DateTime.UtcNow, new DateTime(), "Tester", "Tester");

        //Act
        var result = await _createThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Number);
        result.ShouldNotHaveValidationErrorFor(x => x.Letter);
        result.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.CreatedBy);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);

    }

    [Fact]
    public async Task CreateThemeCommandValidator_ValidateTheme_When_CreateThemeCommand_Object_Called_Should_Return_CreatedBy_Validation()
    {
        //Arrange
        var command = new CreateThemeCommand(randomGenerator.RandomString(20), 1, randomGenerator.RandomString(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), null, "Tester");

        //Act
        var result = await _createThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Number);
        result.ShouldNotHaveValidationErrorFor(x => x.Letter);
        result.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldHaveValidationErrorFor(x => x.CreatedBy);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);

    }

    [Fact]
    public async Task CreateThemeCommandValidator_ValidateTheme_When_CreateThemeCommand_Object_Called_Should_Return_ModifiedBy_Validation()
    {
        //Arrange
        var command = new CreateThemeCommand(randomGenerator.RandomString(20), 1, randomGenerator.RandomString(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester", null);

        //Act
        var result = await _createThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Number);
        result.ShouldNotHaveValidationErrorFor(x => x.Letter);
        result.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.CreatedBy);
        result.ShouldHaveValidationErrorFor(x => x.ModifiedBy);

    }

    #endregion

}
