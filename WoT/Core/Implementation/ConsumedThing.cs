using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Tavis.UriTemplates;
using WoT.Core.Definitions;
using WoT.Core.Definitions.TD;
using WoT.Core.Errors;

namespace WoT.Core.Implementation
{
    public class ConsumedThing : IConsumedThing
    {

        private readonly ThingDescription _td;
        private readonly Consumer _consumer;
        private readonly CredentialScheme _topLevelSecurityScheme = 0;
        private readonly Dictionary<string, IProtocolClient> _clients;

        private readonly Dictionary<string, ISubscription> _activeSubscriptions;
        private readonly Dictionary<string, ISubscription> _activeObservations;

        private readonly ContentSerdes _contentManager = ContentSerdes.GetInstance();


        /// <summary>
        /// Constructor for ConsumedThing
        /// </summary>
        /// <param name="thingDescription">TD of the consumed thing</param>
        /// <param name="consumer">reference to the consumer</param>
        public ConsumedThing(ThingDescription thingDescription, Consumer consumer)
        {
            _td = thingDescription;
            _consumer = consumer;
            _clients = new Dictionary<string, IProtocolClient>();
            _activeObservations = new Dictionary<string, ISubscription>();
            _activeSubscriptions = new Dictionary<string, ISubscription>();
            foreach (var sec in _td.Security)
            {
                var secDef = _td.SecurityDefinitions[sec];
                switch (secDef.Scheme)
                {
                    case "basic":
                        _topLevelSecurityScheme |= CredentialScheme.Basic;
                        break;
                    case "digest":
                        _topLevelSecurityScheme |= CredentialScheme.Digest;
                        break;
                    case "apikey":
                        _topLevelSecurityScheme |= CredentialScheme.Apikey;
                        break;
                    case "bearer":
                        _topLevelSecurityScheme |= CredentialScheme.Bearer;
                        break;
                    case "psk":
                        _topLevelSecurityScheme |= CredentialScheme.Psk;
                        break;
                    case "oauth2":
                        _topLevelSecurityScheme |= CredentialScheme.OAuth2;
                        break;
                    default:
                        break;
                }
            }

        }

        #region property operations

        /// <summary>
        /// Read a property of the ConsumedThing
        /// </summary>
        /// <typeparam name="T">output type</typeparam>
        /// <param name="propertyName">name of the property to read</param>
        /// <param name="options">options for performing the operation</param>
        /// <returns>an awaitable <see cref="Task"/> that resolves with an object implementing <see cref="IInteractionOutputValue{T}"/></returns>
        /// <exception cref="NotFoundError">Thrown if property was not found</exception>
        /// <exception cref="NotAllowedError">Thrown if property was not found</exception>
        /// <exception cref="SyntaxError">Thrown if no suitable form and client combination was found</exception>
        public async Task<IInteractionOutput<T>> ReadProperty<T>(string propertyName, InteractionOptions? options = null)
        {
            var properties = _td.Properties;
            IProtocolClient protocolClient;
            Form form;
            // 3. Let interaction be [[td]].properties.eventName.
            if (!properties.TryGetValue(propertyName, out var propertyAffordance))
            {
                // 4. If interaction is undefined, reject promise with a NotFoundError and stop.
                throw new NotFoundError($"Property {propertyName} not found in TD with ID {_td.Id}");
            }
            else
            {
                if (propertyAffordance.WriteOnly == true) throw new NotAllowedError($"Cannot read writeOnly property {propertyName}");

                // 5. If option.formIndex is defined, let form be the Form associated with formIndex in interaction.forms array,
                // otherwise let form be a Form in interaction.forms whose op is readproperty, selected by the implementation.
                ClientAndForm clientAndForm = GetClientFor(propertyAffordance.Forms, "readproperty", propertyAffordance, options);
                protocolClient = clientAndForm.ProtocolClient;
                form = clientAndForm.Form;

                // 6. If form is failure, reject promise with a SyntaxError and stop.
                if (form == null || protocolClient == null) throw new SyntaxError($"Could not find form/client that allows reading property {propertyName}");

                // 7. Make a request to the underlying platform (via the Protocol Bindings)
                // to retrieve the value of the eventName Property using form and the optional URI templates given in options.UriVariables.
                if (options.HasValue && options.Value.UriVariables != null)
                    form = HandleUriVariables(form, options.Value.UriVariables);

                Content responseContent = await protocolClient.ReadResource(form);

                InteractionOutput<T> output = new InteractionOutput<T>(responseContent, form, propertyAffordance);
                return output;
            }
        }

