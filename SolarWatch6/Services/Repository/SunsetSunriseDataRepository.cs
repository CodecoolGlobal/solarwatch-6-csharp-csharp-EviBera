using Microsoft.EntityFrameworkCore;
using SolarWatch6.Data;
using SolarWatch6.Models;
using System.Reflection.Metadata.Ecma335;

namespace SolarWatch6.Services.Repository
{
    public class SunsetSunriseDataRepository : ISolarDataRepository
    {
        private readonly SolarWatchContext _dbContext;

        public SunsetSunriseDataRepository(SolarWatchContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SunsetSunriseData> AddSolarDataAsync(SunsetSunriseData data)
        {
            await _dbContext.SunsetSunriseDataCollection.AddAsync(data);
            await _dbContext.SaveChangesAsync();
            return data;
        }

        public async Task<IEnumerable<SunsetSunriseData>> GetAllByCityIdAsync(int cityId)
        {
            var solarDataByCityId = await _dbContext
                .SunsetSunriseDataCollection.Where(SunsetSunriseData => SunsetSunriseData.CityId == cityId).ToListAsync();
            return solarDataByCityId;
        }

        public async Task<SunsetSunriseData> GetByDateAndCityAsync(DateOnly date, int cityId)
        {
            var solarData = await _dbContext.SunsetSunriseDataCollection
                .FirstOrDefaultAsync(ssd => ssd.CityId == cityId && ssd.Date == date.ToDateTime(TimeOnly.Parse("10:00 PM")));
            return solarData;
        }

        public async Task<ICollection<SunsetSunriseData>> DeleteByCityIdAsync(int cityId)
        {
            var collectionOfDataToDelete = await _dbContext.SunsetSunriseDataCollection.Where(ssd => ssd.CityId == cityId)
                .ToListAsync();

            foreach(var item in collectionOfDataToDelete)
            {
                _dbContext.SunsetSunriseDataCollection.Remove(item);
            }

            await _dbContext.SaveChangesAsync();

            return collectionOfDataToDelete;
        }

        public async Task<SunsetSunriseData> UpdateSolarDataByIdAsync(int solarDataId, SolarDataDTO newData)
        {
            var solarDataToUpdate = await _dbContext.SunsetSunriseDataCollection.FirstOrDefaultAsync(ssd => ssd.Id == solarDataId);

            if (solarDataToUpdate == null)
            {
                return null;
            }

            solarDataToUpdate.Date = newData.Date;
            solarDataToUpdate.Sunset = newData.Sunset;
            solarDataToUpdate.Sunrise = newData.Sunrise;
            solarDataToUpdate.CityId = newData.CityId;

            await _dbContext.SaveChangesAsync();

            return solarDataToUpdate;
        }

        public async Task<SunsetSunriseData> DeleteSolarDataByIdAsync(int solarDataId)
        {
            var solarDataToDelete = await _dbContext.SunsetSunriseDataCollection.FirstOrDefaultAsync(ssd => ssd.Id == solarDataId);

            if (solarDataToDelete == null)
            {
                return null;
            }

            _dbContext.SunsetSunriseDataCollection.Remove(solarDataToDelete);
            await _dbContext.SaveChangesAsync();
            return solarDataToDelete;
        }
    }
}
