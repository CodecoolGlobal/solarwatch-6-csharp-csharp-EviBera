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
    }
}
