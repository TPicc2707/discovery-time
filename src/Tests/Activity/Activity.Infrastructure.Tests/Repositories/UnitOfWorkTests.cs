using Microsoft.Data.SqlClient;

namespace Activity.Infrastructure.Tests.Repositories;

public class UnitOfWorkTests : IAsyncLifetime
{
    private readonly IApplicationDbContext _dbContext;
    private readonly DbContextOptions<ApplicationDbContext> _dbOptions;
    private readonly IMockNSubstituteMethods _mockingFramework;
    private readonly Faker _faker;
    private readonly SqliteConnection _connection;

    public UnitOfWorkTests()
    {
        _connection = new SqliteConnection("DataSource=UnitOfWorkDb");
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

    #region Complete

    [Fact]
    public async Task UnitOfWork_Complete_When_Called_Save_Changes()
    {
        //Arrange
        var expected = 1;
        var mockThemeLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockUnitOfWorkLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<UnitOfWork>>(new object[] { });
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<UnitOfWork>(new object[] { _dbContext, mockUnitOfWorkLoggingObject, mockThemeLoggingObject });
        mockUnitOfWork = _mockingFramework.SetupReturnsResult(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() }, expected);

        //Act
        var actual = await mockUnitOfWork.Complete(new CancellationToken());

        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task UnitOfWork_Complete_When_Called_Shoudl_Return_SqlException_Error()
    {
        //Arrange
        var mockThemeLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockUnitOfWorkLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<UnitOfWork>>(new object[] { });
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<UnitOfWork>(new object[] { _dbContext, mockUnitOfWorkLoggingObject, mockThemeLoggingObject });

        mockUnitOfWork = _mockingFramework.SetupThrowsException(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() }, CreateSqlException
            .MakeSqlException(@"Data Source=.;Database=GURANTEED_TO_FAIL2;Connection Timeout=1"));

        //Act
        Func<Task> act = () => mockUnitOfWork.Complete(new CancellationToken());

        //Assert
        var ex = await Assert.ThrowsAsync<SqlException>(act);
        Assert.IsType<SqlException>(ex);
    }

    [Fact]
    public async Task UnitOfWork_Complete_When_Called_Shoudl_Return_DbUpdateConncurrencyException_Error()
    {
        //Arrange
        var expected = "Foo";
        var mockThemeLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockUnitOfWorkLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<UnitOfWork>>(new object[] { });
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<UnitOfWork>(new object[] { _dbContext, mockUnitOfWorkLoggingObject, mockThemeLoggingObject });

        mockUnitOfWork = _mockingFramework.SetupThrowsException(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() }, new DbUpdateConcurrencyException(expected));

        //Act
        Func<Task> act = () => mockUnitOfWork.Complete(new CancellationToken());

        //Assert
        var ex = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(act);
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public async Task UnitOfWork_Complete_When_Called_Shoudl_Return_DbUpdateException_Error()
    {
        //Arrange
        var expected = "Foo";
        var mockThemeLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockUnitOfWorkLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<UnitOfWork>>(new object[] { });
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<UnitOfWork>(new object[] { _dbContext, mockUnitOfWorkLoggingObject, mockThemeLoggingObject });

        mockUnitOfWork = _mockingFramework.SetupThrowsException(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() }, new DbUpdateException(expected));

        //Act
        Func<Task> act = () => mockUnitOfWork.Complete(new CancellationToken());

        //Assert
        var ex = await Assert.ThrowsAsync<DbUpdateException>(act);
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public async Task UnitOfWork_Complete_When_Called_Shoudl_Return_Exception_Error()
    {
        //Arrange
        var expected = "Foo";
        var mockThemeLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<Domain.Models.Theme>>(new object[] { });
        var mockUnitOfWorkLoggingObject = _mockingFramework.InitializeMockedClass<ILogger<UnitOfWork>>(new object[] { });
        var mockUnitOfWork = _mockingFramework.InitializeMockedClass<UnitOfWork>(new object[] { _dbContext, mockUnitOfWorkLoggingObject, mockThemeLoggingObject });

        mockUnitOfWork = _mockingFramework.SetupThrowsException(mockUnitOfWork, x => x.Complete(_mockingFramework.GetObject<CancellationToken>()), new object[] { _mockingFramework.GetObject<CancellationToken>() }, new Exception(expected));

        //Act
        Func<Task> act = () => mockUnitOfWork.Complete(new CancellationToken());

        //Assert
        var ex = await Assert.ThrowsAsync<Exception>(act);
        Assert.Equal(expected, ex.Message);
    }

    #endregion
}
