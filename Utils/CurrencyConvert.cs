using ExtensionMethods;
using payto.JsonTypes;
using System.Text.Json;

namespace payto.Utils;
internal class CurrencyConvert
{
   

    /// <summary>
    /// Check if CLN has currencyrate plugin
    /// https://github.com/lightningd/plugins/tree/master/currencyrate
    /// </summary>
    public static bool HasCLNCurrencyPlugin()
    {
        var json_res = RunCli.ExecuteLightnigCli("plugin list");

        var result = JsonSerializer.Deserialize<ClnPluginList>(json_res, SourceGenerationContextPayto.Default.ClnPluginList);

        //currencyrate

        return (result?.plugins?.Any(x => x.name.Contains("currencyrate", StringComparison.InvariantCultureIgnoreCase)
                                       && x.active)
                                    ) == true;
    }

    /// <summary>
    /// Use CLN currencyrate plugin to convert USD/EUR/CZK etc to MSAT
    /// </summary>
    public static ulong CLNConvertToMsat(ulong amount, string currency)
    {
        var json_res = RunCli.ExecuteLightnigCli($"currencyconvert {amount} {currency}");

        var result = JsonSerializer.Deserialize<ClnCurrencyconvertResult>(json_res, SourceGenerationContextPayto.Default.ClnCurrencyconvertResult);

        //currencyrate
        if (result is null)
            throw new Exception($"No result from converting {amount} {currency} to msat using currencyconvert");

        var msat = result.msat.Replace("msat", "", StringComparison.InvariantCultureIgnoreCase);

        var parse = msat.TryParseNumber<ulong>();

        if(!parse.success)
            throw new Exception($"ConvertToMsat: Can't parse {msat} to number");

        Console.WriteLine($"Converted {amount} {currency} to {(parse.result / 1000).AmountWithSeparators()} sat");

        return parse.result;
    }




}
