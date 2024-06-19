using Newtonsoft.Json;

namespace Olx.Models;

public class Region
{
    [JsonProperty("Ref")]
    public string Id { get; set; }
    
    [JsonProperty("AreasCenter")]
    public string CenterId { get; set; }
    
    [JsonProperty("Description")]
    public string Name { get; set; }
}