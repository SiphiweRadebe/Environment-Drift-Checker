using FluentMigrator;

namespace EnvironmentDriftChecker.Data.Migrations
{
    [Migration(003)]
    public class SeedData : Migration
    {
        public override void Up()
        {
            // Seed Environments
            Insert.IntoTable("Environments").Row(new
            {
                Name = "Development",
                Description = "Local development environment",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });

            Insert.IntoTable("Environments").Row(new
            {
                Name = "QA",
                Description = "Quality assurance testing environment",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });

            Insert.IntoTable("Environments").Row(new
            {
                Name = "UAT",
                Description = "User acceptance testing environment",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });

            Insert.IntoTable("Environments").Row(new
            {
                Name = "Production",
                Description = "Live production environment",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });

            // Seed Configuration Items for Development
            Insert.IntoTable("ConfigurationItems").Row(new
            {
                EnvironmentId = 1,
                Key = "ApiUrl",
                Value = "https://dev.api.sage.com",
                Category = "Api",
                LastUpdated = DateTime.UtcNow,
                IsSensitive = false
            });

            Insert.IntoTable("ConfigurationItems").Row(new
            {
                EnvironmentId = 1,
                Key = "DatabaseConnection",
                Value = "Server=dev-sql.sage.com;Database=PayrollDev",
                Category = "Database",
                LastUpdated = DateTime.UtcNow,
                IsSensitive = true
            });

            Insert.IntoTable("ConfigurationItems").Row(new
            {
                EnvironmentId = 1,
                Key = "FeatureFlags:NewPayrollUI",
                Value = "true",
                Category = "Feature",
                LastUpdated = DateTime.UtcNow,
                IsSensitive = false
            });

            Insert.IntoTable("ConfigurationItems").Row(new
            {
                EnvironmentId = 1,
                Key = "Timeout",
                Value = "30",
                Category = "Performance",
                LastUpdated = DateTime.UtcNow,
                IsSensitive = false
            });

            // Seed Configuration Items for QA
            Insert.IntoTable("ConfigurationItems").Row(new
            {
                EnvironmentId = 2,
                Key = "ApiUrl",
                Value = "https://qa.api.sage.com",
                Category = "Api",
                LastUpdated = DateTime.UtcNow,
                IsSensitive = false
            });

            Insert.IntoTable("ConfigurationItems").Row(new
            {
                EnvironmentId = 2,
                Key = "DatabaseConnection",
                Value = "Server=qa-sql.sage.com;Database=PayrollQA",
                Category = "Database",
                LastUpdated = DateTime.UtcNow,
                IsSensitive = true
            });

            Insert.IntoTable("ConfigurationItems").Row(new
            {
                EnvironmentId = 2,
                Key = "FeatureFlags:NewPayrollUI",
                Value = "true",
                Category = "Feature",
                LastUpdated = DateTime.UtcNow,
                IsSensitive = false
            });

            Insert.IntoTable("ConfigurationItems").Row(new
            {
                EnvironmentId = 2,
                Key = "Timeout",
                Value = "45",
                Category = "Performance",
                LastUpdated = DateTime.UtcNow,
                IsSensitive = false
            });

            // Seed Configuration Items for UAT
            Insert.IntoTable("ConfigurationItems").Row(new
            {
                EnvironmentId = 3,
                Key = "ApiUrl",
                Value = "https://uat.api.sage.com",
                Category = "Api",
                LastUpdated = DateTime.UtcNow,
                IsSensitive = false
            });

            Insert.IntoTable("ConfigurationItems").Row(new
            {
                EnvironmentId = 3,
                Key = "DatabaseConnection",
                Value = "Server=uat-sql.sage.com;Database=PayrollUAT",
                Category = "Database",
                LastUpdated = DateTime.UtcNow,
                IsSensitive = true
            });

            Insert.IntoTable("ConfigurationItems").Row(new
            {
                EnvironmentId = 3,
                Key = "FeatureFlags:NewPayrollUI",
                Value = "false",
                Category = "Feature",
                LastUpdated = DateTime.UtcNow,
                IsSensitive = false
            });

            Insert.IntoTable("ConfigurationItems").Row(new
            {
                EnvironmentId = 3,
                Key = "Timeout",
                Value = "60",
                Category = "Performance",
                LastUpdated = DateTime.UtcNow,
                IsSensitive = false
            });

            // Seed Configuration Items for Production
            Insert.IntoTable("ConfigurationItems").Row(new
            {
                EnvironmentId = 4,
                Key = "ApiUrl",
                Value = "https://api.sage.com",
                Category = "Api",
                LastUpdated = DateTime.UtcNow,
                IsSensitive = false
            });

            Insert.IntoTable("ConfigurationItems").Row(new
            {
                EnvironmentId = 4,
                Key = "DatabaseConnection",
                Value = "Server=prod-sql.sage.com;Database=PayrollProd",
                Category = "Database",
                LastUpdated = DateTime.UtcNow,
                IsSensitive = true
            });

            Insert.IntoTable("ConfigurationItems").Row(new
            {
                EnvironmentId = 4,
                Key = "FeatureFlags:NewPayrollUI",
                Value = "false",
                Category = "Feature",
                LastUpdated = DateTime.UtcNow,
                IsSensitive = false
            });

            Insert.IntoTable("ConfigurationItems").Row(new
            {
                EnvironmentId = 4,
                Key = "Timeout",
                Value = "60",
                Category = "Performance",
                LastUpdated = DateTime.UtcNow,
                IsSensitive = false
            });

            // Seed Drift Alerts
            Insert.IntoTable("DriftAlerts").Row(new
            {
                AlertName = "Critical Drift Warning",
                Description = "Alert when more than 5 configuration drifts detected",
                Condition = "DriftCount > 5",
                IsActive = true,
                NotificationChannel = "Teams",
                CreatedAt = DateTime.UtcNow
            });

            Insert.IntoTable("DriftAlerts").Row(new
            {
                AlertName = "Security Config Mismatch",
                Description = "Alert when security-related configs differ",
                Condition = "Category = 'Security' AND DriftType = 'Different'",
                IsActive = true,
                NotificationChannel = "Email",
                CreatedAt = DateTime.UtcNow
            });

            // Link alerts to Production environment
            Insert.IntoTable("EnvironmentAlerts").Row(new
            {
                EnvironmentId = 4, // Production
                DriftAlertId = 1
            });

            Insert.IntoTable("EnvironmentAlerts").Row(new
            {
                EnvironmentId = 4, // Production
                DriftAlertId = 2
            });
        }

        public override void Down()
        {
            Delete.FromTable("EnvironmentAlerts").AllRows();
            Delete.FromTable("DriftAlerts").AllRows();
            Delete.FromTable("ConfigurationItems").AllRows();
            Delete.FromTable("Environments").AllRows();
        }
    }
}