        public async Task WriteProperty<T>(string propertyName, T value, InteractionOptions? options = null)
        {
            var properties = _td.Properties;
            IProtocolClient protocolClient;
            Form form;
            // 3. Let interaction be [[td]].properties.eventName.
            if (!properties.TryGetValue(propertyName, out var propertyAffordance))
            {
                // 4. If interaction is undefined, reject promise with a NotFoundError and stop.
                throw new NotFoundError($"Property {propertyName} not found in TD with ID {_td.Id}");
            }
            else
            {
                if (propertyAffordance.ReadOnly == true) throw new NotAllowedError($"Cannot write readOnly property {propertyName}");
                ClientAndForm clientAndForm = GetClientFor(propertyAffordance.Forms, "writeproperty", propertyAffordance, options);
                protocolClient = clientAndForm.ProtocolClient;
                form = clientAndForm.Form;
                // Handle UriVariables
                if (options.HasValue && options.Value.UriVariables != null) form = HandleUriVariables(form, options.Value.UriVariables);

                if (form == null) throw new SyntaxError($"Could not find a form that allows writing property {propertyName}");

                await protocolClient.WriteResource(form, _contentManager.ValueToContent<T>(value, form.ContentType, propertyAffordance));
            }
        }

        public async Task<ISubscription> ObserveProperty<T>(string propertyName, Action<IInteractionOutput<T>> listener, Action<Exception> onerror = null, InteractionOptions? options = null)
        {
            PropertyAffordance propertyAffordance = _td.Properties[propertyName] ?? throw new NotFoundError($"ConsumedThing '{_td.Title}' (Id:'{_td.Id}') does not have event {propertyName}");

            ClientAndForm clientAndForm = GetClientFor(propertyAffordance.Forms, "observeproperty", propertyAffordance);

            if (clientAndForm.Form == null)
            {
                throw new SyntaxError($"ConsumedThing '{_td.Title}' did not find a suitable form for observing property affordance '{propertyName}'");
            }

            if (clientAndForm.ProtocolClient == null)
            {
                throw new SyntaxError($"ConsumedThing '${_td.Title}' did not get suitable client for ${clientAndForm.Form.Href}");
            }

            if (_activeObservations.ContainsKey(propertyName))
            {
                throw new NotAllowedError($"ConsumedThing '${_td.Title}' has already a function subscribed to '{propertyName}' .You can only subscribe once");
            }

            Form formWithoutUriTemplates = clientAndForm.Form;
            if (options.HasValue && options.Value.UriVariables != null) 
            {
                formWithoutUriTemplates = HandleUriVariables(clientAndForm.Form, options.Value.UriVariables);
            }

            var internalSub = await clientAndForm.ProtocolClient.SubscribeResource(formWithoutUriTemplates, (content) =>
            {
                try
                {
                    listener(HandleInteractionOutput<T>(content, formWithoutUriTemplates, propertyAffordance));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while processing observation for ${propertyAffordance.Title}. ${e}");
                }
            },
            (err) =>
            {
                onerror?.Invoke(err);
            },
            () => { }
            );

            Subscription sub = new Subscription(Subscription.SubscriptionType.Observation, propertyName, propertyAffordance, formWithoutUriTemplates, internalSub);
            AddObservation(propertyName, sub);
            return sub;
        }

