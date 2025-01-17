using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Tavis.UriTemplates;
using WoT.Definitions;
using WoT.Errors;
using WoT.ProtocolBindings;
namespace WoT.Implementation
{
    public class ConsumedThing:IConsumedThing
    {

        private readonly ThingDescription _td;
        private readonly Consumer _consumer;
        private readonly Dictionary<string, ISubscription> _activeSubscriptions;
        private readonly Dictionary<string, ISubscription> _activeObservations;

        public ConsumedThing(ThingDescription thingDescription, Consumer consumer)
        {
            this._td = thingDescription;
            this._consumer = consumer;
            _activeObservations = new Dictionary<string, ISubscription>();
            _activeSubscriptions = new Dictionary<string, ISubscription>();

        }

        #region property operations
        public async Task<IInteractionOutput<T>> ReadProperty<T>(string propertyName, InteractionOptions? options = null)
        {
            var properties = this._td.Properties;
            IProtocolClient protocolClient;
            Form form;
            // 3. Let interaction be [[td]].properties.propertyName.
            if (!properties.TryGetValue(propertyName, out var propertyAffordance))
            {
                // 4. If interaction is undefined, reject promise with a NotFoundError and stop.
                throw new NotFoundError($"Property {propertyName} not found in TD with ID {_td.Id}");
            }
            else
            {
                if (propertyAffordance.WriteOnly == true) throw new NotAllowedError($"Cannot read writeOnly property {propertyName}");
                ClientAndForm clientAndForm = _consumer.GetClientFor(propertyAffordance.Forms, "readproperty", options);
                protocolClient = clientAndForm.protocolClient;
                form = clientAndForm.form;
                if (options.HasValue && options.Value.uriVariables != null) 
                    form = HandleUriVariables(form, options.Value.uriVariables);
                if (form == null || protocolClient == null) throw new NotFoundError($"Could not find form/client that allows reading property {propertyName}");
                Stream responseStream = await protocolClient.SendGetRequest(form);
                InteractionOutput<T> output = new InteractionOutput<T>(propertyAffordance, form, responseStream);
                return output;
            }
        }

        public async Task WriteProperty<T>(string propertyName, T value, InteractionOptions? options = null)
        {
            var properties = this._td.Properties;
            IProtocolClient protocolClient;
            Form form;
            // 3. Let interaction be [[td]].properties.propertyName.
            if (!properties.TryGetValue(propertyName, out var propertyAffordance))
            {
                // 4. If interaction is undefined, reject promise with a NotFoundError and stop.
                throw new Exception($"Property {propertyName} not found in TD with ID {_td.Id}");
            }
            else
            {
                if (propertyAffordance.ReadOnly == true) throw new Exception($"Cannot write readOnly property {propertyName}");
                ClientAndForm clientAndForm = _consumer.GetClientFor(propertyAffordance.Forms, "writeproperty", options);
                protocolClient = clientAndForm.protocolClient;
                form = clientAndForm.form;
                // Handle UriVariables
                if (options.HasValue && options.Value.uriVariables != null) form = HandleUriVariables(form, options.Value.uriVariables);

                if (form == null) throw new Exception($"Could not find a form that allows writing property {propertyName}");

                await protocolClient.SendPutRequest(form, value);
                
            }
        }

