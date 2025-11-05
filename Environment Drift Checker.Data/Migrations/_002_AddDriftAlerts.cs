using FluentMigrator;

namespace EnvironmentDriftChecker.Data.Migrations
{
    [Migration(002)]
    public class AddDriftAlerts : Migration
    {
        public override void Up()
        {
            // Create DriftAlerts table
            Create.Table("DriftAlerts")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("AlertName").AsString(100).NotNullable()
                .WithColumn("Description").AsString(500).Nullable()
                .WithColumn("Condition").AsString(200).NotNullable()
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("NotificationChannel").AsString(50).NotNullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable();

            // Create EnvironmentAlerts join table (Many-to-Many)
            Create.Table("EnvironmentAlerts")
                .WithColumn("EnvironmentId").AsInt32().NotNullable()
                .WithColumn("DriftAlertId").AsInt32().NotNullable();

            Create.PrimaryKey("PK_EnvironmentAlerts")
                .OnTable("EnvironmentAlerts")
                .Columns("EnvironmentId", "DriftAlertId");

            Create.ForeignKey("FK_EnvironmentAlerts_Environments")
                .FromTable("EnvironmentAlerts").ForeignColumn("EnvironmentId")
                .ToTable("Environments").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);

            Create.ForeignKey("FK_EnvironmentAlerts_DriftAlerts")
                .FromTable("EnvironmentAlerts").ForeignColumn("DriftAlertId")
                .ToTable("DriftAlerts").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);
        }

        public override void Down()
        {
            Delete.Table("EnvironmentAlerts");
            Delete.Table("DriftAlerts");
        }
    }
}