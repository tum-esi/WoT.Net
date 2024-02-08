using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WoT.Definitions;
using WoT.Errors;

namespace WoT 
{
    /// <summary>
    /// An interface describing the capabilities of a <see ref="https://www.w3.org/TR/wot-scripting-api/#dfn-wot-consumer">WoT Consumer</see>
    /// </summary>
    public interface IConsumer
    {
        /// <summary>
        /// Expects an <c>td</c> argument and returns a <see cref="Task"/> that resolves with an object implementing <see cref="IConsumedThing"/> interface that represents a client interface to operate with the Thing.
        /// </summary>
        /// <param name="td">TD of the client Thing</param>
        /// <returns> Task that resolves with an object implementing <see cref="IConsumedThing"/> interface </returns>
        /// <seealso href="https://www.w3.org/TR/wot-scripting-api/#the-consume-method">WoT Scripting API</seealso>
        Task<IConsumedThing> Consume(ThingDescription td);
    }

    /// <summary>
    /// An interface describing the capabilities of <see ref="https://www.w3.org/TR/wot-scripting-api/#dfn-wot-producer">WoT Producer</see>
    /// </summary>
    public interface IProducer
    {
        Task<IExposedThing> Produce(ThingDescription td);
    }

    /// <summary>
    /// An interface describing the capabilities of requesting a TD, part of <see ref="https://www.w3.org/TR/wot-scripting-api/#dfn-wot-discovery">WoT Discovery</see> interface
    /// </summary>
    public interface IRequester
    {
        /// <summary>
        /// Requests a Thing Description from the given URL.
        /// </summary>
        /// <param name="url">URL as a string</param>
        /// <returns>Deserialized TD</returns>
        Task<ThingDescription> RequestThingDescription(string url);

        /// <summary>
        /// Requests a Thing Description from the given URL.
        /// </summary>
        /// <param name="url">URL as a URI object</param>
        /// <returns>Deserialized TD</returns>
        Task<ThingDescription> RequestThingDescription(Uri url);
    }

    /// <summary>
    /// An interface describing the capabilities of <see ref="https://www.w3.org/TR/wot-scripting-api/#dfn-wot-discovery">WoT Discovery</see>
    /// </summary>
    public interface IDiscovery: IRequester
    {
        
    }

    /// <summary>
    /// An interface describing the capabilities of Servient implementing all other conformance interfaces <see cref="IConsumer"/>, <see cref="IProducer"/>, <see cref="IDiscovery"/>
    /// </summary>
    public interface IServient: IConsumer, IProducer, IDiscovery
    {

    }

    /// <summary>
    /// An interface for InteractionOutputs with no output data
    /// </summary>
    /// <remarks>
    /// As this <c>InteractionOutput</c> interface represents and output with no data, all properties of this interface will be set to <c>null</c>
    /// </remarks>
    public interface IInteractionOutput
    {
        /// <summary>
        /// Represents the raw payload in WoT Interactions as a <see cref="Stream"/>
        /// </summary>
        /// <value>
        /// <c>null</c>
        /// </value>
        Stream Data { get; }

        /// <summary>
        /// Tells whether the data stream has been disturbed
        /// </summary>
        /// <value>
        /// <c>false</c>
        /// </value>
        bool DataUsed { get; }

        /// <summary>
        /// Represents the <see cref="Form"/> selected from the <see cref="ThingDescription"/> for this WoT <see cref="InteractionAffordance"/>
        /// </summary>
        /// <value>
        /// <c>selected form</c>
        /// </value>
        Form Form { get; }

        /// <summary>
        /// Represents the <see cref="DataSchema"/> of the payload.
        /// </summary>
        /// <value>
        /// <c>null</c>
        /// </value>
        IDataSchema Schema { get; }

        /// <summary>
        /// Returns data stream as array of bytes
        /// </summary>
        /// <returns><see cref="Task"/> that resolves with empty array</returns>
        Task<byte[]> ArrayBuffer();

