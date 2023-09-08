using Microsoft.EntityFrameworkCore;
using SolarWatch6.Models;

namespace SolarWatch6.Data
{
    public class SolarWatchContext : DbContext
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<SunsetSunriseData> SunsetSunriseDataCollection { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=localhost,1433;Database=SolarWatch;User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the City entity
            modelBuilder.Entity<City>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<City>()
                .HasMany(c => c.SunsetSunriseDataList)
                .WithOne(ssd => ssd.City)
                .HasForeignKey(ssd => ssd.CityId);

            // Configure the SunsetSunriseData entity
            modelBuilder.Entity<SunsetSunriseData>()
                .HasKey(ssd => ssd.Id);

            modelBuilder.Entity<SunsetSunriseData>()
                .HasOne(ssd => ssd.City)
                .WithMany(c => c.SunsetSunriseDataList)
                .HasForeignKey(ssd => ssd.CityId);

            modelBuilder.Entity<City>()
                .HasData(
                    new City { Id = 1, CityName = "London", Lat = 51.509865, Lon = -0.118092, Country = "UK" },
                    new City { Id = 2, CityName = "Budapest", Lat = 47.497913, Lon = 19.040236, Country = "HU" },
                    new City { Id = 3, CityName = "Paris", Lat = 48.864716, Lon = 2.349014, Country = "FR" }
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
