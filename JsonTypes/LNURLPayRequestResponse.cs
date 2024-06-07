using System.Text.Json.Serialization;

namespace payto.JsonTypes;

/// <summary>
/// https://github.com/lnurl/luds/blob/luds/06.md
/// LNURLPAY works by making 2 GET REQUESTS
/// This is response from first GET request (step 2. in LUD 06): "how to ask for bolt11 invoice" ("LN WALLET makes a GET request to LN SERVICE Uri using the decoded LNURL.")
/// This is the first response from such LN SERVICE, telling us how to query for bolt11 invoice - send the second GET request to "callback" - 
/// (this time with amount & optionally comment) to get back "LNURLPayRequestCallbackResponse" containing bolt11 invoice
/// </summary>
[JsonSerializable(typeof(LNURLPayServiceResponse))]
public record LNURLPayServiceResponse : LNURLPayError
{
    /// <summary>
    /// The URL from LN SERVICE which will accept the pay request parameters
    /// </summary>
    public string? callback { get; set; }

    /// <summary>
    /// // Max millisatoshi amount LN SERVICE is willing to receive
    /// </summary>
    public ulong maxSendable { get; set; }

    /// <summary>
    /// // Min millisatoshi amount LN SERVICE is willing to receive, can not be less than 1 or more than `maxSendable`
    /// </summary>
    public ulong minSendable { get; set; }

    /// <summary>
    /// // Metadata json which must be presented as raw string here, this is required to pass signature verification at a later step
    /// </summary>
    public string? metadata { get; set; }

    /// <summary>
    /// //"payRequest" // Type of LNURL
    /// </summary>
    public string? tag { get; set; }

    /// <summary>
    /// length of optional message from sender (payer) to reciever (payee)
    /// if commentAllowed == 0, comments are not supported
    /// </summary>
    public int commentAllowed { get; set; }
    public string? nostrPubkey { get; set; }
    public bool allowsNostr { get; set; }
}

[JsonSerializable(typeof(LNURLPayError))]
public record LNURLPayError
{
    //{"status": "ERROR", "reason": "error details..."}

    public string? status { get; set; }

    public string? reason { get; set; }
}
