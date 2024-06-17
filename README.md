
Core Lightning (CLN) companion app that can pay to [lightning address](https://lightningaddress.com/), LNURL, [BIP353 (DNS Payment Instructions)](https://github.com/bitcoin/bips/blob/master/bip-0353.mediawiki) and bolt12 offers.

*I wanted to be able to pay to lightning address directly from terminal using CLN (Core Lightnig), so I created this.*

payto understands:<br/>
[✔] [lightning address based on HTTP/LNURLP](https://lightningaddress.com/)<br/>
[✔] [lightning address based on bolt12 offer in BIP353 TXT DNS](https://github.com/bitcoin/bips/blob/master/bip-0353.mediawiki)<br/>
[✔] [LNURL](https://github.com/lnurl/luds) (LUD [06](https://github.com/lnurl/luds/blob/luds/06.md), [12](https://github.com/lnurl/luds/blob/luds/12.md) and [17](https://github.com/lnurl/luds/blob/luds/17.md))<br/>
[✔] [bolt12 offers](https://bolt12.org/)<br/>

It is simple interactive command line application, that has no external dependencies. Only the standard dotnet library and compiles into single native binary.

It takes one argument - destination - and retrieves information how much can be sent. Then it asks for input - amount & comment and for final confirmation before sending the payment.

Fiat currencies can be used for amount if [CLN currencyrate plugin is active](https://github.com/lightningd/plugins/tree/master/currencyrate).

All the heavy lifting is done by CLN (decoding, fetching & paying invoices) and Linux (DNSSEC validation in case of BIP353).

❗❗ Note on BIP353 state: treat this tool as a prototype when it comes to BIP353 - it works, can retrieve and parse bolt 12 offer from TXT DNS record, do basic DNS SEC validation by delv command and pay the offer. But this is not (yet) fully compliant with the BIP specification.

![image](https://github.com/urza/payto/assets/189804/efb5b840-3c9d-4471-87a8-4bf24ca5c94c)
