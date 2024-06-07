
Core Lightning (CLN) companion app that can pay to lightning address, LNURL, BIP353 (DNS Payment Instructions) and bolt12 offers.

*I wanted to be able to pay to lightning address directly from terminal using CLN (Core Lightnig), so I created this.*

payto understands:<br/>
[✔] [lightning address](https://lightningaddress.com/)<br/>
[✔] [LNURL](https://github.com/lnurl/luds) (LUD [06](https://github.com/lnurl/luds/blob/luds/06.md), [12](https://github.com/lnurl/luds/blob/luds/12.md) and [17](https://github.com/lnurl/luds/blob/luds/17.md))<br/>
[✔] [BIP353 DNS Payment Instructions](https://github.com/bitcoin/bips/pull/1551)<br/>
[✔] [bolt12 offers](https://bolt12.org/)<br/>


It is simple interactive command line application, that has no external dependencies. Only the standard library and compiles into single native binary.

It takes one argument - destination - and retrieves information how much can be sent. Then it asks for input - amount & comment and for final confirmation before sending the payment.

All the heavy lifting is done by CLN (decoding, fetching & paying invoices) and Linux (DNSSEC validation in case of BIP353).

![image](https://github.com/urza/payto/assets/189804/3049059a-ef58-4202-b105-4b2b7a54192d)





