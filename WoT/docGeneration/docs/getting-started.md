# Getting Started

To interact with a Consumed Thing, perform the following step:

* Install package using NuGet and bring namespaces into scope
  
    ```csharp
    using WoT.Core.Implementation;
    using WoT.Binding.Http;
    using WoT.Core.Definitions.TD;
    using Newtonsoft.Json;
    ```

* Instantiate a new ``SimpleHTTPConsumer``

    ```csharp
    Consumer consumer = new();
    ```

* Add ClientFactories for different protocol bindings

  ```csharp
  HttpClientConfig clientConfig = new()
  {
      AllowSelfSigned = true
  };
  consumer.AddClientFactory(new HttpClientFactory(new HttpClientConfig()));
  consumer.AddClientFactory(new HttpsClientFactory(clientConfig));
  ```

* The ``consumer`` can either request the TD from an HTTP URL or can consume a TD string stored in a file or that is hard coded. To request at TD from an HTTP URL, do

    ```csharp
    ThingDescription td = await consumer.RequestThingDescription("http://plugfest.thingweb.io:80/http-data-schema-thing/");
    ```

* To convert a TD string to a ``ThingDescription`` object, do
  
    ```csharp
    string tdString = "{...}";
    ThingDescription td = JSONConvert.Deserialize<ThingDescription>(tdString);
    ```

* The ``consumer`` can now consume the TD object to provide interaction capabilities

    ```csharp
    ConsumedThing consumedThing = (ConsumedThing)consumer.Consume(td);
    ```

* Reading a property works as follows
  
    ```csharp
    // Read a boolean
    bool boolean = await (await consumedThing.ReadProperty<bool>("bool")).Value();
    Console.WriteLine("Read a boolean: " + boolean);
    // Read an integer
    int integer = await (await consumedThing.ReadProperty<int>("int")).Value();
    Console.WriteLine("Read an integer: " + integer);
    // Read a number
    float number = await (await consumedThing.ReadProperty<float>("num")).Value();
    Console.WriteLine("Read a number: " +number);
    // Read a string
    string str = await (await consumedThing.ReadProperty<string>("string")).Value();
    Console.WriteLine("Read a string: " + str);
    // Read an array
    object[] array = await (await consumedThing.ReadProperty<object[]>("array")).Value();
    Console.WriteLine("Read a string: " + JsonConvert.SerializeObject(array));
    // Read an object
    Dictionary<string, object> obj = await (await consumedThing.ReadProperty<Dictionary<string, object>>("object")).Value();
    Console.WriteLine("Read a string: " + JsonConvert.SerializeObject(obj));
    ```

    > [!NOTE]
    > The JSON Schema types (``"null"``, ``"boolean"``, ``"integer"``, ``"number"``, ``"string"``, ``"array"``, ``"object"``) are mapped in C# according to the [TD Specification](https://www.w3.org/TR/wot-thing-description11) as follows:
    > * ``"null"`` &rarr; ``null``
    > * ``"boolean"`` &rarr; ``bool``
    > * ``"integer"`` &rarr; ``int``
    > * ``"number"`` &rarr; ``double``
    > * ``"string"`` &rarr; ``string``
    > * ``"array"`` &rarr; ``T[]`` or ``List<T>``
    > * ``"object"`` &rarr; ``Dictionary<string,T>``

* Writing a property looks as follows:

    ```csharp
    await consumedThing.WriteProperty<int>("int", 5);
    ```

* Invoking actions:

    ```csharp
    // Invoke Action with no input and no output
    await consumedThing.InvokeAction("void-void");

    // Invoke Action with an input and no output
    await consumedThing.InvokeAction("int-void", 1);

    // Invoke Action with no input but an output

    // Use array buffer because output schema is not defined
    var outputBuffer = await (await consumedThing.InvokeAction<int>("void-int")).ArrayBuffer();
    // Buffer to string
    string outputJson = System.Text.Encoding.UTF8.GetString(outputBuffer);
    // Deserialize JSON
    int output = JsonConvert.DeserializeObject<int>(outputJson);
    Console.WriteLine("Output of 'void-int' action was: " + output);

    // Invoke Action with an input and an output
    int output2 = await (await consumedThing.InvokeAction<int, int>("int-int", 4)).Value();
    Console.WriteLine("Output of 'void-int' action was: " + output2);
    ```

* Subscribing to events:
  
    ```csharp
    var sub = await consumedThing.SubscribeEvent<int>("on-int", async (output) =>
    {
        Console.WriteLine("Event:");
        Console.WriteLine(await output.Value());
        Console.WriteLine("---------------------");
    });
    // Stop subscription
    sub.stop()
    ```

Here is the full example:

```csharp
using WoT.Core.Implementation;
using WoT.Binding.Http;
using WoT.Core.Definitions.TD;
using Newtonsoft.Json;


Consumer consumer = new();
HttpClientConfig clientConfig = new()
{
    AllowSelfSigned = true
};
consumer.AddClientFactory(new HttpClientFactory(new HttpClientConfig()));
consumer.AddClientFactory(new HttpsClientFactory(clientConfig));

consumer.Start();

ThingDescription td = await consumer.RequestThingDescription("http://plugfest.thingweb.io:80/http-data-schema-thing/");
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

if (consumedThing != null)
{

    // Subscribe to Event
    var sub = await consumedThing.SubscribeEvent<int>("on-int", async (output) =>
    {
        Console.WriteLine("Event: Received on-int event");
        Console.WriteLine($"Value received: {await output.Value()}");
        Console.WriteLine("---------------------");
    });

    var task = Task.Run(async () =>
    {
        while (sub.Active)
        {
            // Get random integer between 0 and 100
            Random random = new();
            int randomInt = random.Next(0, 100);
            Console.WriteLine($"Writing int {randomInt}");
            Console.WriteLine("---------------------");
            // Write an int
            await consumedThing.WriteProperty("int", randomInt);
            
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        return;
    });

    Task stopTask = Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith(async (task) => { await sub.Stop(); });
    await task;
}
Console.WriteLine("Done");
```