        public async Task<ISubscription> ObserveProperty<T>(string propertyName, Action<IInteractionOutput<T>> listener, InteractionOptions? options = null)
        {
            Subscription subscription = null;
            PropertyAffordance propertyAffordance = null;
            try
            {
                if (listener.GetType() == typeof(Action))
                    throw new TypeError("Listener for property " + propertyName + " specified is not a function.");

                if (_activeObservations.ContainsKey(propertyName))
                    throw new NotAllowedError("Property " + propertyName + " has an already active subscription.");

                if (!_td.Properties.TryGetValue(propertyName, out propertyAffordance))
                    throw new NotFoundError("Property " + propertyName + " was not found in TD. TD Title:" + _td.Title + ".");

                ConsumedThing consumedThing = this;
                Form[] forms = propertyAffordance.Forms;
                ClientAndForm clientAndForm = consumedThing._consumer.GetClientFor(forms, "observeproperty", options, "application/json", "longpoll");
                Form form = clientAndForm.form;
                IProtocolClient protocolClient = clientAndForm.protocolClient;

                if (options.HasValue && options.Value.uriVariables != null)
                    form = HandleUriVariables(form, options.Value.uriVariables);

                subscription = new Subscription(Subscription.SubscriptionType.Observation, propertyName, propertyAffordance, form, this);
                try
                {
                    var task = Task.Run(async () =>
                    {
                        while (subscription.Active)
                        {
                            subscription.StopObservation += (sender, args) => subscription.tokenSource.Cancel();
                            Stream responseStream = await protocolClient.SendGetRequest(form, subscription.CancellationToken);
                            InteractionOutput<T> output = new InteractionOutput<T>(propertyAffordance, form, responseStream);
                            listener.Invoke(output);
                        }
                    }, subscription.CancellationToken);
                    
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Unobserved to property: " + propertyName + " of TD " + _td.Title);
                }
                return subscription;
            }
            catch (TypeError e)
            {
                throw e;
            }
            catch (NotAllowedError e)
            {
                Console.WriteLine(e.ToString() + " The subscription is ignored");
                return null;
            }
        }

        public async Task<ISubscription> ObserveProperty<T>(string propertyName, Action<IInteractionOutput<T>> listener, Action<Exception> onerror, InteractionOptions? options = null)
        {
            Subscription subscription = null;
            PropertyAffordance propertyAffordance = null;
            try
            {
                if (listener.GetType() == typeof(Action))
                    throw new TypeError("Listener for property " + propertyName + " specified is not a function.");

                if (_activeObservations.ContainsKey(propertyName))
                    throw new NotAllowedError("Property " + propertyName + " has an already active subscription.");

                if (!_td.Properties.TryGetValue(propertyName, out propertyAffordance))
                    throw new NotFoundError("Property " + propertyName + " was not found in TD. TD Title:" + _td.Title + ".");

                ConsumedThing consumedThing = this;
                Form[] forms = propertyAffordance.Forms;
                ClientAndForm clientAndForm = consumedThing._consumer.GetClientFor(forms, "observeproperty", options, "application/json", "longpoll");
                Form form = clientAndForm.form;
                IProtocolClient protocolClient = clientAndForm.protocolClient;

                if (options.HasValue && options.Value.uriVariables != null)
                    form = HandleUriVariables(form, options.Value.uriVariables);

                subscription = new Subscription(Subscription.SubscriptionType.Observation, propertyName, propertyAffordance, form, this);
                try
                {
                    var task = Task.Run(async () =>
                    {
                        while (subscription.Active)
                        {
                            subscription.StopObservation += (sender, args) => subscription.tokenSource.Cancel();
                            Stream responseStream = await protocolClient.SendGetRequest(form, subscription.CancellationToken);
                            InteractionOutput<T> output = new InteractionOutput<T>(propertyAffordance, form, responseStream);
                            listener.Invoke(output);
                        }
                    }, subscription.CancellationToken);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Unobserved to property: " + propertyName + " of TD " + _td.Title);
                }
                catch (HttpRequestException e)
                {
                    onerror(e);
                }
                return subscription;
            }
            catch (TypeError e)
            {
                throw e;
            }
            catch (NotAllowedError e)
            {
                Console.WriteLine(e.ToString() + " The observation is ignored");
                return null;
            }
        }


        #endregion
        #region action operations
        public async Task<IInteractionOutput> InvokeAction(string actionName, InteractionOptions? options = null)
        {
            var actions = this._td.Actions;
            
            if (!actions.TryGetValue(actionName, out var actionAffordance))
            {
                // 4. If interaction is undefined, reject promise with a NotFoundError and stop.
                throw new Exception($"Property {actionName} not found in TD with ID {_td.Id}");
            }
            else
            {
                ClientAndForm clientAndForm = _consumer.GetClientFor(actionAffordance.Forms, null, options, "application/json");
                Form form = clientAndForm.form;
                IProtocolClient protocolClient = clientAndForm.protocolClient;
                // Handle UriVariables
                if (options.HasValue && options.Value.uriVariables != null) form = HandleUriVariables(form, options.Value.uriVariables);

                if (form == null) throw new Exception($"Could not find a form that allows invoking action {actionName}");
                Console.Write(form.Href);
                await protocolClient.SendPostRequest(form);
               
                InteractionOutput output = new InteractionOutput(form);
                return output;
            }
        }

