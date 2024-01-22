// See https://aka.ms/new-console-template for more information
using WoT;

WoT.Implementation.SimpleHTTPConsumer consumer = new WoT.Implementation.SimpleHTTPConsumer();
var td = await consumer.RequestThingDescription("http://remotelab.esi.cit.tum.de:8080/virtual-coffee-machine-1_4");
Console.WriteLine(td.ToString());