        public async Task UnobserveProperty (string propertyName, InteractionOptions? options)
        {
            bool isFound = _td.Properties.TryGetValue(propertyName, out PropertyAffordance property);
            if (!isFound)
            {
                throw new NotFoundError($"ConsumedThing '{_td.Title}' does not have property {propertyName}");
            }

            isFound = _activeObservations.TryGetValue(propertyName, out ISubscription subI);

            if (!isFound)
            {
                throw new OperationError($"ConsumedThing '{_td.Title}' is not observing {propertyName}");
            }

            Subscription sub = (Subscription)subI;

            uint formIndex = (uint) sub.FindBestMatchingUnlinkFormIndex();
            if (!options.HasValue)
            {
                options = new InteractionOptions() { FormIndex = formIndex };
            }
            else 
            {
                options = new InteractionOptions()
                {
                    FormIndex = formIndex,
                    Data = options.Value.Data,
                    UriVariables = options.Value.UriVariables
                };
            }

            var result = GetClientFor(property.Forms, "unobserveproperty", property);

            if (result.Form == null)
            {
                throw new SyntaxError($"ConsumedThing '{_td.Title}' did not get suitable form for unobserveproperty {propertyName}");
            }

            Form formWithoutUriTemplates = result.Form;
            if (options.HasValue)
            {
                formWithoutUriTemplates = HandleUriVariables(result.Form, options.Value.UriVariables);
            }

            await result.ProtocolClient.UnlinkResource(formWithoutUriTemplates);
            await sub.Stop();
            RemoveObservation(propertyName);
        }
        #endregion
        #region action operations
        public async Task<IInteractionOutput> InvokeAction(string actionName, InteractionOptions? options = null)
        {
            var actions = _td.Actions;

            if (!actions.TryGetValue(actionName, out var actionAffordance))
            {
                // 4. If interaction is undefined, reject promise with a NotFoundError and stop.
                throw new NotFoundError($"Action {actionName} not found in TD with ID {_td.Id}");
            }
            else
            {
                ClientAndForm clientAndForm = GetClientFor(actionAffordance.Forms, "invokeaction", actionAffordance, options);
                Form form = clientAndForm.Form;
                IProtocolClient protocolClient = clientAndForm.ProtocolClient;
                // Handle UriVariables
                if (options.HasValue && options.Value.UriVariables != null) form = HandleUriVariables(form, options.Value.UriVariables);

                if (form == null) throw new SyntaxError($"Could not find a form that allows invoking action {actionName}");
                Console.Write(form.Href);
                await protocolClient.InvokeResource(form);

                InteractionOutput output = new InteractionOutput(form);
                return output;
            }
        }

        public async Task<IInteractionOutput> InvokeAction<U>(string actionName, U parameters, InteractionOptions? options = null)
        {
            var actions = _td.Actions;
            if (!actions.TryGetValue(actionName, out var actionAffordance))
            {
                // 4. If interaction is undefined, reject promise with a NotFoundError and stop.
                throw new NotFoundError($"Action {actionName} not found in TD with ID {_td.Id}");
            }
            else
            {
                // find suitable form
                ClientAndForm clientAndForm = GetClientFor(actionAffordance.Forms, "invokeaction", actionAffordance, options);
                Form form = clientAndForm.Form;
                IProtocolClient protocolClient = clientAndForm.ProtocolClient;
                // Handle UriVariables
                if (options.HasValue && options.Value.UriVariables != null) form = HandleUriVariables(form, options.Value.UriVariables);

                if (form == null) throw new SyntaxError($"Could not find a form that allows invoking action {actionName}");

                await protocolClient.InvokeResource(form, _contentManager.ValueToContent(parameters, form.ContentType, actionAffordance.Input));

                InteractionOutput output = new InteractionOutput(form);
                return output;
            }
        }

        public async Task<IInteractionOutput<T>> InvokeAction<T>(string actionName, InteractionOptions? options = null)
        {
            var actions = _td.Actions;
            if (!actions.TryGetValue(actionName, out var actionAffordance))
            {
                // 4. If interaction is undefined, reject promise with a NotFoundError and stop.
                throw new NotFoundError($"Action {actionName} not found in TD with ID {_td.Id}");
            }
            else
            {
                // find suitable form
                ClientAndForm clientAndForm = GetClientFor(actionAffordance.Forms, "invokeaction", actionAffordance, options);
                Form form = clientAndForm.Form;
                IProtocolClient protocolClient = clientAndForm.ProtocolClient;
                // Handle UriVariables
                if (options.HasValue && options.Value.UriVariables != null) form = HandleUriVariables(form, options.Value.UriVariables);

                if (form == null) throw new SyntaxError($"Could not find a form that allows invoking action {actionName}");
                Console.Write(form.Href);
                Content responseContent = await protocolClient.InvokeResource(form);
                InteractionOutput<T> output = HandleInteractionOutput<T>(responseContent, form, actionAffordance.Output);
                return output;
            }
        }

