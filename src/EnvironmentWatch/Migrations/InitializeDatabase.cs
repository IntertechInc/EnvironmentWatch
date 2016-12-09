using System.Linq;
using EnvironmentWatch.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EnvironmentWatch.Migrations
{
    public class InitializeDatabase
    {
        public static void SeedData(IApplicationBuilder app)
        {
            using (var serviceScope =
                app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var serviceProvider = serviceScope.ServiceProvider;

                using (var db = serviceProvider.GetService<EnvWatchContext>())
                {
                    SeedDeviceTypes(db);
                    SeedLocations(db);
                    SeedMeasurementTypes(db);
                    SeedReportingDevices(db);
                }
            }
        }

        private static void SeedDeviceTypes(EnvWatchContext db)
        {
            if (!db.DeviceTypes.Any())
            {
                db.DeviceTypes.Add(new DeviceType
                {
                    DeviceTypeId = 1,
                    Name = "Huzzah/ESP8266",
                    Description = "Adafruit breakout board for ESP8266. Arduino compatible"
                });
                db.DeviceTypes.Add(new DeviceType
                {
                    DeviceTypeId = 2,
                    Name = "Raspberry Pi Zero",
                    Description = "Raspberry Pi Zero, likely running Raspian"
                });
                db.SaveChanges();
            }
        }

        private static void SeedLocations(EnvWatchContext db)
        {
            if (!db.Locations.Any())
            {
                db.Locations.AddAsync(new Location
                {
                    LocationId = 1,
                    Name = "Office - My Cube",
                    Description = "Data was gathered at my place of work and in my cube"
                });
                db.Locations.AddAsync(new Location
                {
                    LocationId = 2,
                    Name = "Office - Kitchen",
                    Description = "Data was gathered at my place of work and in the kitchen"
                });
                db.Locations.AddAsync(new Location
                {
                    LocationId = 3,
                    Name = "Home - Office",
                    Description = "Data was gathered at my home and in my office"
                });
                db.Locations.AddAsync(new Location
                {
                    LocationId = 4,
                    Name = "Home - Family Room",
                    Description = "Data was gathered at my home and in my family room"
                });
                db.SaveChanges();
            }
        }

        private static void SeedMeasurementTypes(EnvWatchContext db)
        {
            if (!db.MeasurementTypes.Any())
            {
                db.MeasurementTypes.Add(new MeasurementType
                {
                    MeasurementTypeId = 1,
                    Name = "Temperature",
                    Description = "Ambient temperature in Farhenheit"
                });
                db.MeasurementTypes.Add(new MeasurementType
                {
                    MeasurementTypeId = 2,
                    Name = "Humidity",
                    Description = "Level of relative humidity"
                });
                db.MeasurementTypes.Add(new MeasurementType
                {
                    MeasurementTypeId = 3,
                    Name = "Light",
                    Description = "Light level measured in percent"
                });
                db.SaveChanges();
            }
        }

        private static void SeedReportingDevices(EnvWatchContext db)
        {
            if (!db.ReportingDevices.Any())
            {
                db.ReportingDevices.Add(new ReportingDevice
                {
                    ReportingDeviceId = 1,
                    DeviceTypeId = 1,
                    LocationId = 1,
                    Name = "Huzzah 1",
                    Description = "My first Huzzah/ESP8266"
                });
                db.ReportingDevices.Add(new ReportingDevice
                {
                    ReportingDeviceId = 2,
                    DeviceTypeId = 2,
                    LocationId = 1,
                    Name = "Pi Zero 1",
                    Description = "My Raspberry Pi Zero"
                });
                db.SaveChanges();
            }
        }
    }
}
