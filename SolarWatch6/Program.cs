using SolarWatch6.Data;
using SolarWatch6.Models;
using SolarWatch6.Services;
using SolarWatch6.Services.Repository;
using SolarWatch6.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ICityService, CityService>();
builder.Services.AddSingleton<IJsonProcessor, JsonProcessor>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ISolarDataRepository, SunsetSunriseDataRepository>();
builder.Services.AddDbContext<SolarWatchContext>(ServiceLifetime.Transient);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


PrintCities();

void PrintCities()
{
    using var db = new SolarWatchContext();
    foreach (var city in db.Cities)
    {
        Console.WriteLine($"{city.Id}, {city.CityName}, {city.Lat}, {city.Lon}, {city.Country}");
    }
}

app.Run();
