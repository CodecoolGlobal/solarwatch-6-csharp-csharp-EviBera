using Microsoft.AspNetCore.Authorization;
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


        [HttpGet("GetCityWithSolarData"), Authorize]
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


        [HttpPost("PostCity")]
        public async Task<ActionResult<City>> PostCityAsync(string cityName, double latitude, double longitude, string country,
            string? state)
        {
            try
            {
                var city = new City()
                {
                    CityName = cityName,
                    Lat = latitude,
                    Lon = longitude,
                    Country = country,
                    State = state
                };

                var newCity = await _cityRepository.AddAsync(city);

                if (newCity == null) 
                {
                    return BadRequest("Failed to add new city");
                }

                return Ok(newCity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding new city, {ex.Message}");
                return NotFound($"Error adding new city, {ex.Message}");
            }
        }
        

        [HttpPost("PostSolarData")]
        public async Task<ActionResult<SunsetSunriseData>> PostSolarDataAsync([Required] int cityId, DateTime date, DateTime sunset, 
            DateTime sunrise)
        {
            try
            {
                var sunsetSunriseData = new SunsetSunriseData()
                {
                    Date = date,
                    Sunset = sunset,
                    Sunrise = sunrise,
                    CityId = cityId
                };

                var newSunsetSunriseData = await _sunsetSunriseDataRepository.AddSolarDataAsync(sunsetSunriseData);

                if (sunsetSunriseData == null)
                {
                    return BadRequest("Failed to add solar data");
                }

                return Ok(newSunsetSunriseData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding new solar data, {ex.Message}");
                return NotFound($"Error adding new solar data, {ex.Message}");
            }
        }


        [HttpPatch("UpdateCity")]
        public async Task<ActionResult<City>> UpdateCityAsync([Required] int cityId, [FromBody] CityUpdateDTO cityDto)
        {
            try
            {
                var updatedCity = await _cityRepository.UpdateAsync(cityId, cityDto);

                if (updatedCity == null)
                {
                    return NotFound($"Failed to update city with id {cityId}");
                }

                return Ok(updatedCity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating city, {ex.Message}");
                return BadRequest($"Error updating city, {ex.Message}");
            }
        }
        

        [HttpPatch("UpdateSolarData")]
        public async Task<ActionResult<SunsetSunriseData>> UpdateSolarDataAsync([Required] int solarDataId, 
            [FromBody] SolarDataDTO newSunsetSunriseData)
        {
            try
            {
                var updatedSolarData = await _sunsetSunriseDataRepository.UpdateSolarDataByIdAsync(solarDataId, newSunsetSunriseData);

                if (updatedSolarData == null)
                {
                    return NotFound($"Failed to update solar data with id {solarDataId}");
                }

                return Ok(updatedSolarData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating solar data, {ex.Message}");
                return BadRequest($"Error updating solar data, {ex.Message}");
            }
        }


        [HttpDelete("DeleteCity")]
        public async Task<ActionResult<City>> DeleteCityAndItsSolarDataAsync([Required] int cityId)
        {
            try
            {
                var cityToDelete = await _cityRepository.DeleteByIdAsync(cityId);

                if (cityToDelete == null)
                {
                    return NotFound("Nonexistent city, failed to delete");
                }

                await _sunsetSunriseDataRepository.DeleteByCityIdAsync(cityId);

                return Ok(cityToDelete);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting city, {ex.Message}");
                return BadRequest($"Error deleting city, {ex.Message}");
            }
        }


        [HttpDelete("DeleteSolarData")]
        public async Task<ActionResult<SunsetSunriseData>> DeleteSolarDataAsync([Required] int solarDataId)
        {
            try
            {
                var deletedData = await _sunsetSunriseDataRepository.DeleteSolarDataByIdAsync(solarDataId);

                if (deletedData == null)
                {
                    return NotFound("Nonexistent solar data, failed to delete");
                }

                return Ok(deletedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting solar data, {ex.Message}");
                return BadRequest($"Error deleting solar data, {ex.Message}");
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
                    await _cityRepository.AddAsync(city);
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
                    await _sunsetSunriseDataRepository.AddSolarDataAsync(solarData);
                }

            }

            return solarData;
        }

    }
}
