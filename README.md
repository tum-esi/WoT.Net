# WoT.Net

[![NuGet Version](https://img.shields.io/nuget/v/WoT.Net)](https://www.nuget.org/packages/WoT.Net)


A .NET Standard 2.0 implementation of the W3C Web of Things (WoT) [Scripting API](https://www.w3.org/TR/wot-scripting-api/), inspired by [node-wot](https://github.com/eclipse-thingweb/node-wot)

## What is the W3C Web of Things (WoT)?
The [W3C Web of Things](https://www.w3.org/WoT/) is a standardization effort that aims to facilitate the interoperability between the highly fragmented IoT technologies.
Instead of imposing new technologies, WoT aims to provide a standardized model for modeling and describing the capabilities and the network/web interface that any 
IoT entity is providing. In the context of WoT, the capabilities of any entity are modeled as one of three interaction affordances:
* **Property Affordances**: representing data sources or sinks that can be read or written to.
* **Action Affordances**: representing operations that usually take longer or may physically affect the environment.
* **Event Affordances**: representing any notifications or streams and the means to subscribe to them.

Each entity is then capable of describing its own WoT interface using a standardized description format called the [Thing Description (TD)](https://www.w3.org/TR/wot-thing-description11/),
a JSON-LD document that is both highly human- and machine-readable and contains the entity's WoT model and any additional related metadata.

## What is our aim?
Our long-term goal here is to provide the .NET Standard 2.0 stack that fully implements the [Scripting API](https://www.w3.org/TR/wot-scripting-api/), which would facilitate
rapid development of WoT applications for devices running Windows OS, including the Hololens 2, but would also facilitate the integration of the WoT stack in Unity.
Our short-term goal is to implement the functionalities of a WoT Consumer, i.e. the functionalities needed to fetch a TD and consume it to interact with the entity it describes.
We will focus first on HTTP Things but aim to implement functionality for HTTPS, CoAP, CoAPS, and MQTT in the future.

## Getting Started
An example showing how to use this library
```csharp
using Newtonsoft.Json;
using WoT;
using WoT.Definitions;
using WoT.Implementation;

SimpleHTTPConsumer consumer = new SimpleHTTPConsumer();
ThingDescription td = await consumer.RequestThingDescription("http://plugfest.thingweb.io:8083/testthing");
SimpleConsumedThing consumedThing = await consumer.Consume(td) as SimpleConsumedThing;

// Read a boolean
bool boolean                    = await (await consumedThing.ReadProperty<bool>("bool")).Value();
Console.WriteLine("Read a boolean: " + boolean);
// Read an integer
int integer                     = await (await consumedThing.ReadProperty<int>("int")).Value();
Console.WriteLine("Read an integer: " + integer);
// Read a number
float number                    = await (await consumedThing.ReadProperty<float>("num")).Value();
Console.WriteLine("Read a number: " +number);
// Read a string
string str                      = await (await consumedThing.ReadProperty<string>("string")).Value();
Console.WriteLine("Read a string: " + str);
// Read an array
object[] array                  = await (await consumedThing.ReadProperty<object[]>("array")).Value();
Console.WriteLine("Read a string: " + JsonConvert.SerializeObject(array));
// Read an object
Dictionary<string, object> obj  = await (await consumedThing.ReadProperty<Dictionary<string, object>>("object")).Value();
Console.WriteLine("Read a string: " + JsonConvert.SerializeObject(obj));


// Invoke Action with no input and no output
await consumedThing.InvokeAction("void-void");

// Invoke Action with an input and no output
await consumedThing.InvokeAction("int-void", 1);

// Invoke Action with no input but an output
int output =  await (await consumedThing.InvokeAction<int>("void-int")).Value();
Console.WriteLine("Output of 'void-int' action was: " + output);

// Invoke Action with an input and an output
int output2 = await (await consumedThing.InvokeAction<int, int>("int-int", 4)).Value();
Console.WriteLine("Output of 'void-int' action was: " + output2);

if (consumedThing != null)
{

    // Subscribe to Event
    var sub = await consumedThing.SubscribeEvent<int>("on-int", async (output) =>
    {
        Console.WriteLine("Event:");
        Console.WriteLine(await output.Value());
        Console.WriteLine("---------------------");
    });

    var task = Task.Run(async () =>
    {
        Console.WriteLine("Here");
        Console.WriteLine(consumedThing.HasActiveListeners);
        while (consumedThing.HasActiveListeners) {
            // Write an int
            await consumedThing.WriteProperty<int>("int", 5);
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        return;
    });

    Task stopTask = Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith(async (task) => { await sub.Stop(); });
    await task;
}
Console.WriteLine("Done");
```
For a more detailed step-by-step explanation and documentation, feel free to visit our [documentation page](https://tum-esi.github.io/WoT.Net/).

## Roadmap
- [X] TD Deserializing and Parsing 
- [X] HTTP Consumer (works only with `"application\json"`, `"longpoll"` and no security)
- [ ] HTTPS Consumer
- [ ] CoAP Consumer
- [ ] CoAPS Consumer
- [ ] MQTT Consumer

... more in the future

<!-- first commit -->