        /// <summary>
        /// Parses the data returned by the WoT <see cref="InteractionAffordance"/> and returns a value with the type described by the interaction <see href="DataSchema"/> if that exists, or by the <see cref="Form.ContentType"/> of the interaction <see cref="WoT.Definitions.Form"/>.
        /// </summary>
        /// <returns><see cref="Task"/> with no output</returns>
        Task Value();
    }

    /// <summary>
    /// An interface for InteractionOutputs with output data
    /// </summary>
    /// <typeparam name="T">output data type</typeparam>
    public interface IInteractionOutput<T>
    {
        /// <summary>
        /// Represents the raw payload in WoT Interactions as a <see cref="Stream"/>
        /// </summary>
        /// <value>
        /// <c>output data stream</c>
        /// </value>
        Stream Data { get; }

        /// <summary>
        /// Tells whether the data stream has been disturbed
        /// </summary>
        /// <value>
        /// <c>data stream distribution status</c>
        /// </value>
        bool DataUsed { get; }

        /// <summary>
        /// Represents the <see cref="Form"/> selected from the <see cref="ThingDescription"/> for this WoT <see cref="InteractionAffordance"/>
        /// </summary>
        /// <value>
        /// <c>selected <see cref="WoT.Definitions.Form"/></c>
        /// </value>
        Form Form { get; }

        /// <summary>
        /// Represents the <see cref="DataSchema"/> of the payload.
        /// </summary>
        /// <value>
        /// <c>schema of payload used for validation</c>
        /// </value>
        IDataSchema Schema { get; }

        /// <summary>
        /// Returns data stream as array of bytes
        /// </summary>
        /// <returns><see cref="Task"/> that resolves with data represented as <c>byte[]</c></returns>
        Task<byte[]> ArrayBuffer();

        /// <summary>
        /// Parses the data returned by the WoT <see cref="InteractionAffordance"/> and returns a value with the type described by the interaction <see href="DataSchema"/> if that exists, or by the <see cref="Form.ContentType"/> of the interaction <see cref="WoT.Definitions.Form"/>.
        /// </summary>
        /// <returns><see cref="Task"/> that resolves to data of type <typeparamref name="T"/></returns>
        Task<T> Value();

    }

    /// <summary>
    /// Represents a subscription to Property change and Event interactions.
    /// </summary>
    /// <remarks>
    /// The <see cref="Active"/> boolean property denotes if the subscription is active, i.e. it is not stopped because of an error or because of invocation of the <see cref="Stop(InteractionOptions?)"/> method.
    /// </remarks>
    public interface ISubscription
    {
        /// <summary>
        /// Describes if the current <c>Subscription</c> is active or not
        /// </summary>
        /// <value>
        /// current state of <c>Subscription</c>
        /// </value>
        bool Active { get; }

        /// <summary>
        /// Stops delivering notifications for the subscription. It takes an optional parameter <c>options</c> and returns a <see cref="Task"/>.
        /// </summary>
        /// <param name="options">options passed for interaction</param>
        /// <returns><see cref="Task"/> that resolves when the stopping process finishes</returns>
        Task Stop(InteractionOptions? options = null);
    }

    /// <summary>
    /// Represents a client API to operate a Thing. Belongs to the <see ref="https://www.w3.org/TR/wot-scripting-api/#dfn-wot-consumer">WoT Consumer</see> conformance class.
    /// </summary>
    public interface IConsumedThing
    {
        /// <summary>
        /// Reads a Property value.
        /// </summary>
        /// <typeparam name="T">type of Property value</typeparam>
        /// <param name="propertyName">name of Property that should be read</param>
        /// <param name="options">additional options for performing the interaction</param>
        /// <returns><see cref="Task"/> that resolves with <see cref="IInteractionOutput{T}"/> that represents Property value</returns>
        Task<IInteractionOutput<T>> ReadProperty<T>(string propertyName, InteractionOptions? options = null);