        public async Task<IInteractionOutput<T>> InvokeAction<T, U>(string actionName, U parameters, InteractionOptions? options = null)
        {
            var actions = _td.Actions;
            if (!actions.TryGetValue(actionName, out var actionAffordance))
            {
                // 4. If interaction is undefined, reject promise with a NotFoundError and stop.
                throw new NotFoundError($"Action {actionName} not found in TD with ID {_td.Id}");
            }
            else
            {
                // find suitable form
                ClientAndForm clientAndForm = GetClientFor(actionAffordance.Forms, "invokeaction", actionAffordance, options);
                Form form = clientAndForm.Form;
                IProtocolClient protocolClient = clientAndForm.ProtocolClient;
                // Handle UriVariables
                if (options.HasValue && options.Value.UriVariables != null) form = HandleUriVariables(form, options.Value.UriVariables);

                if (form == null) throw new SyntaxError($"Could not find a form that allows invoking action {actionName}");
                Content responseContent = await protocolClient.InvokeResource(form, _contentManager.ValueToContent(parameters, form.ContentType, actionAffordance.Input));
                InteractionOutput<T> output = HandleInteractionOutput<T>(responseContent, form, actionAffordance.Output);
                return output;
            }
        }
        #endregion
        #region event operations
        public async Task<ISubscription> SubscribeEvent(string eventName, Action listener, Action<Exception> onerror = null, InteractionOptions? options = null)
        {
            EventAffordance eventAffordance = _td.Events[eventName] ?? throw new NotFoundError($"ConsumedThing '{_td.Title}' (Id:'{_td.Id}') does not have event {eventName}");

            ClientAndForm clientAndForm = GetClientFor(eventAffordance.Forms, "subscribeevent", eventAffordance);

            if (clientAndForm.Form == null)
            {
                throw new SyntaxError($"ConsumedThing '{_td.Title}' did not find a suitable form for Event Affordance '{eventName}'");
            }

            if (clientAndForm.ProtocolClient == null)
            {
                throw new SyntaxError($"ConsumedThing '${_td.Title}' did not get suitable client for ${clientAndForm.Form.Href}");
            }

            if (_activeSubscriptions.ContainsKey(eventName))
            {
                throw new NotAllowedError($"ConsumedThing '${_td.Title}' has already a function subscribed to '{eventName}' .You can only subscribe once");
            }

            Form formWithoutUriTemplates = clientAndForm.Form;
            if (options.HasValue && options.Value.UriVariables != null)
            {
                formWithoutUriTemplates = HandleUriVariables(clientAndForm.Form, options.Value.UriVariables);
            }

            var internalSub = await clientAndForm.ProtocolClient.SubscribeResource(formWithoutUriTemplates, (content) =>
            {
                try
                {
                    listener();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while processing event for ${eventAffordance.Title}. ${e}");
                }
            },
            (err) =>
            {
                onerror?.Invoke(err);
            },
            () => { }
            );

            Subscription sub = new Subscription(Subscription.SubscriptionType.Event, eventName, eventAffordance, formWithoutUriTemplates, internalSub);
            AddSubscription(eventName, sub);
            return sub;
        }
        public async Task<ISubscription> SubscribeEvent<T>(string eventName, Action<IInteractionOutput<T>> listener, Action<Exception> onerror = null, InteractionOptions? options = null)
        {
            EventAffordance eventAffordance = _td.Events[eventName] ?? throw new NotFoundError($"ConsumedThing '{_td.Title}' (Id:'{_td.Id}') does not have event {eventName}");

            ClientAndForm clientAndForm = GetClientFor(eventAffordance.Forms, "subscribeevent", eventAffordance);

            if (clientAndForm.Form == null)
            {
                throw new SyntaxError($"ConsumedThing '{_td.Title}' did not find a suitable form for Event Affordance '{eventName}'");
            }

            if (clientAndForm.ProtocolClient == null)
            {
                throw new SyntaxError($"ConsumedThing '${_td.Title}' did not get suitable client for ${clientAndForm.Form.Href}");
            }

            if (_activeSubscriptions.ContainsKey(eventName))
            {
                throw new NotAllowedError($"ConsumedThing '${_td.Title}' has already a function subscribed to '{eventName}' .You can only subscribe once");
            }

            Form formWithoutUriTemplates = clientAndForm.Form;
            if (options.HasValue && options.Value.UriVariables != null)
            {
                formWithoutUriTemplates = HandleUriVariables(clientAndForm.Form, options.Value.UriVariables);
            }

            var internalSub = await clientAndForm.ProtocolClient.SubscribeResource(formWithoutUriTemplates, (content) =>
            {
                try
                {
                    listener(HandleInteractionOutput<T>(content, formWithoutUriTemplates, eventAffordance.Data));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while processing event for ${eventAffordance.Title}. ${e}");
                }
            },
            (err) =>
            {
                onerror?.Invoke(err);
            },
            () => { }
            );

            Subscription sub = new Subscription(Subscription.SubscriptionType.Event, eventName, eventAffordance, formWithoutUriTemplates, internalSub);
            AddSubscription(eventName, sub);
            return sub;
        }
        public async Task UnsubscribeEvent(string eventName, InteractionOptions? options)
        {
            bool isFound = _td.Events.TryGetValue(eventName, out EventAffordance eventAffordance);
            if (!isFound)
            {
                throw new NotFoundError($"ConsumedThing '{_td.Title}' does not have event {eventName}");
            }

            isFound = _activeSubscriptions.TryGetValue(eventName, out ISubscription subI);

            if (!isFound)
            {
                throw new OperationError($"ConsumedThing '{_td.Title}' is not listening to {eventName}");
            }

            Subscription sub = (Subscription)subI;

            uint formIndex = (uint)sub.FindBestMatchingUnlinkFormIndex();
            if (!options.HasValue)
            {
                options = new InteractionOptions() { FormIndex = formIndex };
            }
            else
            {
                options = new InteractionOptions()
                {
                    FormIndex = formIndex,
                    Data = options.Value.Data,
                    UriVariables = options.Value.UriVariables
                };
            }

            var result = GetClientFor(eventAffordance.Forms, "unsubscribeevent", eventAffordance);

            if (result.Form == null)
            {
                throw new SyntaxError($"ConsumedThing '{_td.Title}' did not get suitable form for unsubscribeevent {eventName}");
            }

            Form formWithoutUriTemplates = result.Form;
            if (options.HasValue && options.Value.UriVariables != null)
            {
                formWithoutUriTemplates = HandleUriVariables(result.Form, options.Value.UriVariables);
            }

            await result.ProtocolClient.UnlinkResource(formWithoutUriTemplates);
            await sub.Stop();
            RemoveSubscription(eventName);
        }

