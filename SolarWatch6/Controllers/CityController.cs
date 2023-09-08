using Microsoft.AspNetCore.Mvc;
using SolarWatch6.Models;
using SolarWatch6.Services;
using SolarWatch6.Services.Repository;
using System.ComponentModel.DataAnnotations;

namespace SolarWatch6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ILogger<CityController> _logger;
        private readonly ICityService _cityService;
        private readonly IJsonProcessor _jsonProcessor;
        private readonly ICityRepository _cityRepository;
        private readonly ISolarDataRepository _sunsetSunriseDataRepository;


        public CityController(ILogger<CityController> logger, ICityService cityService, IJsonProcessor jsonProcessor,
            ICityRepository cityRepository, ISolarDataRepository sunsetSunriseDataRepository)
        {
            _logger = logger;
            _cityService = cityService;
            _jsonProcessor = jsonProcessor;
            _cityRepository = cityRepository;
            _sunsetSunriseDataRepository = sunsetSunriseDataRepository;
        }

        [HttpGet("GetAsync")]
        public async Task<ActionResult<CityWithSolarData>> GetAsync(string cityName, [Required] DateOnly day)
        {
            try
            {
                var city = await FindACityAsync(cityName);

                if (city == null)
                {
                    return NotFound($"City {cityName} not found");
                }


                var solarData = await FindSunsetSunriseDataAsync(city, day);

                if (solarData == null)
                {
                    return NotFound($"Solar data not available for {cityName} on {day}");
                }


                if (city.SunsetSunriseDataList == null)
                {
                    city.SunsetSunriseDataList = new List<SunsetSunriseData>();
                }

                city.SunsetSunriseDataList.Add(solarData);


                var combinedData = new CityWithSolarData()
                {
                    City = city,
                    SolarData = solarData
                };

                return Ok(combinedData);

            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting solar data, {ex.Message}");
                return NotFound($"Error getting solar data, {ex.Message}");
            }
        }

        private async Task<City> FindACityAsync(string cityName)
        {
            var city = await _cityRepository.GetByNameAsync(cityName);

            if (city == null)
            {
                var newCityData = await _cityService.GetCoordinatesAsync(cityName);
                city = _jsonProcessor.ProcessJsonCityData(newCityData);

                if (city != null)
                {
                    _cityRepository.AddAsync(city);
                }

            }

            return city;
        }

        private async Task<SunsetSunriseData> FindSunsetSunriseDataAsync(City city, DateOnly day)
        { 
            var solarData = await _sunsetSunriseDataRepository.GetByDateAndCityAsync(day, city.Id);

            if (solarData == null)
            {
                var solarDataString = await _cityService.GetSolarDataAsync(day, city);

                if (solarDataString != null)
                {
                    solarData = _jsonProcessor.ProcessJsonSolarData(solarDataString, day);
                    solarData.CityId = city.Id;
                    _sunsetSunriseDataRepository.AddSolarDataAsync(solarData);
                }

            }

            return solarData;
        }

    }
}
