// See https://aka.ms/new-console-template for more informatio5n
using WoT;
using WoT.Definitions;
using WoT.Implementation;

Consumer consumer = new Consumer();
consumer.AddClient(new SimpleHTTPClient());
ThingDescription coffeeMachineTD = await consumer.RequestThingDescription("http://plugfest.thingweb.io:8083/smart-coffee-machine");
ThingDescription counterTD = await consumer.RequestThingDescription("http://plugfest.thingweb.io:8083/counter");
ConsumedThing coffeeMachineConsumedThing = consumer.Consume(coffeeMachineTD) as ConsumedThing;
ConsumedThing counterConsumedThing = consumer.Consume(counterTD) as ConsumedThing;
Action<IInteractionOutput<int>> countListener = async (IInteractionOutput<int> count) => { Console.WriteLine(await count.Value()); };
if (coffeeMachineConsumedThing != null)
{
    //ReadProperty test
    int count = await counterConsumedThing.ReadProperty<int>("count", new InteractionOptions { uriVariables = new Dictionary<string, object> { { "step", 5 } } }).Result.Value();
    Console.WriteLine("Count = " + count);


    //ObserveProperty test
    ISubscription countObserver = await counterConsumedThing.ObserveProperty<int>("count", countListener);

    //InvokeAction test
    IInteractionOutput actionOutput = await counterConsumedThing.InvokeAction("increment", options: new InteractionOptions { uriVariables = new Dictionary<string, object> { { "step", 5 } } }) ; 
    
    //WriteProperty
    await coffeeMachineConsumedThing.WriteProperty<int>("availableResourceLevel", 100, new InteractionOptions { uriVariables = new Dictionary<string, object> { { "id", "water" } } });

}


Console.WriteLine("Done");
Console.ReadLine();
