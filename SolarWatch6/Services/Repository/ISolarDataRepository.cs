using SolarWatch6.Models;

namespace SolarWatch6.Services.Repository
{
    public interface ISolarDataRepository
    {
        Task<IEnumerable<SunsetSunriseData>> GetAllByCityIdAsync(int cityId);
        Task<SunsetSunriseData> AddSolarDataAsync(SunsetSunriseData data);
        Task<SunsetSunriseData> GetByDateAndCityAsync(DateOnly date, int cityId);
        Task<ICollection<SunsetSunriseData>> DeleteByCityIdAsync(int cityId);
        Task<SunsetSunriseData> UpdateSolarDataByIdAsync(int solarDataId, SolarDataDTO newData);
        Task<SunsetSunriseData> DeleteSolarDataByIdAsync(int solarDataId);
    }
}
