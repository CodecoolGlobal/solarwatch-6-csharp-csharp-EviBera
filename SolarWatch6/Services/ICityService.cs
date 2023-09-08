using SolarWatch6.Models;

namespace SolarWatch6.Services
{
    public interface ICityService
    {
        Task<string> GetCoordinatesAsync(string cityName);
        Task<string> GetSolarDataAsync(DateOnly when, City city);
        Task<City> GetCityAsync(string cityData, DateOnly when);
    }
}
