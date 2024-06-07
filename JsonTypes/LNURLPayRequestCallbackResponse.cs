using System.Text.Json.Serialization;

namespace payto.JsonTypes;

/// <summary>
/// /// https://github.com/lnurl/luds/blob/luds/06.md
/// LNURLPAY works by making 2 GET REQUESTS
/// This is response from the second GET request containging bolt11 invoice in "pr" field
/// </summary>
[JsonSerializable(typeof(LNURLPayRequestCallbackResponse))]
public class LNURLPayRequestCallbackResponse
{
    [JsonPropertyName("pr")] 
    public string? Pr { get; set; }

    [JsonPropertyName("routes")] 
    public string[] Routes { get; set; } = Array.Empty<string>();
        
}