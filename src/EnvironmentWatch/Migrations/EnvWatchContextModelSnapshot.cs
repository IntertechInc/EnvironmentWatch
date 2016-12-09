using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using EnvironmentWatch.Models;

namespace EnvironmentWatch.Migrations
{
    [DbContext(typeof(EnvWatchContext))]
    partial class EnvWatchContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EnvironmentWatch.Models.DeviceType", b =>
                {
                    b.Property<int>("DeviceTypeId");

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("DeviceTypeId");

                    b.ToTable("DeviceType");
                });

            modelBuilder.Entity("EnvironmentWatch.Models.Location", b =>
                {
                    b.Property<int>("LocationId");

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("LocationId");

                    b.ToTable("Location");
                });

            modelBuilder.Entity("EnvironmentWatch.Models.Measurement", b =>
                {
                    b.Property<int>("MeasurementId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("LocationId");

                    b.Property<DateTime>("MeasuredDate");

                    b.Property<decimal>("MeasuredValue");

                    b.Property<int>("MeasurementTypeId");

                    b.Property<int>("ReportingDeviceId");

                    b.HasKey("MeasurementId");

                    b.HasIndex("LocationId");

                    b.HasIndex("MeasurementTypeId");

                    b.HasIndex("ReportingDeviceId");

                    b.ToTable("Measurement");
                });

            modelBuilder.Entity("EnvironmentWatch.Models.MeasurementType", b =>
                {
                    b.Property<int>("MeasurementTypeId");

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("MeasurementTypeId");

                    b.ToTable("MeasurementType");
                });

            modelBuilder.Entity("EnvironmentWatch.Models.ReportingDevice", b =>
                {
                    b.Property<int>("ReportingDeviceId");

                    b.Property<string>("Description");

                    b.Property<int>("DeviceTypeId");

                    b.Property<string>("LastIpAddress");

                    b.Property<int>("LocationId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ReportingDeviceId");

                    b.HasIndex("DeviceTypeId");

                    b.HasIndex("LocationId");

                    b.ToTable("ReportingDevice");
                });

            modelBuilder.Entity("EnvironmentWatch.Models.Measurement", b =>
                {
                    b.HasOne("EnvironmentWatch.Models.Location", "Location")
                        .WithMany("Measurements")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EnvironmentWatch.Models.MeasurementType", "MeasurementType")
                        .WithMany("Measurements")
                        .HasForeignKey("MeasurementTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EnvironmentWatch.Models.ReportingDevice", "ReportingDevice")
                        .WithMany("Measurements")
                        .HasForeignKey("ReportingDeviceId");
                });

            modelBuilder.Entity("EnvironmentWatch.Models.ReportingDevice", b =>
                {
                    b.HasOne("EnvironmentWatch.Models.DeviceType", "DeviceType")
                        .WithMany("ReportingDevices")
                        .HasForeignKey("DeviceTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EnvironmentWatch.Models.Location", "Location")
                        .WithMany("ReportingDevices")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
