using System.Text.Json.Serialization;

namespace Olx.Models;

public class Region
{
    [JsonPropertyName("Ref")]
    public string Id { get; set; }
    
    [JsonPropertyName("AreasCenter")]
    public string CenterId { get; set; }
    
    [JsonPropertyName("Description")]
    public string Name { get; set; }
}