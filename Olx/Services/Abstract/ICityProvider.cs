using Olx.Models;

namespace Olx.Services.Abstract;

public interface ICityProvider
{
    public Task<IEnumerable<Region>> GetRegionsAsync();
    public Task<IEnumerable<City>> GetCitiesByRegionAsync(string regionId, int page);
    public Task<IEnumerable<City>> GetCitiesAsync(int page, string? q = null);
    public Task<City> GetCityByIdAsync(string cityId);
    public Task<Region> GetRegionByIdAsync(string regionId);
    
    // TODO: Робота з відділеннями Нової Пошти
}