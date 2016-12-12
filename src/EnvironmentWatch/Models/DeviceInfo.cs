using System.Collections.Generic;
using System.Linq;

namespace EnvironmentWatch.Models
{
    public class DeviceInfo
    {
        public DeviceInfo()
        {
            RecentMeasurements = new List<MeasurementSet>();
            Devices = new List<ReportingDevice>();
        }

        public int? ReportingDeviceId { get; set; }
        public string TypeName { get; set; }
        public string LocationName { get; set; }
        public string LocalIp { get; set; }
        public List<ReportingDevice> Devices { get; set; }
        public List<Location> Locations { get; set; }
        public List<MeasurementSet> RecentMeasurements { get; set; }
        public MeasurementSet LastSet { get; set; }


        public string HoursDelim
        {
            get
            {
                return (!RecentMeasurements.Any()) ? string.Empty
                    : string.Join(",", RecentMeasurements.OrderBy(m => m.MeasuredDate)
                    .Select(m => string.Format("'{0}'", m.HourOnlyString)).ToList());
            }
        }

        public string TempDelim
        {
            get
            {
                return (!RecentMeasurements.Any()) ? string.Empty
                    : string.Join(",", RecentMeasurements.OrderBy(m => m.MeasuredDate)
                    .Select(m => m.TempString).ToList());
            }
        }

        public string HumidDelim
        {
            get
            {
                return (!RecentMeasurements.Any()) ? string.Empty
                    : string.Join(",", RecentMeasurements.OrderBy(m => m.MeasuredDate)
                    .Select(m => m.HumidString).ToList());
            }
        }

        public string LightDelim
        {
            get
            {
                return (!RecentMeasurements.Any()) ? string.Empty
                    : string.Join(",", RecentMeasurements.OrderBy(m => m.MeasuredDate)
                    .Select(m => m.LightString).ToList());
            }
        }
    }
}
