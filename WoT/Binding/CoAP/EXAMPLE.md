# CoAP Binding Example

This example demonstrates how to use the CoAP binding with WoT.Net:

```csharp
using WoT.Core.Implementation;
using WoT.Binding.CoAP;
using WoT.Core.Definitions.TD;

Consumer consumer = new();
CoapClientConfig coapConfig = new()
{
    Timeout = 30000,
    MaxRetransmit = 4,
    AckTimeout = 2000
};

consumer.AddClientFactory(new CoapClientFactory(coapConfig));
consumer.Start();

// Request a Thing Description from a CoAP server
ThingDescription td = await consumer.RequestThingDescription("coap://example.com:5683/thing");
ConsumedThing consumedThing = (ConsumedThing)consumer.Consume(td);

// Read a property
int value = await (await consumedThing.ReadProperty<int>("temperature")).Value();
Console.WriteLine($"Temperature: {value}");

// Write a property
await consumedThing.WriteProperty("setpoint", 22);

// Invoke an action
await consumedThing.InvokeAction("toggle");
```

## Implementation Notes

The current CoAP binding provides a basic implementation of the CoAP protocol supporting GET, POST, PUT, and DELETE operations. It includes:

- Basic CoAP message encoding/decoding (RFC 7252)
- URI-Path option handling
- Content-Format option support
- Response code handling

### Not Yet Implemented

- **CoAP Observe**: Event subscriptions via CoAP Observe are not yet implemented
- **Block1**: Blockwise transfer for large request payloads is not yet implemented
- **DTLS Security**: CoAPS (secure CoAP) is not yet supported
- **Advanced options**: Many CoAP options are not yet implemented

### Block2 Support

The implementation now includes **Block2 (RFC 7959)** support for handling large responses:
- Automatically detects when a server uses blockwise transfer
- Requests subsequent blocks until the complete payload is received
- Assembles blocks into a complete response transparently
- Supports block sizes from 16 to 1024 bytes

This enables fetching large Thing Descriptions and other payloads that exceed the basic CoAP message size limit.

### For Production Use

For production deployments requiring full CoAP support, consider integrating a complete CoAP library such as:
- Com.AugustCellars.CoAP (for .NET Framework)
- CoAPnet (may require .NET Core 3.1+)

The current implementation can be replaced by modifying `WotCoapClient.cs` to use the chosen library while maintaining the `IProtocolClient` interface.
