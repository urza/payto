using LNURL;
using System.Text.Json;
using System.Net.Http.Json;
using payto.JsonTypes;
using ExtensionMethods;

namespace payto;

/// <summary>
/// Bacic flow of paying to LNURL or Lightning Address
/// Implements only LUD-06: "payRequest base spec" and LUD-12: "Comments in payRequest"
/// </summary>
internal class PayToLNURL
{
    /// <summary>
    /// For now only LNURL based address
    /// </summary>
    /// <param name="destination">user@domain.tld => either http&LNURL based or DNS_BIP353 based</param>
    public static async Task PayToAddressAsync(string destination)
    {
        Console.WriteLine("This is lightning address.");

        var lnurl_service = LNURLPay.ExtractUriFromLightningAddress(destination);

        Console.WriteLine($"LN Service Uri:  {lnurl_service.ToString()}");

        await PayUsingLNServiceAsync(lnurl_service);
    }

    public static async Task PayToLNURLAsync(string destination)
    {
        Console.WriteLine("This is LNURL PAY");

        var parsed = LNURLPay.TryParse(destination);

        if (!parsed.success)
        {
            Console.WriteLine($"ERROR PARSING {destination}");
            return;
        }

        var lnurl_service = parsed.LnService;
        Console.WriteLine($"LN Service Uri:  {lnurl_service.ToString()}");

        await PayUsingLNServiceAsync(lnurl_service);
    }

    private static async Task PayUsingLNServiceAsync(Uri lnService)
    {
        /// First LNURL PAY response to get info about how much we can pay and where to call for bolt11 invoice
        LNURLPayServiceResponse response1 = await CallLnServiceAsync(lnService);

        /// get amount and note from user
        ulong amount_to_send_millisatoshi = GetAmount(response1);
        string? payer_note = GetPayerComment(response1);

        Uri request2 = BuildRequest2(response1, amount_to_send_millisatoshi, payer_note);

        /// LNURL PAY - second GET request to get the Bolt11 invoice
        (LNURLPayRequestCallbackResponse? response2, string bolt11invoice) = await PayRequestAsync(request2);

        Console.WriteLine($"Got bolt11 invoice: {bolt11invoice}");
        Console.WriteLine("Parsing...");
        var bolt11decoded = ParseBolt11Invoice(bolt11invoice);

        /// TODO
        /// https://github.com/lnurl/luds/blob/luds/06.md
        /// 7. LN WALLET Verifies that h tag in provided invoice is a hash of metadata string converted to byte array in UTF - 8 encoding.

        /// 8. LN WALLET Verifies that amount in provided invoice equals the amount previously specified by user.
        if (bolt11decoded.AmountMsat != amount_to_send_millisatoshi)
            throw new Exception($"Amount in invoice is not the same as amount we want to send. In millisatoshi: {bolt11decoded.AmountMsat} vs {amount_to_send_millisatoshi}");

        var pay = ConfirmPay(bolt11decoded);

        if (!pay)
        {
            Console.WriteLine("CANCELED!");
            return;
        }

        Console.WriteLine("Paying invoice..");
        var payresult = RunCli.ExecuteLightnigCli($"pay {bolt11invoice}");
        Console.WriteLine(payresult);
    }

    private static async Task<LNURLPayServiceResponse> CallLnServiceAsync(Uri lnService)
    {
        //Console.WriteLine($"calling LN SERVICE at: {lnService?.ToString()}");
        
        HttpClient httpClient = new HttpClient();

        var response = await httpClient.GetFromJsonAsync<LNURLPayServiceResponse>(lnService, SourceGenerationContextPayto.Default.LNURLPayServiceResponse);

        Console.WriteLine();
        //Console.WriteLine($"response1: {response1}");

        if (response is null)
            throw new Exception("response from LnService is null");

        if (response?.callback?.IsEmpty() == true)
            throw new Exception("LN service returned empty callback");

        return response!;
    }

