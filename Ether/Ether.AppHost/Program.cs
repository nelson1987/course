var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");
var messaging = builder.AddRabbitMQ("RabbitMQConnection");
var sql = builder.AddSqlServer("sql")
                 .AddDatabase("sqldata");
var mongo = builder.AddMongoDB("mongo")
                    .AddDatabase("mongodb");


var apiService = builder.AddProject<Projects.Ether_ApiService>("apiservice")
    .WithReference(messaging)
    .WithReference(sql)
    .WithReference(mongo);

builder.AddProject<Projects.Ether_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();
