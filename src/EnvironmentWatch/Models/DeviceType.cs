using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentWatch.Models
{
    [Table("DeviceType")]
    public class DeviceType
    {
        [Required]
        public int DeviceTypeId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<ReportingDevice> ReportingDevices { get; set; }
    }
}
