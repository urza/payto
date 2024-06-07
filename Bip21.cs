using System.Collections.Immutable;
using System.Web;

namespace payto;
internal class Bip21
{
    // possible kyes in bip21 that can hold bolt12 offer
    private static ImmutableArray<string> bip21_ln_keys = ["lightning", "lno", "lno1", "b12"];
        
    /// <summary>
    /// Try to extract bolt12 offer from bit21 string
    /// </summary>
    public static (bool success, string? offer) TryGetOffer(string bip21)
    {
        // Split the input string into the base part and the query string
        var parts = bip21.Split('?');

        if (parts.Length == 2)
        {
            // Parse the query string
            var queryParams = HttpUtility.ParseQueryString(parts[1]);

            // Find the first value that starts with "lno1" (case-insensitive) for any of the specified keys
            string? lightningValue = bip21_ln_keys
                        .Select(key => queryParams[key])
                        .FirstOrDefault(value => value?.StartsWith("lno1", StringComparison.InvariantCultureIgnoreCase) ?? false);

            //if the bip21 string contains multiple values with the same key this will be broken here, in that case ParseQueryString joins them with comma

            if (lightningValue != null)
                return (true, lightningValue);
        }

        return (false, null);
    }

}
