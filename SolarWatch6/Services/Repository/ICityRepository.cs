﻿using SolarWatch6.Models;

namespace SolarWatch6.Services.Repository
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetAllAsync();
        Task<City?> GetByNameAsync(string name);

        Task<City> AddAsync(City city);
        Task<City> DeleteByIdAsync(int cityId);
        Task<City> UpdateAsync(int id, CityUpdateDTO cityDto);


    }
}
