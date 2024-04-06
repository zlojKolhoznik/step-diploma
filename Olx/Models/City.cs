using Newtonsoft.Json;

namespace Olx.Models;

public class City
{
    [JsonProperty("Ref")]
    public string Id { get; set; }
}