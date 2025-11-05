using Microsoft.EntityFrameworkCore;
using EnvironmentDriftChecker.Domain.Entities;

namespace EnvironmentDriftChecker.Data.Context
{
    public class DriftCheckerDbContext : DbContext
    {
        public DriftCheckerDbContext(DbContextOptions<DriftCheckerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Domain.Entities.Environment> Environments { get; set; }
        public DbSet<ConfigurationItem> ConfigurationItems { get; set; }
        public DbSet<DriftRecord> DriftRecords { get; set; }
        public DbSet<DriftDetail> DriftDetails { get; set; }
        public DbSet<DriftAlert> DriftAlerts { get; set; }
        public DbSet<EnvironmentAlert> EnvironmentAlerts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Environment Configuration
            modelBuilder.Entity<Domain.Entities.Environment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // ConfigurationItem Configuration
            modelBuilder.Entity<ConfigurationItem>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Key).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Value).IsRequired().HasMaxLength(500);
                entity.Property(c => c.Category).IsRequired().HasMaxLength(50);

                entity.HasOne(c => c.Environment)
                      .WithMany(e => e.ConfigurationItems)
                      .HasForeignKey(c => c.EnvironmentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(c => new { c.EnvironmentId, c.Key }).IsUnique();
            });

            // DriftRecord Configuration
            modelBuilder.Entity<DriftRecord>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Status).IsRequired().HasMaxLength(20);

                entity.HasOne(d => d.SourceEnvironment)
                      .WithMany(e => e.DriftRecordsAsSource)
                      .HasForeignKey(d => d.SourceEnvironmentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.TargetEnvironment)
                      .WithMany(e => e.DriftRecordsAsTarget)
                      .HasForeignKey(d => d.TargetEnvironmentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // DriftDetail Configuration
            modelBuilder.Entity<DriftDetail>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Key).IsRequired().HasMaxLength(100);
                entity.Property(d => d.SourceValue).HasMaxLength(500);
                entity.Property(d => d.TargetValue).HasMaxLength(500);
                entity.Property(d => d.DriftType).IsRequired().HasMaxLength(50);
                entity.Property(d => d.Severity).IsRequired().HasMaxLength(20);

                entity.HasOne(d => d.DriftRecord)
                      .WithMany(r => r.DriftDetails)
                      .HasForeignKey(d => d.DriftRecordId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // DriftAlert Configuration
            modelBuilder.Entity<DriftAlert>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.AlertName).IsRequired().HasMaxLength(100);
                entity.Property(a => a.Description).HasMaxLength(500);
                entity.Property(a => a.Condition).IsRequired().HasMaxLength(200);
                entity.Property(a => a.NotificationChannel).IsRequired().HasMaxLength(50);
            });

            // EnvironmentAlert Configuration (Many-to-Many Join Table)
            modelBuilder.Entity<EnvironmentAlert>(entity =>
            {
                entity.HasKey(ea => new { ea.EnvironmentId, ea.DriftAlertId });

                entity.HasOne(ea => ea.Environment)
                      .WithMany(e => e.EnvironmentAlerts)
                      .HasForeignKey(ea => ea.EnvironmentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ea => ea.DriftAlert)
                      .WithMany(a => a.EnvironmentAlerts)
                      .HasForeignKey(ea => ea.DriftAlertId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}