using System.Net.Http.Headers;
using Newtonsoft.Json;
using Olx.Models;
using Olx.Services.Abstract;

namespace Olx.Services.Implementation;

public class NovaPostCityProvider : ICityProvider
{
    private readonly IConfiguration _configuration;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public NovaPostCityProvider(IConfiguration configuration)
    {
        _configuration = configuration;
        _apiKey = _configuration["NovaPostApi:ApiKey"] ?? throw new ArgumentNullException("ApiKey is required");
        _baseUrl = _configuration["NovaPostApi:BaseUrl"] ?? throw new ArgumentNullException("BaseUrl is required");
    }

    public async Task<IEnumerable<Region>> GetRegionsAsync()
    {
        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl);
        request.Content = new StringContent(JsonConvert.SerializeObject(new
        {
            apiKey = _apiKey,
            modelName = "Address",
            calledMethod = "getSettlementAreas",
            methodProperties = new { }
        }));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<NovaPostApiResponse<Region>>(content);
        if (!result?.Success ?? true)
        {
            throw new Exception("Failed to get regions");
        }

        return result.Data;
    }

    public async Task<IEnumerable<City>> GetCitiesByRegionAsync(string regionId, int page = 1)
    {
        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl);
        request.Content = new StringContent(JsonConvert.SerializeObject(new
        {
            apiKey = _apiKey,
            modelName = "Address",
            calledMethod = "getSettlements",
            methodProperties = new
            {
                Page = page,
                Warehouse = "0",
                AreaRef = regionId
            }
        }));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<NovaPostApiResponse<City>>(content);
        if (!result?.Success ?? true)
        {
            throw new Exception("Failed to get cities");
        }

        return result.Data;
    }

    public async Task<IEnumerable<City>> GetCitiesAsync(int page, string? q = null)
    {
        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl);
        request.Content = new StringContent(JsonConvert.SerializeObject(new
        {
            apiKey = _apiKey,
            modelName = "Address",
            calledMethod = "getSettlements",
            methodProperties = new
            {
                Page = page,
                Warehouse = "0",
                FindByString = q
            }
        }));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<NovaPostApiResponse<City>>(content);
        if (!result?.Success ?? true)
        {
            throw new Exception("Failed to get cities");
        }

        return result.Data;
    }

    public async Task<City> GetCityByIdAsync(string cityId)
    {
        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl);
        request.Content = new StringContent(JsonConvert.SerializeObject(new
        {
            apiKey = _apiKey,
            modelName = "Address",
            calledMethod = "getSettlements",
            methodProperties = new
            {
                Warehouse = "0",
                Ref = cityId
            }
        }));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<NovaPostApiResponse<City>>(content);
        if (!result?.Success ?? true)
        {
            throw new Exception("Failed to get cities");
        }

        return result.Data.First();
    }

    public async Task<Region> GetRegionByIdAsync(string regionId)
    {
        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl);
        request.Content = new StringContent(JsonConvert.SerializeObject(new
        {
            apiKey = _apiKey,
            modelName = "Address",
            calledMethod = "getSettlementAreas",
            methodProperties = new { }
        }));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<NovaPostApiResponse<Region>>(content);
        if (!result?.Success ?? true)
        {
            throw new Exception("Failed to get regions");
        }

        return result.Data.First(a => a.Id == regionId);
    }
}