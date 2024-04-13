using Olx.Models;

namespace Olx.Services.Abstract;

public interface IDeliveryProvider
{
    public Task<IEnumerable<Region>> GetRegionsAsync();
    public Task<IEnumerable<City>> GetCitiesByRegionAsync(string regionId);
    public Task<IEnumerable<City>> GetCitiesAsync(string? q = null);
    public Task<City> GetCityByIdAsync(string cityId);
    public Task<Region> GetRegionByIdAsync(string regionId);
    public Task<IEnumerable<Warehouse>> GetWarehousesByCityAsync(string cityId);
    public Task<Warehouse> GetWarehouseByNumberAsync(string cityId, string q);
}