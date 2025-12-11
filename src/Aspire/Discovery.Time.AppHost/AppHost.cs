var builder = DistributedApplication.CreateBuilder(args);

//Create Services

var postgres = builder
        .AddPostgres("postgres", port: 5432)
        .WithPgAdmin()
        .WithDataVolume()
        .WithLifetime(ContainerLifetime.Persistent);

var rabbitmq = builder
      .AddRabbitMQ("rabbitmq")
      .WithManagementPlugin()
      .WithDataVolume()
      .WithLifetime(ContainerLifetime.Persistent);

var password = builder.AddParameter("SqlServerSaPassword", secret: true);

var sql = builder.AddSqlServer("sql", password)
          .WithDataVolume()
          .WithLifetime(ContainerLifetime.Persistent);


//Postgres
var themeDb = postgres.AddDatabase("themeDb");

//Sql
var activityDb = sql.AddDatabase("activityDb");

//APIs
var theme = builder
            .AddProject<Projects.Theme_API>("theme-api",
            configure: static project =>
            {
                project.ExcludeLaunchProfile = true;
                project.ExcludeKestrelEndpoints = false;
            })
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
            .WithHttpEndpoint(port: 6000)
            .WithHttpsEndpoint(port: 6060)
            .WithReference(themeDb)
            .WithReference(rabbitmq)
            //.WithReference(keycloak)
            .WaitFor(themeDb)
            .WaitFor(rabbitmq);
            //.WaitFor(keycloak);

var activity = builder
            .AddProject<Projects.Activity_API>("activity-api",
            configure: static project =>
            {
                project.ExcludeLaunchProfile = true;
                project.ExcludeKestrelEndpoints = false;
            })
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
            .WithHttpEndpoint(port: 6001)
            .WithHttpsEndpoint(port: 6061)
            .WithReference(activityDb)
            .WithReference(rabbitmq)
            //.WithReference(keycloak)
            .WaitFor(activityDb)
            .WaitFor(rabbitmq);
            //.WaitFor(keycloak);

builder.Build().Run();
