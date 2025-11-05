using Microsoft.EntityFrameworkCore;
using FluentMigrator.Runner;
using EnvironmentDriftChecker.Data.Context;
using EnvironmentDriftChecker.Data.Repositories;
using EnvironmentDriftChecker.Data.Repositories.Interfaces;
using EnvironmentDriftChecker.ApiService.Services;
using EnvironmentDriftChecker.ApiService.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations
builder.AddServiceDefaults();

// Add controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Environment Drift Checker API",
        Version = "v1",
        Description = "API for detecting and tracking configuration drift across environments"
    });
});

// Configure Database Connection (Aspire will inject this automatically)
var connectionString = builder.Configuration.GetConnectionString("driftdb")
    ?? throw new InvalidOperationException("Connection string 'driftdb' not found.");

// Add DbContext for EF Core
builder.Services.AddDbContext<DriftCheckerDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure FluentMigrator
builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddSqlServer()
        .WithGlobalConnectionString(connectionString)
        .ScanIn(typeof(DriftCheckerDbContext).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());

// Register Repositories
builder.Services.AddScoped<IEnvironmentRepository, EnvironmentRepository>();
builder.Services.AddScoped<IConfigurationItemRepository, ConfigurationItemRepository>();
builder.Services.AddScoped<IDriftRecordRepository, DriftRecordRepository>();

// Register Services
builder.Services.AddScoped<IEnvironmentService, EnvironmentService>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<IDriftCheckerService, DriftCheckerService>();

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins("https://localhost:7001", "http://localhost:5001") // Blazor app ports
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazor");
app.UseAuthorization();
app.MapControllers();

// Run FluentMigrator migrations on startup
using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

    try
    {
        runner.MigrateUp();
        app.Logger.LogInformation("Database migrations completed successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while migrating the database");
        throw;
    }
}

app.Run();