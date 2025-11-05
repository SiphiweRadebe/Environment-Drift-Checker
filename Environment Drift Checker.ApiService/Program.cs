using Microsoft.EntityFrameworkCore;
using FluentMigrator.Runner;
using EnvironmentDriftChecker.Data.Context;
using EnvironmentDriftChecker.Data.Repositories;
using EnvironmentDriftChecker.Data.Repositories.Interfaces;
using EnvironmentDriftChecker.ApiService.Services;
using EnvironmentDriftChecker.ApiService.Services.Interfaces;

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

// Run FluentMigrator migrations on startup with retry logic
using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    var dbContext = scope.ServiceProvider.GetRequiredService<DriftCheckerDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    // Wait for SQL Server to be ready (with retries)
    var maxRetries = 10;
    var retryCount = 0;
    var delay = TimeSpan.FromSeconds(3);

    while (retryCount < maxRetries)
    {
        try
        {
            logger.LogInformation("Attempting to connect to database... (Attempt {Retry}/{Max})", retryCount + 1, maxRetries);
            await dbContext.Database.CanConnectAsync();
            logger.LogInformation(" Successfully connected to database");
            break;
        }
        catch (Exception ex)
        {
            retryCount++;
            if (retryCount >= maxRetries)
            {
                logger.LogError(ex, "❌ Failed to connect to database after {Max} attempts", maxRetries);
                throw;
            }
            logger.LogWarning("⏳ Database not ready yet. Waiting {Delay} seconds... ({Retry}/{Max})", delay.TotalSeconds, retryCount, maxRetries);
            await Task.Delay(delay);
        }
    }

    // Run migrations
    try
    {
        logger.LogInformation("Running database migrations...");
        runner.MigrateUp();
        logger.LogInformation(" Database migrations completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, " An error occurred while migrating the database");
        throw;
    }
}

app.Run();