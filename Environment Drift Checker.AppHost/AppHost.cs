var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server container with a database
var sql = builder.AddSqlServer("sql")
    .WithDataVolume()  // Persist data between runs
    .AddDatabase("driftdb");

// Add API Service with database reference
// WaitFor ensures SQL Server is fully ready before starting the API
var apiService = builder.AddProject<Projects.Environment_Drift_Checker_ApiService>("apiservice")
    .WithReference(sql)
    .WaitFor(sql);  // ⭐ Wait for SQL Server to be healthy

// Add Blazor Web App with API reference
builder.AddProject<Projects.Environment_Drift_Checker_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);  // ⭐ Wait for API to be ready

builder.Build().Run();