using Activity.Application.Data;
using Activity.Domain.Models;
using Activity.Domain.ValueObjects;
using Activity.Infrastructure.Data;
using Activity.Infrastructure.Repositories;
using Bogus;
using Discovery.Time.Tests.Data.MockData;
using Discovery.Time.Tests.Data.TestingFramework;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Activity.Infrastructure.Tests.Repositories;

public class ThemeRepositoryTests : IAsyncLifetime
{
    private readonly IApplicationDbContext _dbContext;
    private readonly DbContextOptions<ApplicationDbContext> _dbOptions;
    private readonly IMockNSubstituteMethods _mockingFramework;
    private readonly Faker _faker;
    private readonly SqliteConnection _connection;

    public ThemeRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=ThemeRepoDb");
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
    public void ThemeRepository_Constructor_WhenInitiated_ShouldReturnArgumentNullException()
    {
        //Arrange/Act/Assert
        Assert.Throws<ArgumentNullException>(() => new ThemeRepository(null, null));
    }

    [Fact]
    public void ThemeRepository_Constructor_WhenInitiated_ShouldReturnBaseRepositoryType()
    {
        //Arrange
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var repository = new ThemeRepository(_dbContext, mockLoggingObject);

        //Act/Assert
        Assert.IsType<ThemeRepository>(repository);
    }

    #endregion

    #region GetAllThemeActivitiesAsync

    [Fact]
    public async Task ThemeRepository_GetAllThemeActivitiesAsync_WhenCalled_ShouldReturn_ExpectedResult()
    {
        //Arrange
        int pageIndex = 0;
        int pageSize = 10;
        var expected = _dbContext.Activities.Skip(pageSize * pageIndex).Take(pageSize).Count();
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new ThemeRepository(_dbContext, mockLoggingObject);

        //Act
        var result = await repository.GetAllThemeActivitiesAsync(pageIndex, pageSize);
        var actual = result.Count;

        //Assert
        Assert.Equal(expected, actual);
        Assert.IsAssignableFrom<IReadOnlyList<Domain.Models.Activity>>(result);
    }

    [Fact]
    public async Task ThemeRepository_GetAllThemeActivitiesAsync_WhenCalled_ShouldReturn_NoResult()
    {
        //Arrange
        int pageIndex = 0;
        int pageSize = 10;
        _dbContext.Activities.RemoveRange(_dbContext.Activities.ToList());
        await _dbContext.SaveChangesAsync(new CancellationToken());
        var expected = 0;
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new ThemeRepository(_dbContext, mockLoggingObject);

        //Act
        var result = await repository.GetAllThemeActivitiesAsync(pageIndex, pageSize);
        var actual = result.Count;

        //Assert
        Assert.Equal(expected, actual);
        Assert.IsAssignableFrom<IReadOnlyList<Domain.Models.Activity>>(result);

    }

