using SolarWatch6.Models;

namespace SolarWatch6.Services
{
    public class CityService : ICityService
    {
        private readonly ILogger<CityService> _logger;
        private readonly IJsonProcessor _jsonProcessor;
        private readonly HttpClient client = new HttpClient();

        public CityService(ILogger<CityService> logger, IJsonProcessor jsonProcessor)
        {
            _logger = logger;
            _jsonProcessor = jsonProcessor;
        }


        public async Task<string> GetCoordinatesAsync(string cityName)
        {
            var apiKey = "5d6fd30487df4b2187e713818f4ea218";
            var urlCity = $"http://api.openweathermap.org/geo/1.0/direct?q={cityName}&limit=1&appid={apiKey}";
            _logger.LogInformation("Calling OpenWeather API with url: {url}", urlCity);

            var cityData = await client.GetAsync(urlCity);

            if (await cityData.Content.ReadAsStringAsync() == "[]")
            {
                throw new Exception("City not found");
            }

            return await cityData.Content.ReadAsStringAsync();
        }

        public async Task<string> GetSolarDataAsync(DateOnly when, City aCity)
        {
            string day = $"{when.Year}-{when.Month}-{when.Day}";
            var urlSun = $"https://api.sunrise-sunset.org/json?lat={aCity.Lat}&lng={aCity.Lon}&date={day}";
            _logger.LogInformation("Calling OpenWeather API with url: {url}", urlSun);

            var solarData = await client.GetAsync(urlSun);

            if (await solarData.Content.ReadAsStringAsync() == "[]" || await solarData.Content.ReadAsStringAsync() == "{}")
            {
                throw new Exception("Sunrise and sunset data not found");
            }

            return await solarData.Content.ReadAsStringAsync();
        }

        public async Task<City> GetCityAsync(string cityName, DateOnly day)
        {
            var cityData = await GetCoordinatesAsync(cityName);
            var city = _jsonProcessor.ProcessJsonCityData(cityData);
            var solarData = await GetSolarDataAsync(day, city);
            var sunsetSunriseData = _jsonProcessor.ProcessJsonSolarData(solarData, day);

            if (city.Country == "HU")
            {
                SunsetSunriseData localSolarData = new SunsetSunriseData()
                {
                    Date = day.ToDateTime(TimeOnly.Parse("10:00 PM")),
                    Sunrise = sunsetSunriseData.Sunrise.AddHours(2),
                    Sunset = sunsetSunriseData.Sunset.AddHours(2),
                };

                city.SunsetSunriseDataList.Add(localSolarData);
            }
            else
            {
                city.SunsetSunriseDataList.Add(sunsetSunriseData);
            }

            return city;
        }
    }
}
