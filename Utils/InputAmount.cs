using ExtensionMethods;

namespace payto.Utils;
internal class InputAmount
{
    public static ulong GetAmountFromUser(ulong? min_sendable_msat, ulong? max_sendable_msat)
    {
        Console.WriteLine();
        ConsoleHelper.WriteLine("How much do you want to send?", ConsoleColor.DarkYellow);
        Console.WriteLine("Space and _ allowed for visual separation. Number without suffix will be treated as sats or specify fiat currency (currencyrate plugin must be active).");

        var userInput = Console.ReadLine();

        var amounttosend_msat = ParseAmountToMsat(userInput);

        if (min_sendable_msat.HasValue && amounttosend_msat < min_sendable_msat)
            throw new ArgumentOutOfRangeException("Amount is not between min sendable and max sendable");

        if (max_sendable_msat.HasValue && amounttosend_msat > max_sendable_msat)
            throw new ArgumentOutOfRangeException("Amount is not between min sendable and max sendable");

        return amounttosend_msat; // return in millisathosi
    }

    /// <summary>
    /// Parse input from user to msat for sending
    /// </summary>
    /// <param name="inputted_amount">User input in sats or number with currency suffix. 1_000_000 will be 1 million sats, 1000 EUR will be converted to msats using currencyconvert plugin.</param>
    /// <returns>Amount in millisats</returns>
    public static ulong ParseAmountToMsat(string inputted_amount)
    {
        if (inputted_amount.IsEmpty())
            throw new ArgumentException("amount to send is empty");

        var input = inputted_amount.Trim();

        if (input.EndsWith("msat", StringComparison.InvariantCultureIgnoreCase))
            throw new ArgumentException("Msat not supported as amount input.");


        input = input.Replace(" ", "") //remove visual separators
                     .Replace("_", "")
                     .Replace("sat", "", StringComparison.InvariantCultureIgnoreCase) //remove sat or sats
                     .Replace("sats", "", StringComparison.InvariantCultureIgnoreCase) //remove sat or sats
                     ;

        var parsed = input.TryParseNumber<ulong>();

        /// if it is just a number
        if (parsed.success)
            return parsed.result * 1000; // was sats, return msat

        /// not just a number - assuming last 3 chars is currency suffix

        /// do we have currency converter?
        if (!CurrencyConvert.HasCLNCurrencyPlugin())
            throw new Exception("Inputted amount in fiat currency requires CLN currencyconvert plugin");

        /// asume currencies are always 3 chars at the end
        int currencyStartIndex = input.Length - 3;

        // Extract the amount substring
        string amountString = input.Substring(0, currencyStartIndex);

        // Extract the currency substring
        string currencyString = input.Substring(currencyStartIndex);

        parsed = amountString.TryParseNumber<ulong>();

        if (!parsed.success)
            throw new ArgumentException($"ParseAmountToMsat - wrong number format {amountString}");


        return CurrencyConvert.CLNConvertToMsat(parsed.result, currencyString);
    }
}