    [Fact]
    public async Task ThemeRepository_GetAllThemeActivitiesAsync_WhenCalled_ShouldReturn_Exception_Error()
    {
        //Arrange
        int pageIndex = 0;
        int pageSize = 10;
        var expected = "Foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockObject = _mockingFramework.InitializeMockedClass<ThemeRepository>(new object[] { _dbContext, mockLoggingObject });

        mockObject = _mockingFramework.SetupThrowsException(mockObject, x => x.GetAllThemeActivitiesAsync(_mockingFramework.GetObject<int>(), _mockingFramework.GetObject<int>()), new object[] { _mockingFramework.GetObject<int>(), _mockingFramework.GetObject<int>() },  new Exception(expected));

        //Act
        Func<Task> act = () => mockObject.GetAllThemeActivitiesAsync(pageIndex, pageSize);
        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);

    }

    #endregion

    #region GetThemeActivityByIdAsync

    [Fact]
    public async Task ThemeRepository_GetThemeActivityByIdAsync_WhenCalled_ShouldReturn_ExpectedResult()
    {
        //Arrange
        var activity = _dbContext.Activities.FirstOrDefault();
        var expectedId = activity.Id;
        var expectedName = activity.Name;
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new ThemeRepository(_dbContext, mockLoggingObject);

        //Act
        var result = await repository.GetThemeActivityByIdAsync(expectedId, new CancellationToken());
        var actualId = result.Id;
        var actualName = result.Name;

        //Assert
        Assert.Equal(expectedId, actualId);
        Assert.Equal(expectedName, actualName);
        Assert.IsType<Domain.Models.Activity>(result);

    }

    [Fact]
    public async Task ThemeRepository_GetThemeActivityByIdAsync_WhenCalled_ShouldReturn_NoResult()
    {
        //Arrange
        var noResultId = ActivityId.Of(Guid.NewGuid());
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new ThemeRepository(_dbContext, mockLoggingObject);

        //Act
        var result = await repository.GetThemeActivityByIdAsync(noResultId, new CancellationToken());

        //Assert
        Assert.Null(result);

    }

    [Fact]
    public async Task ThemeRepository_GetThemeActivityByIdAsync_WhenCalled_ShouldReturn_FormatException_Error()
    {
        //Arrange
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new ThemeRepository(_dbContext, mockLoggingObject);

        //Act/Assert
        await Assert.ThrowsAsync<FormatException>(() => repository.GetThemeActivityByIdAsync(ActivityId.Of(new Guid("")), new CancellationToken()));

    }

    [Fact]
    public async Task PersonRepository_GetPersonAddressByIdAsync_WhenCalled_ShouldReturn_Exception_Error()
    {
        //Arrange
        var expected = "Foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockObject = _mockingFramework.InitializeMockedClass<ThemeRepository>(new object[] { _dbContext, mockLoggingObject });

        mockObject = _mockingFramework.SetupThrowsException(mockObject, x => x.GetThemeActivityByIdAsync(_mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ActivityId>(), _mockingFramework.GetObject<CancellationToken>() }, new Exception(expected));

        //Act
        Func<Task> act = () => mockObject.GetThemeActivityByIdAsync(ActivityId.Of(Guid.NewGuid()), new CancellationToken());

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);

    }

    #endregion

    #region GetAllThemeActivitiesByThemeIdAsync

    [Fact]
    public async Task ThemeRepository_GetAllThemeActivitiesByThemeIdAsync_WhenCalled_ShouldReturn_ExpectedResult()
    {
        //Arrange
        var theme = _dbContext.Themes.FirstOrDefault();
        var expectedThemeId = theme.Id;
        var activities = _dbContext.Activities.Where(x => x.ThemeId == theme.Id);
        var expected = activities.Count();
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new ThemeRepository(_dbContext, mockLoggingObject);

        //Act
        var result = await repository.GetAllThemeActivitiesByThemeIdAsync(theme.Id, new CancellationToken());
        var actual = result.Count;

        //Assert
        Assert.Equal(expected, actual);
        foreach (var activity in activities)
            Assert.Equal(expectedThemeId, activity.ThemeId);
        Assert.IsType<List<Domain.Models.Activity>>(result);

    }

    [Fact]
    public async Task ThemeRepository_GetAllThemeActivitiesByThemeIdAsync_WhenCalled_ShouldReturn_NoResult()
    {
        //Arrange
        var theme = _dbContext.Themes.FirstOrDefault();
        _dbContext.Activities.RemoveRange(_dbContext.Activities.ToList());
        await _dbContext.SaveChangesAsync(new CancellationToken());

        var expected = 0;
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new ThemeRepository(_dbContext, mockLoggingObject);

        //Act
        var result = await repository.GetAllThemeActivitiesByThemeIdAsync(theme.Id, new CancellationToken());
        var actual = result.Count;

        //Assert
        Assert.Equal(expected, actual);
        Assert.IsAssignableFrom<IReadOnlyList<Domain.Models.Activity>>(result);

    }

    [Fact]
    public async Task ThemeRepository_GetAllThemeActivitiesByThemeIdAsync_WhenCalled_ShouldReturn_FormatException_Error()
    {
        //Arrange
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new ThemeRepository(_dbContext, mockLoggingObject);

        //Act/Assert
        await Assert.ThrowsAsync<FormatException>(() => repository.GetAllThemeActivitiesByThemeIdAsync(ThemeId.Of(new Guid("")), new CancellationToken()));

    }

    [Fact]
    public async Task ThemeRepository_GetAllThemeActivitiesByThemeIdAsync_WhenCalled_ShouldReturn_Exception_Error()
    {
        //Arrange
        var expected = "Foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockObject = _mockingFramework.InitializeMockedClass<ThemeRepository>(new object[] { _dbContext, mockLoggingObject });

        mockObject = _mockingFramework.SetupThrowsException(mockObject, x => x.GetAllThemeActivitiesByThemeIdAsync(_mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ThemeId>(), _mockingFramework.GetObject<CancellationToken>() }, new Exception(expected));

        //Act
        Func<Task> act = () => mockObject.GetAllThemeActivitiesByThemeIdAsync(ThemeId.Of(Guid.NewGuid()), new CancellationToken());

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);

    }

    #endregion

    #region GetAllThemeActivitiesByNameAsync

    [Fact]
    public async Task ThemeRepository_GetAllThemeActivitiesByNameAsync_WhenCalled_ShouldReturn_ExpectedResult()
    {
        //Arrange
        var nameToSearch = _faker.Random.String(1).ToLower();
        var expected = _dbContext.Activities.Where(x => x.Name.Value.ToLower().Contains(nameToSearch.ToLower())).Count();
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new ThemeRepository(_dbContext, mockLoggingObject);

        //Act
        var result = await repository.GetAllThemeActivitiesByNameAsync(ActivityName.Of(nameToSearch), new CancellationToken());
        var actual = result.Count;

        //Assert
        Assert.Equal(expected, actual);
        Assert.IsAssignableFrom<IReadOnlyList<Domain.Models.Activity>>(result);

    }

    [Fact]
    public async Task ThemeRepository_GetAllThemeActivitiesByNameAsync_WhenCalled_ShouldReturn_NoResult()
    {
        //Arrange
        var nameToSearch = _faker.Random.String(1).ToLower();
        _dbContext.Activities.RemoveRange(_dbContext.Activities.ToList());
        await _dbContext.SaveChangesAsync(new CancellationToken());

        var expected = 0;
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new ThemeRepository(_dbContext, mockLoggingObject);

        //Act
        var result = await repository.GetAllThemeActivitiesByNameAsync(ActivityName.Of(nameToSearch), new CancellationToken());
        var actual = result.Count;

        //Assert
        Assert.Equal(expected, actual);
        Assert.IsAssignableFrom<IReadOnlyList<Domain.Models.Activity>>(result);

    }

    [Fact]
    public async Task ThemeRepository_GetAllThemeActivitiesByNameAsync_WhenCalled_ShouldReturn_Exception_Error()
    {
        //Arrange
        var nameToSearch = _faker.Random.String(1).ToLower();
        var expected = "Foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockObject = _mockingFramework.InitializeMockedClass<ThemeRepository>(new object[] { _dbContext, mockLoggingObject });

        mockObject = _mockingFramework.SetupThrowsException(mockObject, x => x.GetAllThemeActivitiesByNameAsync(_mockingFramework.GetObject<ActivityName>(), _mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<ActivityName>(), _mockingFramework.GetObject<CancellationToken>() }, new Exception(expected));

        //Act
        Func<Task> act = () => mockObject.GetAllThemeActivitiesByNameAsync(ActivityName.Of(nameToSearch), new CancellationToken());

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);

    }

    #endregion

    #region GetThemeActivitiesLongCountAsync

    [Fact]
    public async Task ThemeRepository_GetThemeActivitiesLongCountAsync_WhenCalled_ShouldReturn_ExpectedResult()
    {
        //Arrange
        var expected = _dbContext.Activities.LongCount();
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new ThemeRepository(_dbContext, mockLoggingObject);

        //Act
        var result = await repository.GetThemeActivitiesLongCountAsync(new CancellationToken());

        //Assert
        Assert.Equal(expected, result);
        Assert.IsType<long>(result);
    }

    [Fact]
    public async Task ThemeRepository_GetThemeActivitiesLongCountAsync_WhenCalled_ShouldReturn_Exception_Error()
    {
        //Arrange
        var expected = "Foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockObject = _mockingFramework.InitializeMockedClass<ThemeRepository>(new object[] { _dbContext, mockLoggingObject });

        mockObject = _mockingFramework.SetupThrowsException(mockObject, x => x.GetThemeActivitiesLongCountAsync(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() }, new Exception(expected));

        //Act
        Func<Task> act = () => mockObject.GetThemeActivitiesLongCountAsync(new CancellationToken());

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);

    }
    #endregion

    #region CreateThemeActivityAsync

    [Fact]
    public async Task ThemeRepository_CreateThemeActivityAsync_WhenCalled_ShouldReturn_ExpectedResult()
    {
        //Arrange
        var themeIds = _dbContext.Themes.Select(x => x.Id.Value).ToList();
        var testData = ThemeFakeData.GenerateActivities(themeIds, _faker).First();
        var expectedCount = _dbContext.Activities.Count() + 1;
        var expectedId = testData.Id;
        var expectedName = testData.Name;
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new ThemeRepository(_dbContext, mockLoggingObject);

        //Act
        var result = await repository.CreateThemeActivityAsync(testData);
        await _dbContext.SaveChangesAsync(new CancellationToken());

        var actualId = result.Id;
        var actualName = result.Name;
        var actualCount = _dbContext.Activities.Count();

        //Assert
        Assert.Equal(expectedCount, actualCount);
        Assert.Equal(expectedId, actualId);
        Assert.Equal(expectedName, actualName);
        Assert.IsType<Domain.Models.Activity>(result);

    }

    [Fact]
    public async Task ThemeRepository_CreateThemeActivityAsync_WhenCalled_ShouldReturn_InvalidOperationException_Error()
    {
        //Arrange
        var themeIds = _dbContext.Themes.Select(x => x.Id.Value).ToList();
        var testData = ThemeFakeData.GenerateActivities(themeIds, _faker).First();

        var expected = "Foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockObject = _mockingFramework.InitializeMockedClass<ThemeRepository>(new object[] { _dbContext, mockLoggingObject });

        mockObject = _mockingFramework.SetupThrowsException(mockObject, x => x.CreateThemeActivityAsync(_mockingFramework.GetObject<Domain.Models.Activity>()), new InvalidOperationException(expected));

        //Act
        Func<Task> act = () => mockObject.CreateThemeActivityAsync(testData);

        //Assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Contains(expected, ex.Message);

    }

    [Fact]
    public async Task ThemeRepository_CreateThemeActivityAsync_WhenCalled_ShouldReturn_NullReference_Error()
    {
        //Arrange
        var themeIds = _dbContext.Themes.Select(x => x.Id.Value).ToList();
        var testData = ThemeFakeData.GenerateActivities(themeIds, _faker).First();

        var expected = "Foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockObject = _mockingFramework.InitializeMockedClass<ThemeRepository>(new object[] { _dbContext, mockLoggingObject });

        mockObject = _mockingFramework.SetupThrowsException(mockObject, x => x.CreateThemeActivityAsync(_mockingFramework.GetObject<Domain.Models.Activity>()), new NullReferenceException(expected));

        //Act
        Func<Task> act = () => mockObject.CreateThemeActivityAsync(testData);

        //Assert
        NullReferenceException ex = await Assert.ThrowsAsync<NullReferenceException>(act);
        Assert.Contains(expected, ex.Message);

    }

    [Fact]
    public async Task ThemeRepository_CreateThemeActivityAsync_WhenCalled_ShouldReturn_Exception_Error()
    {
        //Arrange
        var themeIds = _dbContext.Themes.Select(x => x.Id.Value).ToList();
        var testData = ThemeFakeData.GenerateActivities(themeIds, _faker).First();

        var expected = "Foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockObject = _mockingFramework.InitializeMockedClass<ThemeRepository>(new object[] { _dbContext, mockLoggingObject });

        mockObject = _mockingFramework.SetupThrowsException(mockObject, x => x.CreateThemeActivityAsync(_mockingFramework.GetObject<Domain.Models.Activity>()), new Exception(expected));

        //Act
        Func<Task> act = () => mockObject.CreateThemeActivityAsync(testData);

        //Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains(expected, ex.Message);
    }

    #endregion

    #region UpdateThemeActivity

    [Fact]
    public async Task ThemeRepository_UpdateThemeActivity_WhenCalled_ShouldReturn_ExpectedResult()
    {
        //Arrange
        var testData = _dbContext.Activities.FirstOrDefault();
        testData.Update(testData.ThemeId, ActivityName.Of("Update Activity Name"), ActivityDetails.Of(testData.Details.Description, testData.Details.Url, testData.Details.Date));
        var expectedName = testData.Name;
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new ThemeRepository(_dbContext, mockLoggingObject);


        //Act
        repository.UpdateThemeActivity(testData);
        await _dbContext.SaveChangesAsync(new CancellationToken());

        var updatedResult = await repository.GetThemeActivityByIdAsync(testData.Id, new CancellationToken());

        var actualName = updatedResult.Name;

        //Assert
        Assert.Equal(expectedName, actualName);
        Assert.IsType<Domain.Models.Activity>(updatedResult);
    }

    [Fact]
    public async Task ThemeRepository_UpdateThemeActivity_WhenCalled_ShouldReturn_InvalidOperationException_Error()
    {
        //Arrange
        var expected = "Foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockObject = _mockingFramework.InitializeMockedClass<ThemeRepository>(new object[] { _dbContext, mockLoggingObject });

        var testData = _dbContext.Activities.FirstOrDefault();
        mockObject = _mockingFramework.SetupThrowsException(mockObject, x => x.UpdateThemeActivity(_mockingFramework.GetObject<Domain.Models.Activity>()), new InvalidOperationException(expected));

        //Act
        Action act = () => mockObject.UpdateThemeActivity(testData);

        //Assert
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(act);
        Assert.Contains(expected, ex.Message);
    }

    [Fact]
    public async Task ThemeRepository_UpdateThemeActivity_WhenCalled_ShouldReturn_NullReference_Error()
    {
        //Arrange

        var expected = "Foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockObject = _mockingFramework.InitializeMockedClass<ThemeRepository>(new object[] { _dbContext, mockLoggingObject });

        var testData = _dbContext.Activities.FirstOrDefault();
        mockObject = _mockingFramework.SetupThrowsException(mockObject, x => x.UpdateThemeActivity(_mockingFramework.GetObject<Domain.Models.Activity>()), new NullReferenceException(expected));

        //Act
        Action act = () => mockObject.UpdateThemeActivity(testData);

        //Assert
        NullReferenceException ex = Assert.Throws<NullReferenceException>(act);
        Assert.Contains(expected, ex.Message);
    }

    [Fact]
    public async Task ThemeRepository_UpdateThemeActivity_WhenCalled_ShouldReturn_Exception_Error()
    {
        //Arrange

        var expected = "Foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockObject = _mockingFramework.InitializeMockedClass<ThemeRepository>(new object[] { _dbContext, mockLoggingObject });

        var testData = _dbContext.Activities.FirstOrDefault();
        mockObject = _mockingFramework.SetupThrowsException(mockObject, x => x.UpdateThemeActivity(_mockingFramework.GetObject<Domain.Models.Activity>()), new Exception(expected));

        //Act
        Action act = () => mockObject.UpdateThemeActivity(testData);

        //Assert
        Exception ex = Assert.Throws<Exception>(act);
        Assert.Contains(expected, ex.Message);
    }

    #endregion

    #region DeleteThemeActivity

    [Fact]
    public async Task ThemeRepository_DeleteThemeActivity_WhenCalled_ShouldReturn_ExpectedResult()
    {
        //Arrange
        var expected = _dbContext.Activities.ToList().Count - 1;
        var testData = _dbContext.Activities.FirstOrDefault();
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });

        var repository = new ThemeRepository(_dbContext, mockLoggingObject);

        //Act
        repository.DeleteThemeActivity(testData);
        await _dbContext.SaveChangesAsync(new CancellationToken());

        var actual = _dbContext.Activities.ToList().Count;

        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task ThemeRepository_DeleteThemeActivity_WhenCalled_ShouldReturn_Exception_Error()
    {
        //Arrange
        var expected = "Foo";
        var mockLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockObject = _mockingFramework.InitializeMockedClass<ThemeRepository>(new object[] { _dbContext, mockLoggingObject });

        var testData = _dbContext.Activities.FirstOrDefault();
        mockObject = _mockingFramework.SetupThrowsException(mockObject, x => x.DeleteThemeActivity(_mockingFramework.GetObject<Domain.Models.Activity>()), new Exception(expected));

        //Act
        Action act = () => mockObject.DeleteThemeActivity(testData);

        //Assert
        Exception ex = Assert.Throws<Exception>(act);
        Assert.Contains(expected, ex.Message);
    }

    #endregion
}
