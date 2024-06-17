using ExtensionMethods;

namespace payto;

/// <summary>
/// BIP 353: DNS Payment Instructions https://github.com/bitcoin/bips/pull/1551
/// Tries to find bolt12 offer in DNS record for user@domail.tld and pay that offer.
/// Other payment options are ignored, only bolt12 offer is used.
/// It uses Linux commands to get the dns and validate dnssec - I dont want to bring dependency on other libraries here and standard library doesn't have that.
/// </summary>
internal class PayToDns
{
    // temporary store lookup result to avoid second query, result should be bip21
    public static Dictionary<string, string> tmpLookupResult = new();

    /// <summary>
    /// 1. 
    /// </summary>
    /// <param name="address">email-like looking address (with or without ₿ symbol at the beginning)</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static bool TryQueryDnsInfo(string address)
    {
        Console.WriteLine($"Trying to find BIP353 - TXT DNS record for {address}");

        if (address.StartsWith("₿"))
            address = address.Substring(1);

        var split = address.Trim().Split('@');

        if (split.Length != 2)
            throw new Exception("Wrong address format, must be user@domain");

        var user = split[0];
        var domain = split[1];

        /// first use linux dig command to see if there is any dns record
        var dig_cmd = "dig";
        var dig_args = $"-t txt {user}.user._bitcoin-payment.{domain} +short +dnssec";
        string dig_result = "";
        try
        {
            dig_result = RunCli.ExecuteCommand(dig_cmd, dig_args);
        }
        catch (Exception ex)
        {
            // don't throw exception here, in case the address is actually LNURL
            Console.Write($"Executing dig failed {ex.Message}");
            return false;
        }

        if (dig_result.IsEmpty())
        {
            Console.WriteLine($"No TXT DNS record for {user}.user._bitcoin-payment.{domain} found");
            return false;
        }

        dig_result = dig_result.Trim()
                        .Replace("\" \"", "") //longer entries than 255 charactes sometimes come as multiple stings separated by space
                        .Replace("\"", "") //remove " 
                        .Replace(Environment.NewLine, ""); //remove line endings


        Console.WriteLine($"Found this: {dig_result}");
        Console.WriteLine("Validating DNSSEC..");

        /// now validate dnssec - not possible with std lib, and don't want to bring other dependecies
        /// so let Linux do it. Veryfiyng locally might not work because "the trust chain is broken", so use Cloudflare by default

        var delv_cmd = "delv";
        var delv_args = $"-t txt {user}.user._bitcoin-payment.{domain} @1.1.1.1";
        string delv_result = "";
        
        try
        {
            delv_result = RunCli.ExecuteCommand(delv_cmd, delv_args);
        }
        catch(Exception ex)
        {
            Console.WriteLine("Executing delv command (to verify DNSSEC) failed");
            throw; // end the program here because it was BIP353
        }

        if (delv_result.IsEmpty())
            throw new Exception("Failed validating DNSSEC");

        var firstline = delv_result.GetLines().First();
        if (!firstline.Contains("; fully validated", StringComparison.InvariantCultureIgnoreCase))
            throw new Exception($"Failed validating DNSSEC - {firstline}");

        Console.WriteLine("All good");
        
        tmpLookupResult.Add(address, dig_result);
        return true;
    }

    /// <summary>
    /// Expects that tmpLookupResult contains the address and bip21 value and that the DNSSEc was validated.
    /// </summary>
    /// <param name="address">user@domain with or without </param>
    /// <exception cref="Exception"></exception>
    public static void PayToAddress(string address)
    {
        if (address.StartsWith("₿"))
            address = address.Substring(1);

        if (!tmpLookupResult.ContainsKey(address))
            throw new Exception("Here should be something rather than nothing. Trying pay to dns address that wasn't found or validated?");

        var bip21 = tmpLookupResult[address];

        var res = Bip21.TryGetOffer(bip21);

        if (!res.success)
            throw new Exception("BIP21 parsing error");

        var bolt12offer = res.offer;

        if (bolt12offer.IsEmpty())
            throw new Exception("Offer from bip21 is empty");

        PayToBolt12.PayToOffer(bolt12offer);
    }
}
