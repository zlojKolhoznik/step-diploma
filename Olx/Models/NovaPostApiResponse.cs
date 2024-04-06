using System.Text.Json.Serialization;

namespace Olx.Models;

public class NovaPostApiResponse<T>
{

    [JsonPropertyName("success")] 
    public bool Success { get; set; }

    [JsonPropertyName("data")] 
    public IList<T> Data { get; set; }

    [JsonPropertyName("errors")] 
    public IList<object> Errors { get; set; }

    [JsonPropertyName("warnings")]
    public IList<object> Warnings { get; set; }
    
    [JsonPropertyName("messageCodes")]
    public IList<object> MessageCodes { get; set; }

    [JsonPropertyName("errorCodes")] 
    public IList<object> ErrorCodes { get; set; }

    [JsonPropertyName("warningCodes")]
    public IList<object> WarningCodes { get; set; }

    [JsonPropertyName("infoCodes")] 
    public IList<object> InfoCodes { get; set; }
}
