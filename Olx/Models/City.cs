using Newtonsoft.Json;

namespace Olx.Models;

public class City
{
    [JsonProperty("Ref")]
    public string Id { get; set; }
    
    // Lat and lon - ?
    
    [JsonProperty("Description")]
    public string Name { get; set; }
    
    [JsonProperty("SettlementTypeDescription")]
    public string Type { get; set; }
    
    [JsonProperty("Area")]
    public string RegionId { get; set; }
    
    [JsonProperty("AreaDescription")]
    public string Region { get; set; }
}