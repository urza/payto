using payto;


if (args.Length == 1)
{
    await PayTo.PayToDestinationAsync(args[0]);

    Console.WriteLine("payto finished");
    return;
}

Console.WriteLine("Usage: payto destination");
Console.WriteLine("Destination can be bolt11, bolt12, LNURL, lightning address");
return;

