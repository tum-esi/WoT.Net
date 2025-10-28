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

// Invoke an action with Input and Output
var result = await thing.InvokeAction<double, double>("incrementFor", 1);
double value = await result.Value();

```

## Implementation Notes

The current CoAP binding provides a basic implementation of the CoAP protocol supporting GET, POST, PUT, and DELETE operations. It includes:

- Basic CoAP message encoding/decoding (RFC 7252)
- URI-Path option handling
- Content-Format option support
- Response code handling

### Not Yet Implemented

- **CoAP Observe**: Event subscriptions via CoAP Observe are not yet implemented
- **DTLS Security**: CoAPS (secure CoAP) is not yet supported
- **Advanced options**: Many CoAP options are not yet implemented

### For Production Use

For production deployments requiring full CoAP support, consider integrating a complete CoAP library such as:
- Com.AugustCellars.CoAP (for .NET Framework)
- CoAPnet (may require .NET Core 3.1+)

The current implementation can be replaced by modifying `WotCoapClient.cs` to use the chosen library while maintaining the `IProtocolClient` interface.