        /// <summary>
        /// Writes a single Property.
        /// </summary>
        /// <typeparam name="T">type of Property value</typeparam>
        /// <param name="propertyName">name of Property that should be written to</param>
        /// <param name="value">value that should be written</param>
        /// <param name="options">additional options for performing the interaction</param>
        /// <returns><see cref="Task"/> that finishes when interaction is performed and a response is received</returns>
        Task WriteProperty<T>(string propertyName, T value, InteractionOptions? options = null);

        /// <summary>
        /// Makes a request for invoking an Action. Does not send or receive a payload.
        /// </summary>
        /// <param name="actionName">name of Action to be invoked</param>
        /// <param name="options">additional options for performing the interaction</param>
        /// <returns><see cref="Task"/> that finishes when interaction is performed and a response is received</returns>
        Task<IInteractionOutput> InvokeAction(string actionName, InteractionOptions? options = null);

        /// <summary>
        /// Makes a request for invoking an Action. Sends a payload. Does not receive a payload.
        /// </summary>
        /// <typeparam name="U">type of payload parameters</typeparam>
        /// <param name="actionName">name of Action to be invoked</param>
        /// <param name="parameters">paramters send in the payload</param>
        /// <param name="options">additional options for performing the interaction</param>
        /// <returns><see cref="Task"/> that finishes when interaction is performed and a response is received</returns>
        Task<IInteractionOutput> InvokeAction<U>(string actionName, U parameters, InteractionOptions? options = null);

        /// <summary>
        /// Makes a request for invoking an Action. Does not send a payload. Receives a payload.
        /// </summary>
        /// <typeparam name="T">type of response payload value</typeparam>
        /// <param name="actionName">name of Action to be invoked</param>
        /// <param name="options">additional options for performing the interaction</param>
        /// <returns><see cref="Task"/> that resolves with <see cref="IInteractionOutput{T}"/> representing value of response payload</returns>
        Task<IInteractionOutput<T>> InvokeAction<T>(string actionName, InteractionOptions? options = null);

        /// <summary>
        /// Makes a request for invoking an Action. Sends and receives payload.
        /// </summary>
        /// <typeparam name="T">type of response payload value</typeparam>
        /// <typeparam name="U">type of payload parameters</typeparam>
        /// <param name="actionName">name of Action to be invoked</param>
        /// <param name="parameters">paramters send in the payload</param>
        /// <param name="options">additional options for performing the interaction</param>
        /// <returns><see cref="Task"/> that resolves with <see cref="IInteractionOutput{T}"/> representing value of response payload</returns>
        Task<IInteractionOutput<T>> InvokeAction<T, U>(string actionName, U parameters, InteractionOptions? options = null);

        /// <summary>
        /// Makes a request for Property value change notifications.
        /// </summary>
        /// <typeparam name="T">type of Property value</typeparam>
        /// <param name="propertyName">name of Property to be observed</param>
        /// <param name="listener">a callback function that is executed once a Property value change was notified; takes <see cref="IInteractionOutput{T}"/> as input representing received data</param>
        /// <param name="options">additional options for performing the interaction</param>
        /// <returns><see cref="Task"/> that resolves with <see cref="ISubscription"/> representing the active subscription</returns>
        /// <exception cref="NotAllowedError"/>
        Task<ISubscription> ObserveProperty<T>(string propertyName, Action<IInteractionOutput<T>> listener, InteractionOptions? options = null);

        /// <inheritdoc cref="ObserveProperty{T}(string, Action{IInteractionOutput{T}}, InteractionOptions?)"/>
        /// <param name="onerror">a callback function that executed if the request fails; takes <see cref="Exception"/> as input</param>
        Task<ISubscription> ObserveProperty<T>(string propertyName, Action<IInteractionOutput<T>> listener, Action<Exception> onerror, InteractionOptions? options = null);

