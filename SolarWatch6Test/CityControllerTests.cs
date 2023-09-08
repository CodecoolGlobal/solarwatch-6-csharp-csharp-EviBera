using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SolarWatch6.Controllers;
using SolarWatch6.Models;
using SolarWatch6.Services;
using SolarWatch6.Services.Repository;

namespace SolarWatch6Test
{
    [TestFixture]
    public class CityControllerTests
    {
        private Mock<ILogger<CityController>> _loggerMock;
        private Mock<ICityService> _cityServiceMock;
        private Mock<IJsonProcessor> _jsonProcessorMock;
        private Mock<ICityRepository> _cityRepositoryMock;
        private Mock<ISolarDataRepository> _sunsetSunriseDataRepositoryMock;
        private CityController _controller;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<CityController>>();
            _cityServiceMock = new Mock<ICityService>();
            _jsonProcessorMock = new Mock<IJsonProcessor>();
            _cityRepositoryMock = new Mock<ICityRepository>();
            _sunsetSunriseDataRepositoryMock = new Mock<ISolarDataRepository>();
            _controller = new CityController(_loggerMock.Object, _cityServiceMock.Object, _jsonProcessorMock.Object,
                _cityRepositoryMock.Object, _sunsetSunriseDataRepositoryMock.Object);
        }

        [Test]
        public async Task GetAsync_WhenCityExistsAndSolarDataAvailable_ShouldReturnOkResult()
        {
            // Arrange
            var cityName = "TestCity";
            var day = new DateOnly(2023, 9, 7);

            var city = new City() { Id = 100, CityName = "TestCity", Lat = 33.33, Lon = 33.33, Country = "TestCountry"};
            var solarData = new SunsetSunriseData { Id = 100, Date = new DateTime(2023, 9, 7), 
                Sunrise = new DateTime(2023, 9, 7, 6, 0, 0), Sunset = new DateTime(2023, 9, 7, 19, 30, 0), CityId = 100 };

            _cityRepositoryMock.Setup(repo => repo.GetByNameAsync(cityName))
                .ReturnsAsync(city);
            _sunsetSunriseDataRepositoryMock.Setup(repo => repo.GetByDateAndCityAsync(day, city.Id))
                .ReturnsAsync(solarData);

            // Act
            var result = await _controller.GetAsync(cityName, day);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = (OkObjectResult)result.Result;
            Assert.IsInstanceOf<CityWithSolarData>(okResult.Value);
        }

        [Test]
        public async Task GetAsyncReturnsNotFoundResult_IfCityIsNotAvailable()
        {

            var cityName = "NonExistentCity";
            
            _cityRepositoryMock.Setup(repo => repo.GetByNameAsync(cityName))
                .ReturnsAsync((City)null);
            
            // Act
            var result = await _controller.GetAsync(cityName, new DateOnly(2023, 9, 7));

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);

        }
        

        [Test]
        public async Task GetAsyncReturnsNotFoundResult_IfSolarDataAreNotAvailable()
        {

            var cityName = "TestCity";
            var day = new DateOnly(2023, 9, 7);

            var city = new City() { Id = 100, CityName = "TestCity", Lat = 33.33, Lon = 33.33, Country = "TestCountry" };
            SunsetSunriseData solarData = null;

            _cityRepositoryMock.Setup(repo => repo.GetByNameAsync(cityName))
                .ReturnsAsync(city);
            _sunsetSunriseDataRepositoryMock.Setup(repo => repo.GetByDateAndCityAsync(day, city.Id))
                .ReturnsAsync(solarData);

            // Act
            var result = await _controller.GetAsync(cityName, day);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);

        }

        
    }
}