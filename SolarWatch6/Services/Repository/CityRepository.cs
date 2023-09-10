using Microsoft.EntityFrameworkCore;
using SolarWatch6.Data;
using SolarWatch6.Models;

namespace SolarWatch6.Services.Repository
{
    public class CityRepository : ICityRepository
    {

        private readonly SolarWatchContext _dbContext;

        public CityRepository(SolarWatchContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<City> AddAsync(City city)
        {
            await _dbContext.Cities.AddAsync(city);
            await _dbContext.SaveChangesAsync();
            return city;
        }

        public async Task<IEnumerable<City>> GetAllAsync()
        {
            return await _dbContext.Cities.ToListAsync();
        }

        public async Task<City?> GetByNameAsync(string name)
        {
            return await _dbContext.Cities.FirstOrDefaultAsync(c => c.CityName  == name);
        }

        public async Task<City> DeleteByIdAsync(int cityId)
        {
            var cityToDelete = await _dbContext.Cities.FirstOrDefaultAsync(c => c.Id == cityId);

            if (cityToDelete == null)
            {
                // City with the specified ID was not found
                return null;
            }

            _dbContext.Cities.Remove(cityToDelete);
            await _dbContext.SaveChangesAsync();
            return cityToDelete;
        }

        public async Task<City> UpdateAsync(int id, CityUpdateDTO cityDto)
        {
            var cityToUpdate = await _dbContext.Cities.FirstOrDefaultAsync(c => c.Id == id);

            if (cityToUpdate == null)
            {
                return null;
            }

            cityToUpdate.CityName = cityDto.CityName;
            cityToUpdate.Lat = cityDto.Lat;
            cityToUpdate.Lon = cityDto.Lon;
            cityToUpdate.Country = cityDto.Country;
            cityToUpdate.State = cityDto.State;
            await _dbContext.SaveChangesAsync();

            return cityToUpdate;
        }

    }
}
