namespace payto;
public static class BtcSatFormat
{
    /// <summary>
    /// Converts value in sats to string that looks like this
    ///         1 sat => 0.00_000_001 
    ///    1 mio sats => 0.01_000_000 
    ///    500K sats  => 0.00_500_000
    ///        1 btc  => 1.00_000_000
    /// 24355.551 btc => 24355.55_100_000
    /// </summary>
    public static string SatToLNBtc(ulong sats)
    {
        // Convert the input to a decimal to preserve the fractional part after division.
        decimal value = sats / 100_000_000M;

        // Split the number into integer and fractional parts.
        string[] parts = value.ToString("0.00_000_000", System.Globalization.CultureInfo.InvariantCulture).Split('.');

        // Ensure the integer part does not contain any commas or underscores.
        string integerPart = parts[0].Replace(",", "");

        // Combine the integer part with the fractional part using a decimal point, ensuring the fractional part maintains its formatting.
        string formattedString = integerPart + "." + parts[1];

        return formattedString;
    }

    /// <summary>
    /// Print to console with colors
    /// </summary>
    public static void PrintSatToLNBtc(ulong sats)
    {
        var str = SatToLNBtc(sats);

        Console.ForegroundColor = ConsoleColor.DarkGray;

        foreach (var ch in str)
        {
            // keep console dark gray until we encounter first digit that is not zero
            if (ch != '0' && ch != '_' && ch != '.')
                Console.ResetColor();

            Console.Write(ch);
        }

        Console.ResetColor();
        Console.Write(Environment.NewLine);
    }
}
