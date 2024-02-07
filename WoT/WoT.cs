using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WoT.Definitions;

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
    public interface IInteractionOutput
    {
        
        Stream Data { get; }
        bool DataUsed { get; }
        Form Form { get; }
        IDataSchema Schema { get; }
        Task<byte[]> ArrayBuffer();
        Task Value();
    }

    /// <summary>
    /// An interface for InteractionOutputs with output data
    /// </summary>
    /// <typeparam name="T">output data type</typeparam>
    public interface IInteractionOutput<T>
    {
        /// <summary>
        /// 
        /// </summary>
        Stream Data { get; }
        bool DataUsed { get; }
        Form Form { get; }
        IDataSchema Schema { get; }
        Task<byte[]> ArrayBuffer();
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
        Task<ISubscription> ObserveProperty<T>(string propertyName, Action<IInteractionOutput<T>> listener, InteractionOptions? options = null);
        Task<ISubscription> ObserveProperty<T>(string propertyName, Action<IInteractionOutput<T>> listener, Action<Exception> onerror, InteractionOptions? options = null);
        Task<ISubscription> SubscribeEvent(string eventName, Action listener, InteractionOptions? options = null);
        Task<ISubscription> SubscribeEvent(string eventName, Action listener, Action<Exception> onerror, InteractionOptions? options = null);
        Task<ISubscription> SubscribeEvent<T>(string eventName, Action<IInteractionOutput<T>> listener, InteractionOptions? options = null);
        Task<ISubscription> SubscribeEvent<T>(string eventName, Action<IInteractionOutput<T>> listener, Action<Exception> onerror, InteractionOptions? options = null);

    }

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
    public struct InteractionOptions
    {
        public uint? formIndex;
        public Dictionary<string, object> uriVariables;
        public object data;

    }
}
