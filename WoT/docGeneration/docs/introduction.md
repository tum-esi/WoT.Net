# Introduction

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

## Library Structure

The library is structured as follows:

* ###  ``WoT.Core`` namespace

  Top-level namespace of the library which includes all definitions of the conformance class interfaces as described in the [WoT Scripting API](https://www.w3.org/TR/wot-scripting-api/). These are ``Cosumer``, ``Producer`` and ``Discovery``. These interfaces can be implemented to extend the capabilities of the stack.

* ### ``WoT.Core.Definitions`` namespace

  This namespace includes all interfaces and classes required for defining the ``ThingDescription`` class as described in the [TD Specification](https://www.w3.org/TR/wot-thing-description11/). This class is used as a cornerstone for building the rest of the library.

* ### ``WoT.Core.Implementation`` namespace

  This namespace includes all implementations of the interfaces defined in ``WoT`` namespace. These implementations provide the capability of implementing and/or interacting with WoT Entities.

* ### ``WoT.Core.Errors`` namespace
  
  This namespace contains definitions of all errors (exceptions) defined in the context of WoT

* ### ``WoT.Core.TDHelpers`` namespace
  
  This namespace contains all ``JSONConverter``s and needed to serialize and deserialize the TD.
