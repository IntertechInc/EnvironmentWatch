using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentWatch.Models
{
    [Table("Measurement")]
    public class Measurement
    {
        [Required]
        public int MeasurementId { get; set; }
        [Required]
        public int ReportingDeviceId { get; set; }
        [Required]
        public int MeasurementTypeId { get; set; }
        [Required]
        public int LocationId { get; set; }
        [Required]
        public decimal MeasuredValue { get; set; }
        [Required]
        public System.DateTime MeasuredDate { get; set; }
        public virtual ReportingDevice ReportingDevice { get; set; }
        public virtual MeasurementType MeasurementType { get; set; }
        public virtual Location Location { get; set; }
    }

}
