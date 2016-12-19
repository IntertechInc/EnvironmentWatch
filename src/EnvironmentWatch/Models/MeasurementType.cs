using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentWatch.Models
{
    [Table("MeasurementType")]
    public class MeasurementType
    {
        [Required]
        public int MeasurementTypeId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Measurement> Measurements { get; set; }
    }


    public enum MeasureTypeEnum
    {
        Temperature = 1,
        Humidity = 2,
        Light = 3
    }
}
