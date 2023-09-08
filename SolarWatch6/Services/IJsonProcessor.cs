using SolarWatch6.Models;

namespace SolarWatch6.Services
{
    public interface IJsonProcessor
    {
        City ProcessJsonCityData(string cityData);
        SunsetSunriseData ProcessJsonSolarData(string solarData, DateOnly when);
    }
}
