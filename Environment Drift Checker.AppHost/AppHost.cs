var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server container with a database
var sql = builder.AddSqlServer("sql")
    .WithDataVolume()  // Persist data between runs
    .AddDatabase("driftdb");

// Add API Service with database reference
var apiService = builder.AddProject<Projects.Environment_Drift_Checker_ApiService>("apiservice")
    .WithReference(sql);

// Add Blazor Web App with API reference
builder.AddProject<Projects.Environment_Drift_Checker_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();