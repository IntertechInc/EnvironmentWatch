using System;

namespace EnvironmentWatch.Models
{
    public class MeasurementSet
    {
        public DateTime? MeasuredDate { get; set; }
        public string LocationName { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public decimal? Light { get; set; }

        public string DateOnlyString => MeasuredDate?.ToString("yyyy-MM-dd") ?? string.Empty;
        public string TimeOnlyString => MeasuredDate?.ToString("hh:mm:ss tt") ?? string.Empty;
        public string HourOnlyString => MeasuredDate?.ToString("htt") ?? string.Empty;
        public string ShortDayTimeString => MeasuredDate?.ToString("ddd h tt") ?? string.Empty;

        public string TempString => (Temperature != null) ? Temperature.Value.ToString("###.0") : "0.0";
        public string HumidString => (Humidity != null) ? Humidity.Value.ToString("###.0") : "0.0";
        public string LightString => (Light != null) ? Light.Value.ToString("###.0") : "0.0";
    }
}