        /// <summary>
        /// Makes a request for subscribing to Event notifications that do not provide data.
        /// </summary>
        /// <param name="eventName">name of Event</param>
        /// <param name="listener">a callback function that is executed once notified. Does not take any parameters</param>
        /// <param name="options">additional options for performing the interaction</param>
        /// <returns><see cref="Task"/> that resolves with <see cref="ISubscription"/> representing the active subscription</returns>
        Task<ISubscription> SubscribeEvent(string eventName, Action listener, InteractionOptions? options = null);

        /// <inheritdoc cref="ObserveProperty{T}(string, Action{IInteractionOutput{T}}, InteractionOptions?)"/>
        /// <param name="onerror">a callback function that executed if the request fails; takes <see cref="Exception"/> as input</param>
        Task<ISubscription> SubscribeEvent(string eventName, Action listener, Action<Exception> onerror, InteractionOptions? options = null);

        /// <summary>
        /// Makes a request for subscribing to Event notifications that provide data.
        /// </summary>
        /// <typeparam name="T">type of received payload data</typeparam>
        /// <param name="eventName">name of Event</param>
        /// <param name="listener">a callback function that is executed once notified. Takes <see cref="IInteractionOutput{T}"/> as input representing received data</param>
        /// <param name="options">additional options for performing the interaction</param>
        /// <returns><see cref="Task"/> that resolves with <see cref="ISubscription"/> representing the active subscription</returns>
        Task<ISubscription> SubscribeEvent<T>(string eventName, Action<IInteractionOutput<T>> listener, InteractionOptions? options = null);

        /// <inheritdoc cref="SubscribeEvent{T}(string, Action{IInteractionOutput{T}}, InteractionOptions?)"/>
        /// <param name="onerror">a callback function that executed if the request fails; takes <see cref="Exception"/> as input</param>
        Task<ISubscription> SubscribeEvent<T>(string eventName, Action<IInteractionOutput<T>> listener, Action<Exception> onerror, InteractionOptions? options = null);

        /// <summary>
        /// Returns the <see cref="ThingDescription"/> of the <see cref="IConsumedThing"/> object that represents the Thing Description of the <see cref="IConsumedThing"/>. Applications may consult the Thing metadata stored in <see cref="ThingDescription"/> in order to introspect its capabilities before interacting with it.
        /// </summary>
        /// <returns>TD of Consumed Thing</returns>
        ThingDescription GetThingDescription();

    }

    /// <summary>
    /// 
    /// </summary>
    public interface IExposedThing
    {
        IExposedThing SetPropertyReadHandler(string name, Action<InteractionOptions?> propertyReadHandler);
        IExposedThing SetPropertyWriteHandler<T>(string name, Action<IInteractionOutput<T>, InteractionOptions?> propertyWriteHandler);
        IExposedThing SetPropertyObserveHandler(string name, Action<InteractionOptions?> propertyReadHandler);
        IExposedThing SetPropertyUnobserveHandler(string name, Action<InteractionOptions?> propertyReadHandler);
        Task EmitPropertyChange(string name);
        Task EmitPropertyChange<T>(string name, T data);
        IExposedThing SetActionHandler<T>(string name, Action<IInteractionOutput<T>, InteractionOptions?> ActionHandler);
        IExposedThing SetEventSubscribeHandler(string name, Action<InteractionOptions?> handler);
        IExposedThing SetEventUnsubscribeHandler(string name, Action<InteractionOptions?> handler);
        IExposedThing EmitEvent(string name, Action<InteractionOptions?> propertyReadHandler);
        Task Expose();
        Task Destroy();
        ThingDescription GetThingDescription();

    }

    /// <summary>
    /// Holds the interaction options that need to be exposed for application scripts.
    /// </summary>
    public struct InteractionOptions
    {
        /// <summary>
        /// Represents an application hint for which Form definition, identified by this index, of the TD to use for the given WoT interaction.
        /// </summary>
        public uint? formIndex;

        /// <summary>
        /// Represents the URI template variables to be used with the WoT Interaction
        /// </summary>
        public Dictionary<string, object> uriVariables;

        /// <summary>
        /// Represents additional opaque data that needs to be passed to the interaction.
        /// </summary>
        public object data;

    }
}
