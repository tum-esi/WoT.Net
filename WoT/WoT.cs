using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WoT.Definitions;
using WoT.Implementation;

namespace WoT 
{
    public interface IConsumer
    {
        Task<IConsumedThing> Consume(ThingDescription td);
        Task<ThingDescription> RequestThingDescription(string url);
    }

    public interface IProducer
    {
        Task<IExposedThing> Produce(ThingDescription td);
    }

    public interface IServient: IConsumer, IProducer
    {

    }

    /// <summary>
    /// An Interface for InteractionOutputs with no output data
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
    public interface IInteractionOutput<T>
    {
        Stream Data { get; }
        bool DataUsed { get; }
        Form Form { get; }
        IDataSchema Schema { get; }
        Task<byte[]> ArrayBuffer();
        Task<T> Value();

    }

    public interface ISubscription
    {
        bool Active { get; }
        Task Stop(InteractionOptions options);
    }

    public interface IConsumedThing
    {
        Task<IInteractionOutput<T>> ReadProperty<T>(string propertyName, InteractionOptions? options = null);
        Task WriteProperty<T>(string propertyName, T value, InteractionOptions? options = null);
        Task<IInteractionOutput> InvokeAction(string actionName, InteractionOptions? options = null);
        Task<IInteractionOutput> InvokeAction<U>(string actionName, U parameters, InteractionOptions? options = null);
        Task<IInteractionOutput<T>> InvokeAction<T>(string actionName, InteractionOptions? options = null);
        Task<IInteractionOutput<T>> InvokeAction<T, U>(string actionName, U parameters, InteractionOptions? options = null);
        Task<ISubscription> ObserveProperty<T>(string propertyName, Action<T> listener, InteractionOptions? options = null);
        Task<ISubscription> ObserveProperty<T>(string propertyName, Action<T> listener, Action<Exception> onerror, InteractionOptions? options = null);
        Task<ISubscription> SubscribeEvent<T>(string eventName, Action<T> listener, InteractionOptions? options = null);
        Task<ISubscription> SubscribeEvent<T>(string eventName, Action<T> listener, Action<Exception> onerror, InteractionOptions? options = null);

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
