using FluentMigrator;

namespace EnvironmentDriftChecker.Data.Migrations
{
    [Migration(001)]
    public class InitialSchema : Migration
    {
        public override void Up()
        {
            // Create Environments table
            Create.Table("Environments")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString(50).NotNullable().Unique()
                .WithColumn("Description").AsString(200).Nullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true);

            // Create ConfigurationItems table
            Create.Table("ConfigurationItems")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("EnvironmentId").AsInt32().NotNullable()
                .WithColumn("Key").AsString(100).NotNullable()
                .WithColumn("Value").AsString(500).NotNullable()
                .WithColumn("Category").AsString(50).NotNullable()
                .WithColumn("LastUpdated").AsDateTime().NotNullable()
                .WithColumn("IsSensitive").AsBoolean().NotNullable().WithDefaultValue(false);

            Create.ForeignKey("FK_ConfigurationItems_Environments")
                .FromTable("ConfigurationItems").ForeignColumn("EnvironmentId")
                .ToTable("Environments").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);

            Create.Index("IX_ConfigurationItems_EnvironmentId_Key")
                .OnTable("ConfigurationItems")
                .OnColumn("EnvironmentId").Ascending()
                .OnColumn("Key").Ascending()
                .WithOptions().Unique();

            // Create DriftRecords table
            Create.Table("DriftRecords")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("SourceEnvironmentId").AsInt32().NotNullable()
                .WithColumn("TargetEnvironmentId").AsInt32().NotNullable()
                .WithColumn("ComparisonDate").AsDateTime().NotNullable()
                .WithColumn("TotalKeys").AsInt32().NotNullable()
                .WithColumn("DriftCount").AsInt32().NotNullable()
                .WithColumn("Status").AsString(20).NotNullable();

            Create.ForeignKey("FK_DriftRecords_SourceEnvironment")
                .FromTable("DriftRecords").ForeignColumn("SourceEnvironmentId")
                .ToTable("Environments").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.None);

            Create.ForeignKey("FK_DriftRecords_TargetEnvironment")
                .FromTable("DriftRecords").ForeignColumn("TargetEnvironmentId")
                .ToTable("Environments").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.None);

            // Create DriftDetails table
            Create.Table("DriftDetails")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("DriftRecordId").AsInt32().NotNullable()
                .WithColumn("Key").AsString(100).NotNullable()
                .WithColumn("SourceValue").AsString(500).Nullable()
                .WithColumn("TargetValue").AsString(500).Nullable()
                .WithColumn("DriftType").AsString(50).NotNullable()
                .WithColumn("Severity").AsString(20).NotNullable();

            Create.ForeignKey("FK_DriftDetails_DriftRecords")
                .FromTable("DriftDetails").ForeignColumn("DriftRecordId")
                .ToTable("DriftRecords").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);
        }

        public override void Down()
        {
            Delete.Table("DriftDetails");
            Delete.Table("DriftRecords");
            Delete.Table("ConfigurationItems");
            Delete.Table("Environments");
        }
    }
}
