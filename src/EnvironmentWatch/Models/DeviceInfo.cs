using System.Collections.Generic;

namespace EnvironmentWatch.Models
{
    public class DeviceInfo
    {
        public int? ReportingDeviceId { get; set; }
        public string TypeName { get; set; }
        public string LocationName { get; set; }
        public string LocalIp { get; set; }
        public List<ReportingDevice> Devices { get; set; } = new List<ReportingDevice>();
        public List<Location> Locations { get; set; } = new List<Location>();
        public MeasurementSet LastSet { get; set; } = new MeasurementSet();
    }
}
