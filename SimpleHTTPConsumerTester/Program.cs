// See https://aka.ms/new-console-template for more information
using WoT;
using WoT.Definitions;
using WoT.Implementation;

SimpleHTTPConsumer consumer = new SimpleHTTPConsumer();
ThingDescription td = await consumer.RequestThingDescription("http://plugfest.thingweb.io:8083/smart-coffee-machine");
SimpleConsumedThing? consumedThing = await consumer.Consume(td) as SimpleConsumedThing;
if(consumedThing != null) await consumedThing.WriteProperty<int>("availableResourceLevel", 100 ,new InteractionOptions { uriVariables = new Dictionary<string, object> { { "id", "water" } } });
Console.WriteLine("Done");
