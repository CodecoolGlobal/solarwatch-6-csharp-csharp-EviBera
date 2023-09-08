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

    }
}
