using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EnvironmentWatch.Models
{
    public class LocationHandler
    {
        public int ReportingDeviceId { get; set; }

        public string DeviceTypeName { get; set; }

        [DisplayName("Location Name")]
        [Required(ErrorMessage = "Please enter a location name")]
        public string LocationName { get; set; }

        [DisplayName("Location Description")]
        public string LocationDesc { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }
    }
}
