using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentWatch.Models
{
    [Table("ReportingDevice")]
    public class ReportingDevice
    {
        [Required]
        public int ReportingDeviceId { get; set; }
        [Required]
        public int DeviceTypeId { get; set; }
        [Required]
        public int LocationId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string LastIpAddress { get; set; }
        public virtual DeviceType DeviceType { get; set; }
        public virtual Location Location { get; set; }
        public virtual ICollection<Measurement> Measurements { get; set; }
    }

}
