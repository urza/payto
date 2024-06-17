using payto;


if (args.Length == 1)
{
    await PayTo.PayToDestinationAsync(args[0]);

    Console.WriteLine("payto finished");
    return;
}

Console.WriteLine("Usage: payto destination");
Console.WriteLine("Destination can be Lightning Address (HTTP/LNURLP based or BIP353 DNS based), LNURLP or BOLT12 Offer");
return;

