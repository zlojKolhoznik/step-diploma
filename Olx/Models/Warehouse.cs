using Newtonsoft.Json;

namespace Olx.Models;

public class Warehouse
{
    [JsonProperty("Ref")]
    public string Id { get; set; }

    [JsonProperty("Number")]
    public int Number { get; set; }

    [JsonProperty("CityRef")]
    public string CityId { get; set; }

    [JsonProperty("ShortAddress")]
    public string ShortAddress { get; set; }

    [JsonProperty("TypeOfWarehouse")]
    public string WarehouseType { get; set; }

    [JsonProperty("Description")]
    public string Description { get; set; }

    [JsonProperty("TotalMaxWeightAllowed")]
    public string TotalMaxWeight { get; set; }
}