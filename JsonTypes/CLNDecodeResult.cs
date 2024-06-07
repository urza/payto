using System.Text.Json.Serialization;

namespace payto.JsonTypes;

/// <summary>
/// Added manually from docs. Just to have the strings of Decode result Types on one place
/// https://docs.corelightning.org/reference/lightning-decode
/// 
/// type(string) (one of "bolt12 offer", 
/// "bolt12 invoice", "bolt12 invoice_request", 
/// "bolt11 invoice", "rune", "emergency recover"): 
/// What kind of object it decoded to.
/// </summary>
public class DecodeResultTypes
{
    public static string bolt12_offer = "bolt12 offer";
    public static string bolt12_invoice = "bolt12 invoice";
    public static string bolt12_invoice_request = "bolt12 invoice_request";

    public static string bolt11_invoice = "bolt11 invoice";

    public static string rune = "rune";
    public static string emergency_recover = "emergency recover";
}

[JsonSerializable(typeof(CmdDecodeResponse))]
public class CmdDecodeResponse : CLNMaybeError
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("valid")]
    public bool Valid { get; set; }

    // Bolt12 Offer properties
    [JsonPropertyName("offer_id")]
    public string OfferId { get; set; }

    [JsonPropertyName("offer_description")]
    public string OfferDescription { get; set; }

    [JsonPropertyName("offer_node_id")]
    public string OfferNodeId { get; set; }

    [JsonPropertyName("offer_chains")]
    public List<string> OfferChains { get; set; }

    [JsonPropertyName("offer_metadata")]
    public string OfferMetadata { get; set; }

    [JsonPropertyName("offer_currency")]
    public string OfferCurrency { get; set; }

    [JsonPropertyName("currency_minor_unit")]
    public uint CurrencyMinorUnit { get; set; }

    [JsonPropertyName("offer_amount")]
    public ulong OfferAmount { get; set; }

    [JsonPropertyName("offer_amount_msat")]
    public ulong OfferAmountMsat { get; set; }

    [JsonPropertyName("offer_issuer")]
    public string OfferIssuer { get; set; }

    [JsonPropertyName("offer_features")]
    public string OfferFeatures { get; set; }

    [JsonPropertyName("offer_absolute_expiry")]
    public ulong OfferAbsoluteExpiry { get; set; }

    [JsonPropertyName("offer_quantity_max")]
    public ulong OfferQuantityMax { get; set; }

    [JsonPropertyName("offer_paths")]
    public List<OfferPath> OfferPaths { get; set; }

    [JsonPropertyName("offer_recurrence")]
    public OfferRecurrence OfferRecurrence { get; set; }

    [JsonPropertyName("unknown_offer_tlvs")]
    public List<UnknownTlv> UnknownOfferTlvs { get; set; }

    // Bolt12 Invoice Request properties
    [JsonPropertyName("invreq_metadata")]
    public string InvreqMetadata { get; set; }

    [JsonPropertyName("invreq_payer_id")]
    public string InvreqPayerId { get; set; }

    [JsonPropertyName("signature")]
    public string Signature { get; set; }

    [JsonPropertyName("invreq_chain")]
    public string InvreqChain { get; set; }

    [JsonPropertyName("invreq_amount_msat")]
    public ulong InvreqAmountMsat { get; set; }

    [JsonPropertyName("invreq_features")]
    public string InvreqFeatures { get; set; }

    [JsonPropertyName("invreq_quantity")]
    public ulong InvreqQuantity { get; set; }

    [JsonPropertyName("invreq_payer_note")]
    public string InvreqPayerNote { get; set; }

    [JsonPropertyName("invreq_recurrence_counter")]
    public uint InvreqRecurrenceCounter { get; set; }

    [JsonPropertyName("invreq_recurrence_start")]
    public uint InvreqRecurrenceStart { get; set; }

    [JsonPropertyName("unknown_invoice_request_tlvs")]
    public List<UnknownTlv> UnknownInvoiceRequestTlvs { get; set; }

    // Bolt12 Invoice properties
    [JsonPropertyName("invoice_paths")]
    public List<InvoicePath> InvoicePaths { get; set; }

    [JsonPropertyName("invoice_created_at")]
    public ulong InvoiceCreatedAt { get; set; }

    [JsonPropertyName("invoice_payment_hash")]
    public string InvoicePaymentHash { get; set; }

    [JsonPropertyName("invoice_amount_msat")]
    public ulong InvoiceAmountMsat { get; set; }

    [JsonPropertyName("invoice_relative_expiry")]
    public uint InvoiceRelativeExpiry { get; set; }

    [JsonPropertyName("invoice_fallbacks")]
    public List<InvoiceFallback> InvoiceFallbacks { get; set; }

    [JsonPropertyName("invoice_features")]
    public string InvoiceFeatures { get; set; }

    [JsonPropertyName("invoice_node_id")]
    public string InvoiceNodeId { get; set; }

    [JsonPropertyName("invoice_recurrence_basetime")]
    public ulong InvoiceRecurrenceBasetime { get; set; }

    [JsonPropertyName("unknown_invoice_tlvs")]
    public List<UnknownTlv> UnknownInvoiceTlvs { get; set; }

    // Bolt11 Invoice properties
    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    [JsonPropertyName("created_at")]
    public ulong CreatedAt { get; set; }

    [JsonPropertyName("expiry")]
    public ulong Expiry { get; set; }

    [JsonPropertyName("payee")]
    public string Payee { get; set; }

    [JsonPropertyName("payment_hash")]
    public string PaymentHash { get; set; }

    [JsonPropertyName("min_final_cltv_expiry")]
    public uint MinFinalCltvExpiry { get; set; }

    [JsonPropertyName("amount_msat")]
    public ulong AmountMsat { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("description_hash")]
    public string DescriptionHash { get; set; }

    [JsonPropertyName("payment_secret")]
    public string PaymentSecret { get; set; }

    [JsonPropertyName("payment_metadata")]
    public string PaymentMetadata { get; set; }

    [JsonPropertyName("fallbacks")]
    public List<Bolt11Fallback> Fallbacks { get; set; }

    [JsonPropertyName("routes")]
    public List<List<RouteHint>> Routes { get; set; }

    [JsonPropertyName("extra")]
    public List<Bolt11Extra> Extra { get; set; }

    // Rune properties
    [JsonPropertyName("string")]
    public string String { get; set; }

    [JsonPropertyName("restrictions")]
    public List<RuneRestriction> Restrictions { get; set; }

    [JsonPropertyName("unique_id")]
    public string UniqueId { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("hex")]
    public string Hex { get; set; }

    // Emergency Recover properties
    [JsonPropertyName("decrypted")]
    public string Decrypted { get; set; }
}

