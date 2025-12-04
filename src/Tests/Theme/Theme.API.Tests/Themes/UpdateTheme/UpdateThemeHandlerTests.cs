using Bogus;

namespace Theme.API.Tests.Themes.UpdateTheme;

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

    #region UpdateThemeValidation Tests

    [Fact]
    public async Task UpdateThemeHandler_Handle_WhenCalled_Should_Return_Expected_Result()
    {
        //Arrange
        var command = new UpdateThemeCommand(Guid.NewGuid(), _faker.Random.String(20), 1, _faker.Random.String(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester");
        var theme = new Models.Theme()
        {
            Id = command.Id,
            Name = command.Name,
            Number = command.Number,
            Letter = command.Letter,
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            CreatedBy = "Tester",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = command.ModifiedBy,
            ModifiedDate = DateTime.UtcNow
        };

        var mockDocumentSession = _mockingFramework.InitializeMockedClass<IDocumentSession>(new object[] { });
        _mockingFramework.SetupReturnsResult(mockDocumentSession, x => x.LoadAsync<Models.Theme>(_mockingFramework.GetObject<Guid>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<Guid>(), _mockingFramework.GetObject<CancellationToken>() }, theme);
        var handler = new UpdateThemeHandler(mockDocumentSession);

        var expected = true;

        //Act
        var result = await handler.Handle(command, new CancellationToken());
        var actual = result.IsSuccess;

        //Assert
        _mockingFramework.VerifyMethodRun(mockDocumentSession, c => c.Update(_mockingFramework.GetObject<Models.Theme>()), 1);
        _mockingFramework.VerifyMethodRun(mockDocumentSession, c => c.SaveChangesAsync(_mockingFramework.GetObject<CancellationToken>()), 1);
        Assert.IsType<UpdateThemeResult>(result);
        Assert.True(actual);

    }

    [Fact]
    public async Task UpdateThemeHandler_Handle_WhenCalled_Should_Return_Theme_NotFound_Exception()
    {
        //Arrange
        var command = new UpdateThemeCommand(Guid.NewGuid(), _faker.Random.String(20), 1, _faker.Random.String(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester");
        Models.Theme theme = null;

        var mockDocumentSession = _mockingFramework.InitializeMockedClass<IDocumentSession>(new object[] { });
        _mockingFramework.SetupReturnsResult(mockDocumentSession, x => x.LoadAsync<Models.Theme>(_mockingFramework.GetObject<Guid>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<Guid>(), _mockingFramework.GetObject<CancellationToken>() }, theme);
        var handler = new UpdateThemeHandler(mockDocumentSession);

        //Act/Assert
        await Assert.ThrowsAsync<ThemeNotFoundException>(() => handler.Handle(command, new CancellationToken()));
    }

    [Fact]
    public async Task UpdateThemeHandler_Handle_WhenCalled_Should_Return_ArgumentNullException_Result()
    {
        //Arrange
        var command = new UpdateThemeCommand(Guid.NewGuid(), _faker.Random.String(20), 1, _faker.Random.String(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester");
        var theme = new Models.Theme()
        {
            Id = command.Id,
            Name = command.Name,
            Number = command.Number,
            Letter = command.Letter,
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            CreatedBy = "Tester",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = command.ModifiedBy,
            ModifiedDate = DateTime.UtcNow
        };
        var mockDocumentSession = _mockingFramework.InitializeMockedClass<IDocumentSession>(new object[] { });
        _mockingFramework.SetupReturnsResult(mockDocumentSession, x => x.LoadAsync<Models.Theme>(_mockingFramework.GetObject<Guid>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<Guid>(), _mockingFramework.GetObject<CancellationToken>() }, theme);

        _mockingFramework.SetupThrowsException(mockDocumentSession, x => x.Update(_mockingFramework.GetObject<Models.Theme>()), new ArgumentNullException());

        var handler = new UpdateThemeHandler(mockDocumentSession);

        //Act/Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(command, new CancellationToken()));
    }

    [Fact]
    public async Task UpdateThemeHandler_Handle_WhenCalled_Should_Return_Exception_Result()
    {
        //Arrange
        var command = new UpdateThemeCommand(Guid.NewGuid(), _faker.Random.String(20), 1, _faker.Random.String(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester");
        var theme = new Models.Theme()
        {
            Id = command.Id,
            Name = command.Name,
            Number = command.Number,
            Letter = command.Letter,
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            CreatedBy = "Tester",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = command.ModifiedBy,
            ModifiedDate = DateTime.UtcNow
        };
        var mockDocumentSession = _mockingFramework.InitializeMockedClass<IDocumentSession>(new object[] { });
        var expected = "Foo";
        _mockingFramework.SetupReturnsResult(mockDocumentSession, x => x.LoadAsync<Models.Theme>(_mockingFramework.GetObject<Guid>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<Guid>(), _mockingFramework.GetObject<CancellationToken>() }, theme);

        _mockingFramework.SetupThrowsException(mockDocumentSession, x => x.SaveChangesAsync(_mockingFramework.GetObject<CancellationToken>()), new Exception(expected));

        var handler = new UpdateThemeHandler(mockDocumentSession);

        //Act/Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, new CancellationToken()));
    }

    #endregion

    #region UpdateThemeCommandValidation Tests

    [Fact]
    public async Task UpdateThemeCommandValidator_ValidateTheme_When_UpdateThemeCommand_Object_Called_Should_Return_Expected_Result()
    {
        //Arrange
        var command = new UpdateThemeCommand(Guid.NewGuid(), _faker.Random.String(20), 1, _faker.Random.String(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester");

        //Act
        var result = await _updateThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Number);
        result.ShouldNotHaveValidationErrorFor(x => x.Letter);
        result.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);
    }

    [Fact]
    public async Task UpdateThemeCommandValidator_ValidateTheme_When_UpdateThemeCommand_Object_Called_Should_Return_All_Validation_Exceptions()
    {
        //Arrange
        var command = new UpdateThemeCommand(Guid.Empty, null, -1, _faker.Random.String(1), new DateTime(), new DateTime(), null);

        //Act
        var result = await _updateThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Number);
        result.ShouldHaveValidationErrorFor(x => x.Letter);
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
        result.ShouldHaveValidationErrorFor(x => x.ModifiedBy);

    }

    [Fact]
    public async Task UpdateThemeCommandValidator_ValidateTheme_When_UpdateThemeCommand_Object_Called_Should_Return_Empty_Guid_Validation_Exception()
    {
        //Arrange
        var command = new UpdateThemeCommand(Guid.Empty, _faker.Random.String(20), 1, _faker.Random.String(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester");

        //Act
        var result = await _updateThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Number);
        result.ShouldNotHaveValidationErrorFor(x => x.Letter);
        result.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);

    }

    [Fact]
    public async Task UpdateThemeCommandValidator_ValidateTheme_When_UpdateThemeCommand_Object_Called_Should_Return_Name_Is_Not_Valid_Length()
    {
        //Arrange
        var command = new UpdateThemeCommand(Guid.NewGuid(), _faker.Random.String(1), 1, _faker.Random.String(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester");
        var command1 = new UpdateThemeCommand(Guid.NewGuid(), _faker.Random.String(151), 1, _faker.Random.String(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester");

        //Act
        var result = await _updateThemeCommandValidator.TestValidateAsync(command);
        var result1 = await _updateThemeCommandValidator.TestValidateAsync(command1);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Number);
        result.ShouldNotHaveValidationErrorFor(x => x.Letter);
        result.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);

        result1.ShouldNotHaveValidationErrorFor(x => x.Id);
        result1.ShouldHaveValidationErrorFor(x => x.Name);
        result1.ShouldNotHaveValidationErrorFor(x => x.Number);
        result1.ShouldNotHaveValidationErrorFor(x => x.Letter);
        result1.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result1.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);

    }

    [Fact]
    public async Task UpdateThemeCommandValidator_ValidateTheme_When_UpdateThemeCommand_Object_Called_Should_Return_Number_Validation()
    {
        //Arrange
        var command = new UpdateThemeCommand(Guid.NewGuid(), _faker.Random.String(20), -1, _faker.Random.String(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester");
        var command1 = new UpdateThemeCommand(Guid.NewGuid(), _faker.Random.String(20), 101, _faker.Random.String(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester");

        //Act
        var result = await _updateThemeCommandValidator.TestValidateAsync(command);
        var result1 = await _updateThemeCommandValidator.TestValidateAsync(command1);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Number);
        result.ShouldNotHaveValidationErrorFor(x => x.Letter);
        result.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);


        result1.ShouldNotHaveValidationErrorFor(x => x.Id);
        result1.ShouldHaveValidationErrorFor(x => x.Number);
        result1.ShouldNotHaveValidationErrorFor(x => x.Letter);
        result1.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result1.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);

    }

    [Fact]
    public async Task UpdateThemeCommandValidator_ValidateTheme_When_UpdateThemeCommand_Object_Called_Should_Return_Letter_Length_Validation()
    {
        //Arrange
        var command = new UpdateThemeCommand(Guid.NewGuid(), _faker.Random.String(20), 1, _faker.Random.String(1), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester");
        var command1 = new UpdateThemeCommand(Guid.NewGuid(), _faker.Random.String(20), 1, _faker.Random.String(3), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Tester");

        //Act
        var result = await _updateThemeCommandValidator.TestValidateAsync(command);
        var result1 = await _updateThemeCommandValidator.TestValidateAsync(command1);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Number);
        result.ShouldHaveValidationErrorFor(x => x.Letter);
        result.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);

        result1.ShouldNotHaveValidationErrorFor(x => x.Id);
        result1.ShouldNotHaveValidationErrorFor(x => x.Name);
        result1.ShouldNotHaveValidationErrorFor(x => x.Number);
        result1.ShouldHaveValidationErrorFor(x => x.Letter);
        result1.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result1.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);

    }

    [Fact]
    public async Task UpdateThemeCommandValidator_ValidateTheme_When_UpdateThemeCommand_Object_Called_Should_Return_StartDate_Validation()
    {
        //Arrange
        var command = new UpdateThemeCommand(Guid.NewGuid(), _faker.Random.String(20), 1, _faker.Random.String(2), new DateTime(), DateTime.UtcNow.AddDays(10), "Tester");

        //Act
        var result = await _updateThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Number);
        result.ShouldNotHaveValidationErrorFor(x => x.Letter);
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);


    }

    [Fact]
    public async Task UpdateThemeCommandValidator_ValidateTheme_When_UpdateThemeCommand_Object_Called_Should_Return_EndDate_Validation()
    {
        //Arrange
        var command = new UpdateThemeCommand(Guid.NewGuid(), _faker.Random.String(20), 1, _faker.Random.String(2), DateTime.UtcNow, new DateTime(), "Tester");

        //Act
        var result = await _updateThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Number);
        result.ShouldNotHaveValidationErrorFor(x => x.Letter);
        result.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
        result.ShouldNotHaveValidationErrorFor(x => x.ModifiedBy);

    }

    [Fact]
    public async Task UpdateThemeCommandValidator_ValidateTheme_When_UpdateThemeCommand_Object_Called_Should_Return_ModifiedBy_Validation()
    {
        //Arrange
        var command = new UpdateThemeCommand(Guid.NewGuid(), _faker.Random.String(20), 1, _faker.Random.String(2), DateTime.UtcNow, DateTime.UtcNow.AddDays(10), null);

        //Act
        var result = await _updateThemeCommandValidator.TestValidateAsync(command);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Number);
        result.ShouldNotHaveValidationErrorFor(x => x.Letter);
        result.ShouldNotHaveValidationErrorFor(x => x.StartDate);
        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        result.ShouldHaveValidationErrorFor(x => x.ModifiedBy);

    }

    #endregion

}
