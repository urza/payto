namespace payto;

public enum DestinationType
{
    /// <summary>
    /// general case of user@domain.tld
    /// May be LNURL address (http based) or BIP353 address (DNS based)
    /// </summary>
    ADDRESS,

    /// <summary>
    /// libhtning address based on BIP 353: DNS Payment Instructions - https://github.com/bitcoin/bips/pull/1551
    /// </summary>
    ADDRESS_DNS_BIP353,

    /// <summary>
    /// lightning address based on Http and LNURLP https://github.com/andrerfneves/lightning-address/blob/master/README.md
    /// </summary>
    ADDRESS_LNURL,

    /// <summary>
    /// LNURL PayRequest encoded as bech32 lnurlxxx (LUD01) or lnurlp://xxx (LUD17)
    /// </summary>
    LNURL_PAYREQUEST,

    /// <summary>
    /// lno1... offer to fetch invoice
    /// </summary>
    BOLT12_OFFER,
       
    //BOLT12_INVOICE,

    //BOLT11_INVOICE,
}


internal class PayTo
{
    public static DestinationType WhatIsIt(string destination)
    {
        if (destination.Contains("@"))
        {
            /// can be DNS_BIP353 or LNURL_LIGHTNING_ADDRESS

            /// BIP353 says to start with ₿ symbol
            if (destination.StartsWith("₿"))
                return DestinationType.ADDRESS_DNS_BIP353;

            /// realistically let's not expect the ₿ symbol, but still first try check the DNS instead of HTTP for better privacy
            if (PayToDns.TryQueryDnsInfo(destination))
                return DestinationType.ADDRESS_DNS_BIP353;

            /// all other cases assume LNURL address
            return DestinationType.ADDRESS_LNURL;
        }

        if (destination.StartsWith("lno1", StringComparison.InvariantCultureIgnoreCase))
        {
            /// BOLT12_OFFER
            return DestinationType.BOLT12_OFFER;
        }

        if (destination.StartsWith("lnurl1", StringComparison.InvariantCultureIgnoreCase)
            || destination.StartsWith("lnurlp", StringComparison.InvariantCultureIgnoreCase))
        {
            /// LNURL
            return DestinationType.LNURL_PAYREQUEST;
        }

        throw new ArgumentException("The destination is not recognized as valid target. Must be lightning@address.domain or bolt12 offer or LNURLP ");
    }

    internal static async Task PayToDestinationAsync(string destination)
    {
        // trim and remove "lightning:"
        destination = destination.Trim()
                    .Replace("lightning://", "", StringComparison.InvariantCultureIgnoreCase)
                    .Replace("lightning:", "", StringComparison.InvariantCultureIgnoreCase);

        // determine type of destination
        var destinationType = PayTo.WhatIsIt(destination);

        if (destinationType == DestinationType.BOLT12_OFFER)
            PayToBolt12.PayToOffer(destination);

        if (destinationType == DestinationType.ADDRESS_DNS_BIP353)
            PayToDns.PayToAddress(destination);

        if (destinationType == DestinationType.ADDRESS_LNURL)
            await PayToLNURL.PayToAddressAsync(destination);

        if (destinationType == DestinationType.LNURL_PAYREQUEST)
            await PayToLNURL.PayToLNURLAsync(destination);
    }
}