        public async Task<IInteractionOutput> InvokeAction<U>(string actionName, U parameters, InteractionOptions? options = null)
        {
            var actions = this._td.Actions;
            if (!actions.TryGetValue(actionName, out var actionAffordance))
            {
                // 4. If interaction is undefined, reject promise with a NotFoundError and stop.
                throw new Exception($"Property {actionName} not found in TD with ID {_td.Id}");
            }
            else
            {
                // find suitable form
                ClientAndForm clientAndForm = _consumer.GetClientFor(actionAffordance.Forms, null, options, "application/json");
                Form form = clientAndForm.form;
                IProtocolClient protocolClient = clientAndForm.protocolClient;
                // Handle UriVariables
                if (options.HasValue && options.Value.uriVariables != null) form = HandleUriVariables(form, options.Value.uriVariables);

                if (form == null) throw new Exception($"Could not find a form that allows reading property {actionName}");
               
                await protocolClient.SendPostRequest<U>(form, parameters);
                
                InteractionOutput output = new InteractionOutput(form);
                return output;
            }
        }

        public async Task<IInteractionOutput<T>> InvokeAction<T>(string actionName, InteractionOptions? options = null)
        {
            var actions = this._td.Actions;
            if (!actions.TryGetValue(actionName, out var actionAffordance))
            {
                // 4. If interaction is undefined, reject promise with a NotFoundError and stop.
                throw new Exception($"Property {actionName} not found in TD with ID {_td.Id}");
            }
            else
            {
                // find suitable form
                ClientAndForm clientAndForm = _consumer.GetClientFor(actionAffordance.Forms, null, options, "application/json");
                Form form = clientAndForm.form;
                IProtocolClient protocolClient = clientAndForm.protocolClient;
                // Handle UriVariables
                if (options.HasValue && options.Value.uriVariables != null) form = HandleUriVariables(form, options.Value.uriVariables);

                if (form == null) throw new Exception($"Could not find a form that allows reading property {actionName}");
                Console.Write(form.Href);
                Stream responseStream = await protocolClient.SendPostRequest(form);
                InteractionOutput<T> output = new InteractionOutput<T>(actionAffordance.Output, form, responseStream);
                return output;
            }
        }

        public async Task<IInteractionOutput<T>> InvokeAction<T, U>(string actionName, U parameters, InteractionOptions? options = null)
        {
            var actions = this._td.Actions;
            if (!actions.TryGetValue(actionName, out var actionAffordance))
            {
                // 4. If interaction is undefined, reject promise with a NotFoundError and stop.
                throw new Exception($"Property {actionName} not found in TD with ID {_td.Id}");
            }
            else
            {
                // find suitable form
                ClientAndForm clientAndForm = _consumer.GetClientFor(actionAffordance.Forms, null, options, "application/json");
                Form form = clientAndForm.form;
                IProtocolClient protocolClient = clientAndForm.protocolClient;
                // Handle UriVariables
                if (options.HasValue && options.Value.uriVariables != null) form = HandleUriVariables(form, options.Value.uriVariables);

                Stream responseStream = await protocolClient.SendPostRequest<U>(form, parameters);
                InteractionOutput<T> output = new InteractionOutput<T>(actionAffordance.Output, form, responseStream);
                return output;
            }
        }
        #endregion
        #region event operations
        public async Task<ISubscription> SubscribeEvent(string eventName, Action listener, InteractionOptions? options = null)
        {
            Subscription subscription = null;
            EventAffordance eventAffordance = null;
            try
            {
                if (listener.GetType() == typeof(Action))
                    throw new TypeError($"Listener for event {eventName} specified is not a function.");

                if (_activeSubscriptions.ContainsKey(eventName))
                    throw new NotAllowedError($"Event {eventName} has an already active subscription.");

                if (!_td.Events.TryGetValue(eventName, out eventAffordance))
                    throw new NotFoundError($"Event {eventName} was not found in TD. TD Title: {_td.Title}.");

                Form[] forms = eventAffordance.Forms;
                ClientAndForm clientAndForm = _consumer.GetClientFor(forms, "subscribeevent", options, "application/json", "longpoll");
                Form form = clientAndForm.form;
                IProtocolClient protocolClient = clientAndForm.protocolClient;

                if (options.HasValue && options.Value.uriVariables != null)
                    form = HandleUriVariables(form, options.Value.uriVariables);

                subscription = new Subscription(Subscription.SubscriptionType.Event, eventName, eventAffordance, form, this);
                try
                {
                    var task = Task.Run(async () =>
                    {
                        while (subscription.Active)
                        {
                            subscription.StopObservation += (sender, args) => subscription.tokenSource.Cancel();
                            await protocolClient.SendGetRequest(form, subscription.CancellationToken);
                            listener.Invoke();
                        }
                    }, subscription.CancellationToken);
                    //TODO: await task to for proper use of async method, and test
                    //await task;
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine($"Unsubscribed to event: {eventName} of TD {_td.Title}");
                }

                return subscription;
            }
            catch (TypeError e)
            {
                throw e;
            }
            catch (NotAllowedError e)
            {
                Console.WriteLine(e.ToString() + " The subscription is ignored");
                return null;
            }
        }

