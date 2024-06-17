using payto.JsonTypes;
using System.Text.Json;
using ExtensionMethods;
using payto.Utils;

namespace payto;

/// <summary>
/// Interactively pay to Bolt12 offer flow.
/// All the heavy lifting is done by CLN - this just calls the CLN commads to decode, fetch invoice and then pay,
/// adding interactivity - presenting information to the user and asking for inputs and then confirmation before sending.
/// </summary>
internal class PayToBolt12
{
    public static void PayToOffer(string offer)
    {
        /// use CLN to decode the bolt12 offer
        CmdDecodeResponse decoded_offer = ClnDecode(offer, DecodeResultTypes.bolt12_offer);

        /// show user what is inside the offer
        PrintOfferInfo(decoded_offer);

        /// get amount and note from user
        ulong amount_to_send_millisatoshi = GetAmount(decoded_offer); ;
        string? payer_note = GetPayerComment();

        /// fetch bolt12 invoice
        Console.WriteLine("fetching invoice..");
        (CmdDecodeResponse bolt12_invoice_decoded, string bolt12invoice) = FetchBolt12Invoice(offer, amount_to_send_millisatoshi, payer_note);

        /// show user info about the bolt12 invoice
        PrintBolt12InvoiceInfo(bolt12_invoice_decoded);

        /// better check the amounts to avoid mistake
        if (amount_to_send_millisatoshi != bolt12_invoice_decoded.InvreqAmountMsat ||
           amount_to_send_millisatoshi != bolt12_invoice_decoded.InvoiceAmountMsat)
        {
            ConsoleHelper.WriteLine("Amounts not the same, exitig in panic", ConsoleColor.Red);
            return;
        }

        ConsoleHelper.WriteLine("Do you want to pay this invoice? (y / n)", ConsoleColor.DarkYellow);

        if (Console.ReadLine()?.ToLower() is not ("y" or "yes"))
        {
            Console.WriteLine("CANCELED!");
            return;
        }

        Console.WriteLine("Paying invoice...");
        var json_res = RunCli.ExecuteLightnigCli($"pay {bolt12invoice}");
        Console.WriteLine(json_res);
        return;
    }

    private static void PrintOfferInfo(CmdDecodeResponse decoded)
    {
        /// should be "bolt12 offer"
        var type = decoded.Type;
        Console.WriteLine(type);
        Console.WriteLine($"\tvalid: {decoded.Valid}");
        Console.WriteLine($"\tamount msat: {decoded.OfferAmountMsat}");
        Console.WriteLine($"\tamount sat: {(decoded.OfferAmountMsat / 1000).AmountWithSeparators()}");
        Console.WriteLine($"\toffer_description: {decoded.OfferDescription}");
        Console.WriteLine($"\tto node: {decoded.OfferNodeId}");
    }

    private static ulong GetAmount(CmdDecodeResponse decoded)
    {
        ulong amount_to_send_millisatoshi = 0;

        /// amount not specified, user can input anything
        if (decoded.OfferAmountMsat <= 0)
        {
            return InputAmount.GetAmountFromUser(null, null);
        }

        /// amount present in offer - use that
        if (decoded.OfferAmountMsat > 0)
        {
            ConsoleHelper.WriteLine($"Amount (to send) is specified in offer: {decoded.OfferAmountMsat} millisats", ConsoleColor.DarkYellow);
            ConsoleHelper.Write($"That is:", ConsoleColor.DarkYellow);
            BtcSatFormat.PrintSatToLNBtc(decoded.OfferAmountMsat);
            amount_to_send_millisatoshi = decoded.OfferAmountMsat;
            return amount_to_send_millisatoshi;
        }

        return 0;
    }

    private static string? GetPayerComment()
    {
        ConsoleHelper.WriteLine("(optional, enter for skip) payer_note:", ConsoleColor.DarkYellow);
        var payer_note = Console.ReadLine();
        return payer_note;
    }

    private static (CmdDecodeResponse bolt12_invoice_decoded, string bolt12invoice) FetchBolt12Invoice(string offer, ulong amount_to_send_millisatoshi, string? payer_note)
    {
        var json_res = RunCli.ExecuteLightnigCli($"fetchinvoice -k offer={offer} amount_msat={amount_to_send_millisatoshi} payer_note=\"{payer_note}\"");

        Console.WriteLine(json_res);

        var offer_response = JsonSerializer.Deserialize<CLNbolt12OfferResponse>(json_res, SourceGenerationContextPayto.Default.CLNbolt12OfferResponse);
        json_res = RunCli.ExecuteLightnigCli($"decode {offer_response.Invoice}");

        var bolt12_invoice_decoded = JsonSerializer.Deserialize<CmdDecodeResponse>(json_res, SourceGenerationContextPayto.Default.CmdDecodeResponse);

        if (bolt12_invoice_decoded is null)
            throw new Exception("bolt12 invoice is null");

        if (!bolt12_invoice_decoded.Valid)
            throw new Exception("bolt12 invoice is not valid");

        return (bolt12_invoice_decoded, offer_response.Invoice);
    }

    
    private static void PrintBolt12InvoiceInfo(CmdDecodeResponse bolt12_invoice_decoded)
    {
        Console.WriteLine($"\tinvoice_node_id: {bolt12_invoice_decoded.InvoiceNodeId}");
        Console.WriteLine($"\tamount sat: {(bolt12_invoice_decoded.AmountMsat / 1000).AmountWithSeparators()}");
        Console.WriteLine($"\tinvoice_amount_sat: {(bolt12_invoice_decoded.InvoiceAmountMsat / 1000).AmountWithSeparators()}");
        Console.WriteLine($"\tinvreq_amount_sat: {(bolt12_invoice_decoded.InvreqAmountMsat / 1000).AmountWithSeparators()}");
    }

    private static CmdDecodeResponse ClnDecode(string input, string expectedType)
    {
        var json_res = RunCli.ExecuteLightnigCli($"decode {input}");
        var decoded = JsonSerializer.Deserialize(json_res, SourceGenerationContextPayto.Default.CmdDecodeResponse);
        
        // prins offer in json format to console
        //Console.WriteLine(json_res);
        
        if (decoded is null)
            throw new Exception("decoded is null");

        if (!decoded.Valid)
            throw new Exception("CLN: decoded with Valid == false");

        if (decoded.Type != expectedType)
            throw new Exception($"CLN: not decoded as {expectedType}, but instead {decoded.Type}");

        /// code != 0 is error
        if (decoded.Code != 0)
            throw new Exception($"CLN decoded with error: {decoded.Code} {decoded.Message}");

        return decoded;
    }
}
