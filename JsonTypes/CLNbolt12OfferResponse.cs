using System.Text.Json.Serialization;

namespace payto.JsonTypes;

[JsonSerializable(typeof(CLNbolt12OfferResponse))]
public class CLNbolt12OfferResponse
{
    /// <summary>
    /// The BOLT12 invoice we fetched.
    /// </summary>
    [JsonPropertyName("invoice")] 
    public string? Invoice {  get; set; }
}