public class OfferPath
{
    [JsonPropertyName("first_node_id")]
    public string FirstNodeId { get; set; }

    [JsonPropertyName("blinding")]
    public string Blinding { get; set; }

    [JsonPropertyName("path")]
    public List<BlindedPath> Path { get; set; }
}

public class BlindedPath
{
    [JsonPropertyName("blinded_node_id")]
    public string BlindedNodeId { get; set; }

    [JsonPropertyName("encrypted_recipient_data")]
    public string EncryptedRecipientData { get; set; }
}

public class OfferRecurrence
{
    [JsonPropertyName("time_unit")]
    public uint TimeUnit { get; set; }

    [JsonPropertyName("period")]
    public uint Period { get; set; }

    [JsonPropertyName("time_unit_name")]
    public string TimeUnitName { get; set; }

    [JsonPropertyName("basetime")]
    public ulong Basetime { get; set; }

    [JsonPropertyName("start_any_period")]
    public bool StartAnyPeriod { get; set; }

    [JsonPropertyName("limit")]
    public uint Limit { get; set; }

    [JsonPropertyName("paywindow")]
    public PayWindow PayWindow { get; set; }
}

public class PayWindow
{
    [JsonPropertyName("seconds_before")]
    public uint SecondsBefore { get; set; }

    [JsonPropertyName("seconds_after")]
    public uint SecondsAfter { get; set; }

    [JsonPropertyName("proportional_amount")]
    public bool ProportionalAmount { get; set; }
}

public class InvoicePath
{
    [JsonPropertyName("first_node_id")]
    public string FirstNodeId { get; set; }

    [JsonPropertyName("blinding")]
    public string Blinding { get; set; }

    [JsonPropertyName("payinfo")]
    public PayInfo PayInfo { get; set; }

    [JsonPropertyName("path")]
    public List<BlindedPath> Path { get; set; }
}

public class PayInfo
{
    [JsonPropertyName("fee_base_msat")]
    public int FeeBaseMsat { get; set; }

    [JsonPropertyName("fee_proportional_millionths")]
    public uint FeeProportionalMillionths { get; set; }

    [JsonPropertyName("cltv_expiry_delta")]
    public uint CltvExpiryDelta { get; set; }

    [JsonPropertyName("features")]
    public string Features { get; set; }
}

public class InvoiceFallback
{
    [JsonPropertyName("version")]
    public byte Version { get; set; }

    [JsonPropertyName("hex")]
    public string Hex { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; }
}

public class Bolt11Fallback
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("hex")]
    public string Hex { get; set; }

    [JsonPropertyName("addr")]
    public string Addr { get; set; }
}

public class RouteHint
{
    [JsonPropertyName("pubkey")]
    public string Pubkey { get; set; }

    [JsonPropertyName("short_channel_id")]
    public string ShortChannelId { get; set; }

    [JsonPropertyName("fee_base_msat")]
    public int FeeBaseMsat { get; set; }

    [JsonPropertyName("fee_proportional_millionths")]
    public uint FeeProportionalMillionths { get; set; }

    [JsonPropertyName("cltv_expiry_delta")]
    public uint CltvExpiryDelta { get; set; }
}

public class Bolt11Extra
{
    [JsonPropertyName("tag")]
    public string Tag { get; set; }

    [JsonPropertyName("data")]
    public string Data { get; set; }
}

public class RuneRestriction
{
    [JsonPropertyName("alternatives")]
    public List<string> Alternatives { get; set; }

    [JsonPropertyName("summary")]
    public string Summary { get; set; }
}

public class UnknownTlv
{
    [JsonPropertyName("type")]
    public ulong Type { get; set; }

    [JsonPropertyName("length")]
    public ulong Length { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }
}
