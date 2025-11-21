var builder = DistributedApplication.CreateBuilder(args);

//Create Services

var postgres = builder
        .AddPostgres("postgres", port: 5432)
        .WithPgAdmin()
        .WithDataVolume()
        .WithLifetime(ContainerLifetime.Persistent);


//Postgres
var themeDb = postgres.AddDatabase("themeDb");


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
            //.WithReference(rabbitmq)
            //.WithReference(keycloak)
            .WaitFor(themeDb);
            //.WaitFor(rabbitmq)
            //.WaitFor(keycloak);

builder.Build().Run();
