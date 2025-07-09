var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
    .WithRedisInsight()
    .WithLifetime(ContainerLifetime.Persistent);

//Add SQL
var dbPassword = builder.AddParameter("DbPassword");
var sqlServer = builder.AddSqlServer("sqlserver", dbPassword, port:1433)
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("MSSQL_SA_PASSWORD", dbPassword)
    .WithVolume("sql-data", "/var/opt/mssql")
    .AddDatabase("masterdatadb");

var masterDataService = builder.AddProject<Projects.AspireApp_MasterDataService>("masterdataservice")
    .WithReference(sqlServer);
    //.WithHttpHealthCheck("/health");

builder.AddProject<Projects.AspireApp_WeatherAPI>("weatherapi");

builder.AddProject<Projects.YarpGateway>("gateway");

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(masterDataService)
    .WaitFor(masterDataService);

builder.Build().Run();
