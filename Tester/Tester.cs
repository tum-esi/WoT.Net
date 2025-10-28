using System;
using WoT.Core.Implementation;
using WoT.Binding.CoAP;
using WoT.Binding.Http;
using WoT.Core.Definitions.TD;
using Newtonsoft.Json;


var consumer = new Consumer();
ThingDescription td;
string protocol2Test = "http"; // change to "coap" to test CoAP only


if (protocol2Test == "coap")
{
    // CoAP
    var tdUri = args.Length > 0 ? args[0] : "coap://localhost:5683/tester";
    // Basic CoAP client configuration (timeouts in ms)
    var coapConfig = new CoapClientConfig
    {
        Timeout = 5000,
        MaxRetransmit = 4,
        AckTimeout = 2000
    };
    consumer.AddClientFactory(new CoapClientFactory(coapConfig));
    Console.WriteLine($"Requesting TD from: {tdUri}");
    td = await consumer.RequestThingDescription(tdUri);
}
else
{
    // HTTP
    var httpTdUri = args.Length > 0 ? args[0] : "http://localhost:8085/tester";
    consumer.AddClientFactory(new HttpClientFactory(new HttpClientConfig()));
    consumer.Start();

    Console.WriteLine($"Requesting TD from: {httpTdUri}");
    td = await consumer.RequestThingDescription(httpTdUri);
}
// Consume the Thing
ConsumedThing consumedThing = (ConsumedThing)consumer.Consume(td);

// Read a boolean
bool boolean = await (await consumedThing.ReadProperty<bool>("bool")).Value();
Console.WriteLine("Read a boolean: " + boolean);
// Read an integer
int integer = await (await consumedThing.ReadProperty<int>("int")).Value();
Console.WriteLine("Read an integer: " + integer);
// Read a number
float number = await (await consumedThing.ReadProperty<float>("num")).Value();
Console.WriteLine("Read a number: " + number);
// Read a string
string str = await (await consumedThing.ReadProperty<string>("string")).Value();
Console.WriteLine("Read a string: " + str);
// Read an array
object[] array = await (await consumedThing.ReadProperty<object[]>("array")).Value();
Console.WriteLine("Read an array: " + JsonConvert.SerializeObject(array));
// Read an object
Dictionary<string, object> obj = await (await consumedThing.ReadProperty<Dictionary<string, object>>("object")).Value();
Console.WriteLine("Read an object: " + JsonConvert.SerializeObject(obj));


// Invoke Action with no input and no output
await consumedThing.InvokeAction("void-void");

// Invoke Action with an input and no output
await consumedThing.InvokeAction("int-void", 1);

// Invoke Action with no input but an output
var outputBuffer = await (await consumedThing.InvokeAction<int>("void-int")).ArrayBuffer();
// Buffer to string
string outputJson = System.Text.Encoding.UTF8.GetString(outputBuffer);
// Deserialize JSON
int output = JsonConvert.DeserializeObject<int>(outputJson);
Console.WriteLine("Output of 'void-int' action was: " + output);

// Invoke Action with an input and an output
int output2 = await (await consumedThing.InvokeAction<int, int>("int-int", 4)).Value();
Console.WriteLine("Output of 'void-int' action was: " + output2);


Console.WriteLine("Done.");