        public async Task<ISubscription> SubscribeEvent(string eventName, Action listener, Action<Exception> onerror, InteractionOptions? options = null)
        {
            Subscription subscription = null;
            EventAffordance eventAffordance = null;
            try
            {
                if (listener.GetType() == typeof(Action))
                    throw new TypeError($"listener for event {eventName} specified is not a function.");

                if (_activeSubscriptions.ContainsKey(eventName))
                    throw new NotAllowedError($"Event {eventName} has an already active subscription.");

                if (!_td.Events.TryGetValue(eventName, out eventAffordance))
                    throw new NotFoundError($"Event {eventName} was not found in TD. TD Title: {_td.Title}.");

                Form[] forms = eventAffordance.Forms;
                ClientAndForm clientAndForm = _consumer.GetClientFor(forms, "subscribeevent", options, "application/json", "longpoll");
                Form form = clientAndForm.form;
                IProtocolClient protocolClient = clientAndForm.protocolClient;

                if (options.HasValue && options.Value.uriVariables != null)
                    form = HandleUriVariables(form, options.Value.uriVariables);

                subscription = new Subscription(Subscription.SubscriptionType.Event, eventName, eventAffordance, form, this);
                try
                {
                    var task = Task.Run(async () =>
                    {
                        while (subscription.Active)
                        {
                            subscription.StopObservation += (sender, args) => subscription.tokenSource.Cancel();
                            await protocolClient.SendGetRequest(form, subscription.CancellationToken);
                            listener.Invoke();
                        }
                    }, subscription.CancellationToken);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine($"Unsubscribed to event: {eventName} of TD {_td.Title}");
                }
                catch (HttpRequestException e)
                {
                    onerror(e);
                }
                return subscription;
            }
            catch (TypeError e)
            {
                throw e;
            }
            catch (NotAllowedError e)
            {
                Console.WriteLine(e.ToString() + " The subscription is ignored");
                return null;
            }
        }

        public async Task<ISubscription> SubscribeEvent<T>(string eventName, Action<IInteractionOutput<T>> listener, InteractionOptions? options = null)
        {
            Subscription subscription = null;
            EventAffordance eventAffordance = null;
            try
            {
                if (listener.GetType() == typeof(Action))
                    throw new TypeError($"Listener for event {eventName} specified is not a function.");

                if (_activeSubscriptions.ContainsKey(eventName))
                    throw new NotAllowedError($"Event {eventName} has an already active subscription.");

                if (!_td.Events.TryGetValue(eventName, out eventAffordance))
                    throw new NotFoundError($"Event {eventName} was not found in TD. TD Title: {_td.Title}.");

                Form[] forms = eventAffordance.Forms;
                ClientAndForm clientAndForm = _consumer.GetClientFor(forms, "observeproperty", options, "application/json", "longpoll");
                Form form = clientAndForm.form;
                IProtocolClient protocolClient = clientAndForm.protocolClient;

                if (options.HasValue && options.Value.uriVariables != null)
                    form = HandleUriVariables(form, options.Value.uriVariables);

                subscription = new Subscription(Subscription.SubscriptionType.Event, eventName, eventAffordance, form, this);
                try
                {
                    var task = Task.Run(async () =>
                    {
                        while (subscription.Active)
                        {
                            subscription.StopObservation += (sender, args) => subscription.tokenSource.Cancel();
                            Stream responseStream = await protocolClient.SendGetRequest(form, subscription.CancellationToken);
                            InteractionOutput<T> output = new InteractionOutput<T>(eventAffordance.Data, form, responseStream);
                            listener.Invoke(output);
                        }
                    }, subscription.CancellationToken);
                }

                catch (TaskCanceledException)
                {
                    Console.WriteLine($"Unsubscribed to event: {eventName} of TD {_td.Title}");
                }

                return subscription;
            }
            catch (TypeError e)
            {
                throw e;
            }
            catch (NotAllowedError e)
            {
                Console.WriteLine(e.ToString() + " The subscription is ignored");
                return null;
            }
        }

