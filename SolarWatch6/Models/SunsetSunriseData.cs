using System.Text.Json.Serialization;

namespace SolarWatch6.Models
{
    public class SunsetSunriseData
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }


        // Foreign key to link with City
        public int CityId { get; set; }

        // Navigation property for City
        [JsonIgnore]
        public City City { get; set; }
    }
}
