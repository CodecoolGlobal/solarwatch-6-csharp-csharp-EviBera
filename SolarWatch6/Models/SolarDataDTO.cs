namespace SolarWatch6.Models
{
    public class SolarDataDTO
    {
        public DateTime Date { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
        public int CityId { get; set; }
    }
}
