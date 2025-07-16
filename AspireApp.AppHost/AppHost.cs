var builder = DistributedApplication.CreateBuilder(args);

//Add Cache
var cachePassword = builder.AddParameter("CachePassword");
var cache = builder.AddRedis("cache", 6379, cachePassword)
    .WithRedisInsight()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEnvironment("REDIS_ARGS", "--requirepass \"\""); // disables auth

//Add SQL
var dbPassword = builder.AddParameter("DbPassword");
var sqlServer = builder.AddSqlServer("sql", dbPassword, port:1433)
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithVolume("sql-data", "/var/opt/mssql")
    .AddDatabase("masterdatadb");

//Add RabbbitMQ
var rabbitUser = builder.AddParameter("RabbitUser");
var rabbitPass = builder.AddParameter("RabbitPass");
var rabbit = builder.AddRabbitMQ("rabbitmq", rabbitUser, rabbitPass, 5672)
    .WithImage("rabbitmq:3-management");

var masterDataService = builder.AddProject<Projects.AspireApp_MasterDataService>("masterdataservice")
    .WithReference(rabbit)
    .WaitFor(rabbit)
    .WithReference(sqlServer);
    //.WithHttpHealthCheck("/health");

builder.AddProject<Projects.AspireApp_WeatherAPI>("weatherapi");

builder.AddProject<Projects.YarpGateway>("gateway");

builder.AddProject<Projects.AspireApp_WebWasm>("aspireapp-webwasm")
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(masterDataService)
    .WaitFor(masterDataService)
    .WaitFor(rabbit);

builder.AddProject<Projects.AspireApp_NotificationHubService>("notificationhubservice")
    .WithReference(rabbit)
    .WaitFor(rabbit);

builder.Build().Run();
