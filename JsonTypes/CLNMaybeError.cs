using System.Text.Json.Serialization;

namespace payto.JsonTypes;

[JsonSerializable(typeof(CLNMaybeError))]
public class CLNMaybeError
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
