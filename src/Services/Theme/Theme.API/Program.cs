var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddServiceDefaults();
builder.AddNpgsqlDataSource("themeDb");

var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddCarter();

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("themeDb")!);
}).UseLightweightSessions();

if (builder.Environment.IsDevelopment())
    builder.Services.InitializeMartenWith<ThemeInitialData>();

builder.Services.AddMessageBroker(Assembly.GetExecutingAssembly());

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    builder.WithOrigins("https://localhost:6060")
    .AllowAnyHeader()
    .AllowAnyMethod());
});

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("themeDb")!);

//builder.Services.AddCustomAuthentication();

//builder.Services.AddKeycloakPolicies(ServiceName.LeagueService);


// Configure the HTTP request pipeline.
var app = builder.Build();

app.MapDefaultEndpoints();

//app.UseAuthentication();

//app.UseAuthorization();

app.MapCarter();

app.UseCors();

app.UseExceptionHandler(options => { });

app.MapHealthChecks("/healthz",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.Run();