        #endregion
        public ThingDescription GetThingDescription()
        {
            return _td;
        }



        protected Form HandleUriVariables(Form form, Dictionary<string, object> uriVariables)
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

        protected InteractionOutput<T> HandleInteractionOutput<T>(Content content, Form form, IDataSchema outputDataSchema = null, bool ignoreValidation = false)
        {
            content.type = content.type ?? form.ContentType ?? "application/json";
            if (form.Response != null)
            {
                ContentType parsedMediaTypeContent = new ContentType(content.type);
                ContentType parsedMediaTypeForm = new ContentType(form.Response?.ContentType);

                if (parsedMediaTypeContent != parsedMediaTypeForm)
                {
                    throw new Exception($"Unexpected type '${content.type}' in response. Should be '${form.Response?.ContentType}");
                }
            }
            return new InteractionOutput<T>(content, form, outputDataSchema, ignoreValidation);
        }

        protected Form FindForm(Form[] forms, string op, InteractionAffordance affordance, string[] schemes, uint idx)
        {
            Form form = null;

            foreach (Form f in forms)
            {
                List<string> fop = new List<string>();
                if (f.Op != null)
                {
                    fop.AddRange(f.Op);
                }
                else
                {
                    if (affordance is PropertyAffordance)
                    {
                        fop.Add("readproperty");
                        fop.Add("writeproperty");
                        break;

                    }
                    else if (affordance is ActionAffordance)
                    {
                        fop.Add("invokeaction");
                    }
                    else if (affordance is EventAffordance)
                    {
                        fop.Add("subscribeevent");
                    }
                }

                if (fop != null && fop.Contains(op) && f.Href.OriginalString.StartsWith($"{schemes[idx]}:"))
                {
                    form = f;
                    break;
                }
            }

            return form;
        }