    private static ulong GetAmount(LNURLPayServiceResponse response1)
    {
        var min_sendable_sat = response1!.minSendable / 1000; // show to user in sats
        var max_sendable_sat = response1.maxSendable / 1000;

        ConsoleHelper.WriteLine($"You can send between:", ConsoleColor.DarkYellow);
        Console.Write("Min sendable: "); BtcSatFormat.PrintSatToLNBtc(min_sendable_sat);
        Console.Write("Max sendable: "); BtcSatFormat.PrintSatToLNBtc(max_sendable_sat);
        ConsoleHelper.WriteLine("How much do you want to send? (in sats) (space and _ allowed for visual separation):", ConsoleColor.DarkYellow);

        if (!ulong.TryParse(Console.ReadLine()!.Trim().Replace(" ", "").Replace("_", ""), out var amounttosend_sat))
            throw new Exception("Amount inputted is not a number");

        if (amounttosend_sat < min_sendable_sat || amounttosend_sat > max_sendable_sat)
            throw new ArgumentOutOfRangeException("Amount is not between min sendable and max sendable");

        return amounttosend_sat * 1000; // return in millisathosi
    }
    private static string? GetPayerComment(LNURLPayServiceResponse response1)
    {
        /// if we can add payment comment
        if (response1.commentAllowed > 0)
        {
            ConsoleHelper.WriteLine($"You can add comment/note for the payee up to {response1.commentAllowed} charactes. (Press enter for skip).",
                                ConsoleColor.DarkYellow);

            var comment = Console.ReadLine();

            if (!string.IsNullOrEmpty(comment))
            {
                if (comment.Length > response1.commentAllowed)
                    comment = comment.Substring(0, response1.commentAllowed);

                return comment;
            }
        }

        return null;
    }

    private static Uri BuildRequest2(LNURLPayServiceResponse response1, ulong amount_to_send_millisatoshi, string? payer_note)
    {
        var req2 = new UriBuilder(response1.callback);

        LNURLPay.AppendPayloadToQuery(req2, "amount", amount_to_send_millisatoshi.ToString());

        if (payer_note != null)
        {
            LNURLPay.AppendPayloadToQuery(req2, "comment", payer_note);
        }

        return req2.Uri;
    }

    private static async Task<(LNURLPayRequestCallbackResponse? response2, string bolt11invoice)> PayRequestAsync(Uri request2)
    {
        HttpClient httpClient = new();
        LNURLPayRequestCallbackResponse? response2 = await httpClient.GetFromJsonAsync<LNURLPayRequestCallbackResponse>(request2, SourceGenerationContextPayto.Default.LNURLPayRequestCallbackResponse);

        if (response2 is null)
            throw new Exception("response2 is null");

        if (string.IsNullOrEmpty(response2.Pr))
            throw new Exception("response.pr is empty - no bolt11 invoice");

        var bolt11invoice = response2.Pr;

        return (response2, bolt11invoice);
    }
    
    private static CmdDecodeResponse ParseBolt11Invoice(string bolt11invoice)
    {
        var json_res = RunCli.ExecuteLightnigCli($"decode {bolt11invoice}");

        Console.WriteLine(json_res);
        var decoded = JsonSerializer.Deserialize<CmdDecodeResponse>(json_res, SourceGenerationContextPayto.Default.CmdDecodeResponse);

        if (decoded is null)
            throw new ArgumentNullException("decoded is null");

        if (!decoded.Valid)
            throw new Exception("CLN says not valid bolt11 invoice.");

        if (decoded.Type != DecodeResultTypes.bolt11_invoice)
            throw new Exception("Decoded type is not botl11 invoice");

        return decoded;
    }

    private static bool ConfirmPay(CmdDecodeResponse bolt11decoded)
    {
        Console.WriteLine("pay this bolt11 invoice?");
        Console.WriteLine($"payee: {bolt11decoded.Payee}");
        Console.WriteLine($"amount (sat): {bolt11decoded.AmountMsat / 1000}");
        Console.WriteLine($"description: {bolt11decoded.Description}");

        Console.WriteLine();
        ConsoleHelper.WriteLine("Confirm payment (y) or cancel (n)", ConsoleColor.DarkYellow);

        return Console.ReadLine() == "y";
    }
}
