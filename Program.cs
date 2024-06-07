using payto;


if (args.Length == 1)
{
    await PayTo.PayToDestinationAsync(args[0]);

    Console.WriteLine("payto finished");
    return;
}

Console.WriteLine("Usage: payto destination");
Console.WriteLine("Destination can be Lightning Address, LNURL, BIP353 DNS Payment Instruction or BOLT12 Offer");
return;