        protected List<SecurityScheme> GetSecuritySchemes(string[] securities)
        {
            List<SecurityScheme> schemes = new List<SecurityScheme>();
            foreach (string secur in securities)
            {
                bool isSuccessful = _td.SecurityDefinitions.TryGetValue(secur + "", out SecurityScheme ws);
                if (isSuccessful)
                {
                    schemes.Add(ws);
                }
            }
            return schemes;
        }

        protected void EnsureClientSecurity(IProtocolClient client, Form form = null)
        {
            if (_td.SecurityDefinitions != null)
            {

                if (form != null && form.Security != null && form.Security?.Length > 0)
                {
                    client.SetSecurity(GetSecuritySchemes(form.Security).ToArray(), _consumer.GetCredentials(_td.Id));
                }
                else if (_td.Security != null && _td.Security.Length > 0)
                {
                    client.SetSecurity(GetSecuritySchemes(_td.Security).ToArray(), _consumer.GetCredentials(_td.Id));
                }
            }
        }

        protected ClientAndForm GetClientFor(Form[] forms, string op, InteractionAffordance affordance)
        {
            if (forms.Length == 0)
            {
                throw new SyntaxError($"ConsumedThing '${_td.Title}' has no links for interaction '${affordance.Title}'");
            }

            Form form = null;
            IProtocolClient client = null;

            List<Form> formsList = new List<Form>(forms);
            List<string> schemes = formsList.Select((f) => f.Href.Scheme).ToList();
            int cacheIdx = schemes.FindIndex((scheme) => _clients.ContainsKey(scheme));

            if (cacheIdx >= 0)
            {
                client = _clients[schemes[cacheIdx]];
                form = FindForm(forms, op, affordance, schemes.ToArray(), (uint)cacheIdx);
            }
            else
            {
                int srvIdx = schemes.FindIndex((scheme) => _consumer.HasClientFactoryFor(scheme));

                if (srvIdx < 0)
                {
                    throw new NotFoundError($"ConsumedThing '{_td.Title}' missing ClientFactory for '${schemes}'");
                }

                client = _consumer.BuildClientFor(schemes[srvIdx]);

                _clients.Add(schemes[srvIdx], client);
                form = FindForm(forms, op, affordance, schemes.ToArray(), (uint)srvIdx);

                EnsureClientSecurity(client, form);
            }

            return new ClientAndForm(client, form);
        }

        protected ClientAndForm GetClientFor(Form[] forms, string op, InteractionAffordance affordance, InteractionOptions? options = null)
        {
            if (forms.Length == 0)
            {
                throw new SyntaxError($"ConsumedThing '${_td.Title}' has no links for interaction '${affordance.Title}'");
            }

            Form form;
            IProtocolClient client;

            if (options?.FormIndex != null)
            {
                if (options?.FormIndex >= 0 && options?.FormIndex < forms.Length)
                {
                    form = forms[(int)options?.FormIndex];
                    string scheme = form.Href.Scheme;

                    if (_consumer.HasClientFactoryFor(scheme))
                    {
                        client = _consumer.BuildClientFor(scheme);

                        if (!_clients.ContainsKey(scheme))
                        {
                            _clients.Add(scheme, client);
                        }
                    }
                    else
                    {
                        throw new NotFoundError($"ConsumedThing '{_td.Title}' (id:{_td.Id}) missing ClientFactory for '${scheme}'");
                    }
                }
                else
                {
                    throw new SyntaxError($"ConsumedThing '{_td.Title}' (id:{_td.Id}) missing formIndex '{options?.FormIndex}'");
                }

                return new ClientAndForm(client, form);
            }
            else
            {
                return GetClientFor(forms, op, affordance);
            }
        }

        protected void AddObservation(string name, Subscription sub)
        {
            _activeObservations.Add(name, sub);
        }

        protected void AddSubscription(string name, Subscription sub)
        {
            _activeSubscriptions.Add(name, sub);
        }

        protected void RemoveObservation(string name)
        {
            _activeObservations.Remove(name);
        }

        protected void RemoveSubscription(string name)
        {
            _activeSubscriptions.Remove(name);
        }

    }


}