        public async Task<ISubscription> SubscribeEvent<T>(string eventName, Action<IInteractionOutput<T>> listener, Action<Exception> onerror, InteractionOptions? options = null)
        {
            Subscription subscription = null;
            EventAffordance eventAffordance = null;
            try
            {
                if (listener.GetType() == typeof(Action))
                    throw new TypeError($"listener for event {eventName} specified is not a function.");

                if (_activeSubscriptions.ContainsKey(eventName))
                    throw new NotAllowedError($"Event {eventName} has an already active subscription.");

                if (!_td.Events.TryGetValue(eventName, out eventAffordance))
                    throw new NotFoundError($"Event {eventName} was not found in TD. TD Title: {_td.Title}.");

                Form[] forms = eventAffordance.Forms;
                ClientAndForm clientAndForm = _consumer.GetClientFor(forms, "observeproperty", options, "application/json", "longpoll");
                Form form = clientAndForm.form;
                IProtocolClient protocolClient = clientAndForm.protocolClient;

                if (options.HasValue && options.Value.uriVariables != null)
                    form = HandleUriVariables(form, options.Value.uriVariables);

                subscription = new Subscription(Subscription.SubscriptionType.Event, eventName, eventAffordance, form, this);
                try
                {
                    var task = Task.Run(async () =>
                    {
                        while (subscription.Active)
                        {
                            subscription.StopObservation += (sender, args) => subscription.tokenSource.Cancel();
                            Stream responseStream = await protocolClient.SendGetRequest(form, subscription.CancellationToken);
                            InteractionOutput<T> output = new InteractionOutput<T>(eventAffordance.Data, form, responseStream);
                            listener.Invoke(output);
                        }
                    }, subscription.CancellationToken);
                }

                catch (TaskCanceledException)
                {
                    Console.WriteLine($"Unsubscribed to event: {eventName} of TD {_td.Title}");
                }

                catch (HttpRequestException e)
                {
                    onerror(e);
                }

                return subscription;
            }
            catch (TypeError e)
            {
                throw e;
            }
            catch (NotAllowedError e)
            {
                Console.WriteLine(e.ToString() + " The subscription is ignored");
                return null;
            }
        }
        #endregion
        public ThingDescription GetThingDescription()
        {
            return _td;
        }

        

        protected Form HandleUriVariables(Form form, Dictionary<string, object> uriVariables)
        // {"id": "mariz"}
        {
            var urlTemplate = new UriTemplate(form.Href.OriginalString);
            foreach (var variable in uriVariables)
            {
                urlTemplate.AddParameter(variable.Key, variable.Value);
            }
            string extendedUrl = urlTemplate.Resolve();
            form.Href = new Uri(extendedUrl);
            Console.WriteLine(form.Href.AbsoluteUri);
            return form;
        }

        public void AddObservation(string name, Subscription sub)
        {
            _activeObservations.Add(name, sub);
        }

        public void AddSubscription(string name, Subscription sub)
        {
            _activeSubscriptions.Add(name, sub);
        }

        public void RemoveObservation(string name)
        {
            _activeObservations.Remove(name);
        }

        public void RemoveSubscription(string name)
        {
            _activeSubscriptions.Remove(name);
        }

    }


}