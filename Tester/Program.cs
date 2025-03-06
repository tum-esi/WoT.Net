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