# WoT.Net
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

## Roadmap
- [X] TD Deserializing and Parsing 
- [X] HTTP Consumer (works only with `"application\json"`, `"longpoll"` and no security)
- [ ] HTTPS Consumer
- [ ] CoAP Consumer
- [ ] CoAPS Consumer
- [ ] MQTT Consumer

... more in the future
