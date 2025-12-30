namespace Activity.Infrastructure.Tests.Repositories;

public class BaseRepositoryTests : IAsyncLifetime
{
    private readonly IApplicationDbContext _dbContext;
    private readonly DbContextOptions<ApplicationDbContext> _dbOptions;
    private readonly IMockNSubstituteMethods _mockingFramework;
    private readonly Faker _faker;
    private readonly SqliteConnection _connection;

    public BaseRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=BaseRepoDb");
        _connection.Open();
        _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(_connection).Options;
        _mockingFramework = Helper.GetRequiredService<IMockNSubstituteMethods>() ?? throw new ArgumentNullException(nameof(IMockNSubstituteMethods));
        _dbContext = new ApplicationDbContext(_dbOptions);
        _faker = new Faker();
        if (_dbContext is DbContext dbContext)
            dbContext.Database.EnsureCreated();
    }

    public async Task InitializeAsync()
    {
        await SetupSqliteDb.SetupDb(_dbContext, _faker);
    }

    public async Task DisposeAsync()
    {
        _connection.Close();
        await SetupSqliteDb.DeleteData(_dbContext);
    }

    #region Constructor

    [Fact]
    public void BaseRepository_Constructor_WhenInitiated_ShouldReturnArgumentNullException()
    {
        //Arrange/Act/Assert
        Assert.Throws<ArgumentNullException>(() => new BaseRepository<Domain.Models.Theme>(null, null));
    }

    [Fact]
    public void BaseRepository_Constructor_WhenInitiated_ShouldReturnBaseRepositoryType()
    {
        //Arrange
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var repository = new BaseRepository<Domain.Models.Theme>(_dbContext, mockLoggingObject);

        //Act/Assert
        Assert.IsType<BaseRepository<Domain.Models.Theme>>(repository);
    }

    #endregion

    #region Add

    [Fact]
    public async Task BaseRepository_Add_WhenCalled_ShouldReturnExpectedResult()
    {
        //Arrange

        var expectedCount = _dbContext.Themes.Count() + 1;

        var testData = ThemeFakeData.GenerateActivityThemes(1).First();
        var expected = testData.Id;
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var repository = new BaseRepository<Domain.Models.Theme>(_dbContext, mockLoggingObject);

        //Act
        var result = await repository.AddAsync(testData);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var actual = result.Id;
        var actualCount = _dbContext.Themes.Count();

        //Assert
        Assert.Equal(expected, actual);
        Assert.Equal(expectedCount, actualCount);
        Assert.IsType<Domain.Models.Theme>(result);

    }

    [Fact]
    public async Task BaseRepository_Add_WhenCalled_ShouldReturn_InvalidOperationException_Error()
    {
        //Arrange
        var testData = ThemeFakeData.GenerateActivityThemes(1).FirstOrDefault();

        var expected = "Foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockObject = _mockingFramework.InitializeMockedClass<BaseRepository<Domain.Models.Theme>>(new object[] { _dbContext, mockLoggingObject });

        mockObject = _mockingFramework.SetupThrowsException(mockObject, x => x.AddAsync(_mockingFramework.GetObject<Domain.Models.Theme>()), new InvalidOperationException(expected));

        //Act
        Func<Task> act = () => mockObject.AddAsync(testData);

        //Assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Contains(expected, ex.Message);

    }

    [Fact]
    public async Task BaseRepository_Add_WhenCalled_ShouldReturn_Exception_Error()
    {
        //Arrange
        var testData = ThemeFakeData.GenerateActivityThemes(1).First();

        var expected = "Foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockObject = _mockingFramework.InitializeMockedClass<BaseRepository<Domain.Models.Theme>>(new object[] { _dbContext, mockLoggingObject });

        mockObject = _mockingFramework.SetupThrowsException(mockObject, x => x.AddAsync(_mockingFramework.GetObject<Domain.Models.Theme>()), new Exception(expected));
        //Act
        Func<Task> act = () => mockObject.AddAsync(testData);

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);

    }

    #endregion

    #region Delete

    [Fact]
    public async Task BaseRepository_Delete_WhenCalled_ShouldReturn_ExpectedResult()
    {
        //Arrange
        var testData = ThemeFakeData.GenerateActivityThemes(1).First();

        if (_dbContext is DbContext dbContext)
        {
            await _dbContext.Themes.AddAsync(testData);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
        }

        var expectedCount = _dbContext.Themes.Count() - 1;
        var theme = _dbContext.Themes.Find(testData.Id);
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new BaseRepository<Domain.Models.Theme>(_dbContext, mockLoggingObject);

        //Act
        repository.Delete(theme);
        await _dbContext.SaveChangesAsync(new CancellationToken());

        var result = _dbContext.Themes.FirstOrDefault(x => x.Id == theme.Id);
        var actualCount = _dbContext.Themes.Count();

        //Assert
        Assert.Null(result);
        Assert.Equal(expectedCount, actualCount);

    }

    [Fact]
    public async Task BaseRepository_Delete_WhenCalled_ShouldReturn_Exception_Error()
    {
        //Arrange

        var expected = "Foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockObject = _mockingFramework.InitializeMockedClass<BaseRepository<Domain.Models.Theme>>(new object[] { _dbContext, mockLoggingObject });

        var theme = _dbContext.Themes.FirstOrDefault();

        mockObject = _mockingFramework.SetupThrowsException(mockObject, x => x.Delete(_mockingFramework.GetObject<Domain.Models.Theme>()), new Exception(expected));

        //Act
        Action act = () => mockObject.Delete(theme);

        //Assert
        Exception ex = Assert.Throws<Exception>(act);
        Assert.Equal(expected, ex.Message);

    }

    #endregion

    #region GetAllAsync

    [Fact]
    public async Task BaseRepository_GetAllAsync_WhenCalled_ShouldReturn_ExpectedResult()
    {
        //Arrange
        var expected = _dbContext.Themes.Count();
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new BaseRepository<Domain.Models.Theme>(_dbContext, mockLoggingObject);

        //Act
        var result = await repository.GetAllAsync();
        var actual = result.Count;

        //Assert
        Assert.Equal(expected, actual);
        Assert.IsAssignableFrom<IReadOnlyList<Domain.Models.Theme>>(result);

    }

    [Fact]
    public async Task BaseRepository_GetAllAsync_WhenCalled_ShouldReturn_NoResult()
    {
        //Arrange
        _dbContext.Themes.RemoveRange(_dbContext.Themes.ToList());
        await _dbContext.SaveChangesAsync(new CancellationToken());
        var expected = 0;
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new BaseRepository<Domain.Models.Theme>(_dbContext, mockLoggingObject);

        //Act
        var result = await repository.GetAllAsync();
        var actual = result.Count;

        //Assert
        Assert.Equal(expected, actual);
        Assert.IsAssignableFrom<IReadOnlyList<Domain.Models.Theme>>(result);
    }

    [Fact]
    public async Task BaseRepository_GetAllAsync_WhenCalled_ShouldReturn_Exception_Error()
    {
        //Arrange
        var expected = "Foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockObject = _mockingFramework.InitializeMockedClass<BaseRepository<Domain.Models.Theme>>(new object[] { _dbContext, mockLoggingObject });

        mockObject = _mockingFramework.SetupThrowsException(mockObject, l => l.GetAllAsync(), new Exception(expected));

        //Act
        Func<Task> act = () => mockObject.GetAllAsync();

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);

    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task BaseRepository_GetByIdAsync_WhenCalled_ShouldReturn_ExpectedResult()
    {
        //Arrange
        var testData = _dbContext.Themes.First();
        var expectedId = testData.Id;
        var expectedName = testData.Name;
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new BaseRepository<Domain.Models.Theme>(_dbContext, mockLoggingObject);

        //Act
        var result = await repository.GetByIdAsync(expectedId, new CancellationToken());
        var actualId = result.Id;
        var actualName = result.Name;

        //Assert
        Assert.Equal(expectedId, actualId);
        Assert.Equal(expectedName, actualName);
        Assert.IsType<Domain.Models.Theme>(result);

    }

    [Fact]
    public async Task BaseRepository_GetByIdAsync_WhenCalled_ShouldReturn_NoResult()
    {
        //Arrange
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new BaseRepository<Domain.Models.Theme>(_dbContext, mockLoggingObject);

        //Act
        var result = await repository.GetByIdAsync(ThemeId.Of(Guid.NewGuid()), new CancellationToken());

        //Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task BaseRepository_GetByIdAsync_WhenCalled_ShouldReturn_FormatException_Error()
    {
        //Arrange
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var repository = new BaseRepository<Domain.Models.Theme>(_dbContext, mockLoggingObject);

        //Act/Assert
        await Assert.ThrowsAsync<FormatException>(() => repository.GetByIdAsync(ThemeId.Of(new Guid("")), new CancellationToken()));

    }

    [Fact]
    public async Task BaseRepository_GetByIdAsync_WhenCalled_ShouldReturn_Exception_Error()
    {
        //Arrange
        var expected = "Foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockObject = _mockingFramework.InitializeMockedClass<BaseRepository<Domain.Models.Theme>>(new object[] { _dbContext, mockLoggingObject });

        mockObject = _mockingFramework.SetupThrowsException(mockObject, x => x.GetByIdAsync<ThemeId>(_mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>()), new Exception(expected));

        //Act
        Func<Task> act = () => mockObject.GetByIdAsync(ThemeId.Of(Guid.NewGuid()), new CancellationToken());

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);
    }

    #endregion

    #region Update

    [Fact]
    public async Task BaseRepository_Update_WhenCalled_ShouldReturn_ExpectedResult()
    {
        //Arrange
        var testData = ThemeFakeData.GenerateActivityThemes(1).FirstOrDefault();

        if (_dbContext is DbContext dbContext)
        {
            await _dbContext.Themes.AddAsync(testData);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
        }

        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new BaseRepository<Domain.Models.Theme>(_dbContext, mockLoggingObject);

        //Act
        var expectedId = testData.Id;
        var expectedName = testData.Name;

        repository.Update(testData);
        await _dbContext.SaveChangesAsync(new CancellationToken());

        var updatedResult = await repository.GetByIdAsync(testData.Id, new CancellationToken());

        var actualId = updatedResult.Id;
        var actualName = updatedResult.Name;

        //Assert
        Assert.Equal(expectedId, actualId);
        Assert.Equal(expectedName, actualName);
        Assert.IsType<Domain.Models.Theme>(updatedResult);

    }
    [Fact]
    public async Task BaseRepository_Update_WhenCalled_ShouldReturn_Exception_Error()
    {
        //Arrange
        var testData = ThemeFakeData.GenerateActivityThemes(1).First();
        var expected = "Foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockObject = _mockingFramework.InitializeMockedClass<BaseRepository<Domain.Models.Theme>>(new object[] { _dbContext, mockLoggingObject });

        mockObject = _mockingFramework.SetupThrowsException(mockObject, x => x.Update(_mockingFramework.GetObject<Domain.Models.Theme>()), new Exception(expected));

        //Act
        Action act = () => mockObject.Update(testData);

        //Assert
        Exception ex = Assert.Throws<Exception>(act);
        Assert.Contains(expected, ex.Message);
    }


    #endregion
}
