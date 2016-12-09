using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EnvironmentWatch.Models
{
    public class EnvWatchContext : DbContext
    {
        public EnvWatchContext(DbContextOptions<EnvWatchContext> options) : base(options) { }

        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<MeasurementType> MeasurementTypes { get; set; }
        public DbSet<ReportingDevice> ReportingDevices { get; set; }
        public DbSet<Measurement> Measurements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeviceType>(d => { d.Property(e => e.DeviceTypeId).ValueGeneratedNever(); });
            modelBuilder.Entity<Location>(d => { d.Property(e => e.LocationId).ValueGeneratedNever(); });
            modelBuilder.Entity<MeasurementType>(d =>
            { d.Property(e => e.MeasurementTypeId).ValueGeneratedNever(); });
            modelBuilder.Entity<ReportingDevice>(d =>
            { d.Property(e => e.ReportingDeviceId).ValueGeneratedNever(); });

            modelBuilder.Entity<Measurement>()
                .HasOne(d => d.ReportingDevice)
                .WithMany(m => m.Measurements)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
