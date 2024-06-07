using System.Text.Json.Serialization;

namespace payto.JsonTypes;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(LNURLPayError))]
[JsonSerializable(typeof(LNURLPayServiceResponse))]
[JsonSerializable(typeof(LNURLPayRequestCallbackResponse))]
[JsonSerializable(typeof(CmdDecodeResponse))] 
[JsonSerializable(typeof(CLNbolt12OfferResponse))]
public partial class SourceGenerationContextPayto : JsonSerializerContext
{
}
