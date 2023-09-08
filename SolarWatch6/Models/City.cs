using System.Text.Json.Serialization;

namespace SolarWatch6.Models
{
    public class City
    {
        public int Id { get; set; }
        public string CityName { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string Country { get; set; }
        public string? State { get; set; }


        // Navigation property for SunsetSunriseData
        [JsonIgnore]
        public ICollection<SunsetSunriseData> SunsetSunriseDataList { get; set; }
    }
}
