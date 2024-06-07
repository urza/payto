using System.Net;
using System.Text;

namespace LNURL;

/// <summary>
/// LNURLPay parsing
/// Mostly stolen from https://github.com/Kukks/LNURL and simplified to only what is needed here
/// </summary>
public class LNURLPay
{
    /// <summary>
    /// Parse LNURLPay into Uri of the LN SERVICE
    /// lnurl 
    /// https://github.com/lnurl/luds/blob/luds/06.md
    /// </summary>
    /// <param name="lnurl">must be bech32 and 'lnurl' as the hrp (LUD1) or an lnurl LUD17 scheme.</param>
    /// <returns>tuple (bool success, Uri? LnService) - success is parsing result, LnService is url where to fetch LN invoice</returns>
    public static (bool success, Uri? LnService) TryParse(string lnurl)
    {
        lnurl = lnurl.Replace("lightning://", "", StringComparison.InvariantCultureIgnoreCase);
        lnurl = lnurl.Replace("lightning:", "", StringComparison.InvariantCultureIgnoreCase);

        /// LUD-06: payRequest - https://github.com/lnurl/luds/blob/luds/06.md
        if (lnurl.StartsWith("lnurl1", StringComparison.InvariantCultureIgnoreCase))
        {
            Bech32Engine.Decode(lnurl, out _, out var data);
            var result = new Uri(Encoding.UTF8.GetString(data));

            if (!result.IsOnion() && !result.Scheme.Equals("https"))
                throw new FormatException("LNURL provided is not secure.");
            
            return (success:true, LnService: result);
        }

        /// LUD17 lnurlp://domain.com/payme - https://github.com/lnurl/luds/blob/luds/17.md
        if (lnurl.StartsWith("lnurlp", StringComparison.InvariantCultureIgnoreCase))
        {
            if (Uri.TryCreate(lnurl, UriKind.Absolute, out var lud17Uri))
                return (success: true, LnService: new Uri(lud17Uri.ToString()
                    .Replace(lud17Uri.Scheme + ":", lud17Uri.IsOnion() ? "http:" : "https:")));
        }

        return (success: false, LnService: null);
    }

    /// <summary>
    /// Gets LN SERVICE url from lighting address
    /// urza_cc@fountain.fm => https://fountain.fm/.well-known/lnurlp/urza_cc
    /// </summary>
    public static Uri ExtractUriFromLightningAddress(string lightningAddress)
    {
        var s = lightningAddress.Split("@");
        var s2 = s[1].Split(":");
        UriBuilder uriBuilder;
        if (s2.Length > 1)
            uriBuilder = new UriBuilder(
                s[1].EndsWith(".onion", StringComparison.InvariantCultureIgnoreCase) ? "http" : "https",
                s2[0], int.Parse(s2[1]))
            {
                Path = $"/.well-known/lnurlp/{s[0]}"
            };
        else
            uriBuilder =
                new UriBuilder(s[1].EndsWith(".onion", StringComparison.InvariantCultureIgnoreCase) ? "http" : "https",
                    s2[0])
                {
                    Path = $"/.well-known/lnurlp/{s[0]}"
                };

        return uriBuilder.Uri;
    }

    public static void AppendPayloadToQuery(UriBuilder uri, string key, string value)
    {
        if (uri.Query.Length > 1)
            uri.Query += "&";

        uri.Query = uri.Query + WebUtility.UrlEncode(key) + "=" +
                    WebUtility.UrlEncode(value);
    }
}

public static class UriExtensions
{
    public static bool IsOnion(this Uri uri)
    {
        if (uri == null || !uri.IsAbsoluteUri)
            return false;
        return uri.DnsSafeHost.EndsWith(".onion", StringComparison.InvariantCultureIgnoreCase);
    }
}

