namespace payto;
internal class ConsoleHelper
{
    public static void WriteLine(string message, ConsoleColor foregroundColor)
    {
        var console_orig_color = Console.ForegroundColor;

        Console.ForegroundColor = foregroundColor;

        Console.WriteLine(message);

        Console.ForegroundColor = console_orig_color;
    }

    public static void Write(string message, ConsoleColor foregroundColor)
    {
        var console_orig_color = Console.ForegroundColor;

        Console.ForegroundColor = foregroundColor;

        Console.Write(message);

        Console.ForegroundColor = console_orig_color;
    }
}
