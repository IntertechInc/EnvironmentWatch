using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentWatch.Models
{
    [Table("Location")]
    public class Location
    {
        [Required]
        public int LocationId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<ReportingDevice> ReportingDevices { get; set; }
        public virtual ICollection<Measurement> Measurements { get; set; }
    }

}
