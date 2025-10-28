# WoT.Net.Binding.CoAP

CoAP (Constrained Application Protocol) binding for WoT.Net.

This package provides CoAP protocol support for the WoT.Net library, enabling consumption of Things that use the CoAP protocol.

## Features

- CoAP client implementation for consuming CoAP Things
- Support for GET, POST, PUT, DELETE operations
- Basic CoAP message encoding/decoding (RFC 7252)
- Compatible with .NET Standard 2.0

**Not yet implemented:**
- CoAP Observe for event subscriptions
- DTLS/CoAPS security

## Usage

```csharp
using WoT.Core.Implementation;
using WoT.Binding.CoAP;

Consumer consumer = new();
CoapClientConfig clientConfig = new();
consumer.AddClientFactory(new CoapClientFactory(clientConfig));

consumer.Start();

ThingDescription td = await consumer.RequestThingDescription("coap://example.com/thing");
ConsumedThing thing = (ConsumedThing)consumer.Consume(td);
```
