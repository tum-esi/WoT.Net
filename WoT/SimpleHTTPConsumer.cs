using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tavis.UriTemplates;
using WoT.Definitions;
using WoT.Errors;
using System.Runtime.CompilerServices;

namespace WoT.Implementation
{
    public class SimpleHTTPConsumer : IConsumer
    {
        public readonly HttpClient httpClient;
        private readonly JsonSerializer _serializer;
        public SimpleHTTPConsumer()
        {
            httpClient = new HttpClient();
            _serializer = new JsonSerializer();
        }
        public async Task<IConsumedThing> Consume(ThingDescription td)
        {
            Task<SimpleConsumedThing> task = Task.Run(() =>
            {
                return new SimpleConsumedThing(td, this);
            });
            return await task;
        }

        public async Task<ThingDescription> RequestThingDescription(string url)
        {
            Uri tdUrl = new Uri(url);
            if (tdUrl.Scheme != "http") throw new Exception($"The protocol for accessing the TD url {url} is not HTTP");
            Console.WriteLine($"Info: Fetching TD from {url}");
            HttpResponseMessage tdResponse = await httpClient.GetAsync(tdUrl);
            tdResponse.EnsureSuccessStatusCode();
            Console.WriteLine($"Info: Fetched TD from {url} successfully");
            Console.WriteLine($"Info: Parsing TD");
            HttpContent body = tdResponse.Content;
            string tdData = await body.ReadAsStringAsync();
            TextReader reader = new StringReader(tdData);
            ThingDescription td = _serializer.Deserialize(reader, typeof(ThingDescription)) as ThingDescription;
            Console.WriteLine($"Info: Parsed TD successfully");
            return td;
        }
    }

    public class SimpleConsumedThing : IConsumedThing
    {
        private readonly ThingDescription _td;
        private readonly SimpleHTTPConsumer _consumer;
        public SimpleConsumedThing(ThingDescription td, SimpleHTTPConsumer consumer)
        {
            _td = td;
            _consumer = consumer;
        }
        public async Task<IInteractionOutput> InvokeAction(string actionName, InteractionOptions? options = null)
        {
            var actions = this._td.Actions;
            Form form;
            if (!actions.TryGetValue(actionName, out var actionAffordance))
            {
                // 4. If interaction is undefined, reject promise with a NotFoundError and stop.
                throw new Exception($"Property {actionName} not found in TD with ID {_td.Id}");
            }
            else
            {
                // find suitable form
                form = FindSuitableForm(actionAffordance.Forms, null, "http", "application/json", options);
                // Handle UriVariables
                if (options.HasValue && options.Value.uriVariables != null) form = HandleUriVariables(form, options.Value.uriVariables);

                if (form == null) throw new Exception($"Could not find a form that allows reading property {actionName}");
                Console.Write(form.Href);
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, form.Href);
                var interactionResponse = await _consumer.httpClient.SendAsync(message);
                interactionResponse.EnsureSuccessStatusCode();
                InteractionOutput output = new InteractionOutput(form);
                return output;
            }
        }

        public async Task<IInteractionOutput> InvokeAction<U>(string actionName, U parameters, InteractionOptions? options = null)
        {
            var actions = this._td.Actions;
            Form form;
            if (!actions.TryGetValue(actionName, out var actionAffordance))
            {
                // 4. If interaction is undefined, reject promise with a NotFoundError and stop.
                throw new Exception($"Property {actionName} not found in TD with ID {_td.Id}");
            }
            else
            {
                // find suitable form
                form = FindSuitableForm(actionAffordance.Forms, null, "http", "application/json", options);
                // Handle UriVariables
                if (options.HasValue && options.Value.uriVariables != null) form = HandleUriVariables(form, options.Value.uriVariables);

                if (form == null) throw new Exception($"Could not find a form that allows reading property {actionName}");
                string payloadString = JsonConvert.SerializeObject(parameters);
                StringContent payload = new StringContent(payloadString, Encoding.UTF8, "application/json");
                HttpResponseMessage interactionResponse = await _consumer.httpClient.PostAsync(form.Href, payload);
                interactionResponse.EnsureSuccessStatusCode();
                InteractionOutput output = new InteractionOutput(form);
                return output;
            }
        }
        public async Task<IInteractionOutput<T>> InvokeAction<T>(string actionName, InteractionOptions? options = null)
        {
            var actions = this._td.Actions;
            Form form;
            if (!actions.TryGetValue(actionName, out var actionAffordance))
            {
                // 4. If interaction is undefined, reject promise with a NotFoundError and stop.
                throw new Exception($"Property {actionName} not found in TD with ID {_td.Id}");
            }
            else
            {
                // find suitable form
                form = FindSuitableForm(actionAffordance.Forms, null, "http", "application/json", options);
                // Handle UriVariables
                if (options.HasValue && options.Value.uriVariables != null) form = HandleUriVariables(form, options.Value.uriVariables);

                if (form == null) throw new Exception($"Could not find a form that allows reading property {actionName}");
                Console.Write(form.Href);
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, form.Href);
                HttpResponseMessage interactionResponse = await _consumer.httpClient.SendAsync(message);
                interactionResponse.EnsureSuccessStatusCode();
                Stream responseStream = await interactionResponse.Content.ReadAsStreamAsync();
                InteractionOutput<T> output = new InteractionOutput<T>(actionAffordance.Output, form, responseStream);
                return output;
            }
        }

        public async Task<IInteractionOutput<T>> InvokeAction<T,U>(string actionName, U parameters, InteractionOptions? options = null)
        {
            var actions = this._td.Actions;
            Form form;
            if (!actions.TryGetValue(actionName, out var actionAffordance))
            {
                // 4. If interaction is undefined, reject promise with a NotFoundError and stop.
                throw new Exception($"Property {actionName} not found in TD with ID {_td.Id}");
            }
            else
            {
                // find suitable form
                form = FindSuitableForm(actionAffordance.Forms, null, "http", "application/json", options);
                // Handle UriVariables
                if (options.HasValue && options.Value.uriVariables != null) form = HandleUriVariables(form, options.Value.uriVariables);

                if (form == null) throw new Exception($"Could not find a form that allows reading property {actionName}");
                string payloadString = JsonConvert.SerializeObject(parameters);
                StringContent payload = new StringContent(payloadString, Encoding.UTF8, "application/json");
                HttpResponseMessage interactionResponse = await _consumer.httpClient.PostAsync(form.Href, payload);
                interactionResponse.EnsureSuccessStatusCode();
                Stream responseStream = await interactionResponse.Content.ReadAsStreamAsync();
                InteractionOutput<T> output = new InteractionOutput<T>(actionAffordance.Output, form, responseStream);
                return output;
            }
        }

        public Task<ISubscription> ObserveProperty<T>(string propertyName, Action<T> listener, InteractionOptions? options = null)
        {
            throw new NotImplementedException();
        }

        public Task<ISubscription> ObserveProperty<T>(string propertyName, Action<T> listener, Action<Exception> onerror, InteractionOptions? options = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IInteractionOutput<T>> ReadProperty<T>(string propertyName, InteractionOptions? options = null)
        {
            var properties = this._td.Properties;
            Form form;
            // 3. Let interaction be [[td]].properties.propertyName.
            if (!properties.TryGetValue(propertyName, out var propertyAffordance))
            {
                // 4. If interaction is undefined, reject promise with a NotFoundError and stop.
                throw new Exception($"Property {propertyName} not found in TD with ID {_td.Id}");
            }
            else
            {
                if (propertyAffordance.WriteOnly == true) throw new Exception($"Cannot read writeOnly property {propertyName}");
                // find suitable form
                form = FindSuitableForm(propertyAffordance.Forms, "readproperty", "http", "application/json", options);
                // Handle UriVariables
                if (options.HasValue && options.Value.uriVariables != null) form = HandleUriVariables(form, options.Value.uriVariables);

                if (form == null) throw new Exception($"Could not find a form that allows reading property {propertyName}");
                HttpResponseMessage interactionResponse = await _consumer.httpClient.GetAsync(form.Href);
                interactionResponse.EnsureSuccessStatusCode();
                Stream responseStream = await interactionResponse.Content.ReadAsStreamAsync();
                InteractionOutput <T> output = new InteractionOutput<T>(propertyAffordance, form, responseStream);
                return output;
            }
        }

        public Task<ISubscription> SubscribeEvent<T>(string eventName, Action<T> listener, InteractionOptions? options = null)
        {
            throw new NotImplementedException();
        }

        public Task<ISubscription> SubscribeEvent<T>(string eventName, Action<T> listener, Action<Exception> onerror, InteractionOptions? options = null)
        {
            throw new NotImplementedException();
        }

        public async Task WriteProperty<T>(string propertyName, T value, InteractionOptions? options)
        {
            var properties = this._td.Properties;
            Form form;
            // 3. Let interaction be [[td]].properties.propertyName.
            if (!properties.TryGetValue(propertyName, out var propertyAffordance))
            {
                // 4. If interaction is undefined, reject promise with a NotFoundError and stop.
                throw new Exception($"Property {propertyName} not found in TD with ID {_td.Id}");
            }
            else
            {
                if (propertyAffordance.ReadOnly == true) throw new Exception($"Cannot read writeOnly property {propertyName}");
                // find suitable form
                form = FindSuitableForm(propertyAffordance.Forms, "readproperty", "http", "application/json", options);
                // Handle UriVariables
                if (options.HasValue && options.Value.uriVariables != null) form = HandleUriVariables(form, options.Value.uriVariables);

                if (form == null) throw new Exception($"Could not find a form that allows reading property {propertyName}");
                string payloadString = JsonConvert.SerializeObject(value);
                var payload = new StringContent(payloadString, Encoding.UTF8, "application/json");
                var ms = new HttpRequestMessage(HttpMethod.Put, form.Href);
                ms.Content = payload;
                HttpResponseMessage interactionResponse = await _consumer.httpClient.SendAsync(ms);
                interactionResponse.EnsureSuccessStatusCode();
            }
        }

        public ThingDescription GetThingDescription() { return _td; }

        public void AddCredential(string id, string password)
        {

        }

        protected Form HandleUriVariables(Form form, Dictionary<string, object> uriVariavles)
        {
            var urlTemplate = new UriTemplate(form.Href.OriginalString);
            foreach (var variable in uriVariavles)
            {
                urlTemplate.AddParameter(variable.Key, variable.Value);
            }
            string extendedUrl = urlTemplate.Resolve();
            form.Href = new Uri(extendedUrl);
            Console.WriteLine(form.Href.AbsoluteUri);
            return form;
        }

        protected Form FindSuitableForm(Form[] forms, string op, string scheme, string contentType, InteractionOptions? options = null)
        {
            Form[] filteredForms = forms;
            Form form = null;
            if (options != null)
            {
                if (options.Value.formIndex != null)
                {
                    uint index = options.Value.formIndex.Value;
                    if (index >= 0 && index < forms.Length) form = forms[index];
                }
            }

            if (op != null) { filteredForms = filteredForms.Where((f) => f.Op.Contains(op)).ToArray(); }
            if (scheme != null) { filteredForms = filteredForms.Where((f) => f.Href.Scheme == scheme).ToArray(); }
            if (contentType != null) { filteredForms = filteredForms.Where((f) => f.ContentType == contentType).ToArray(); }

            if (filteredForms.Length > 0) form = filteredForms[0];
            return form;
        }

        
    }

    public class InteractionOutput : IInteractionOutput
    {
        private readonly Form _form;
        public InteractionOutput(Form form) 
        { 
            _form = form;
        }
        public Stream Data => null;

        public bool DataUsed => true;

        public Form Form => _form;

        public IDataSchema Schema => null;

        public Task<byte[]> ArrayBuffer()
        {
            return null;
        }
        public Task Value()
        {
            return null;
        }
    }
    public class InteractionOutput<T> : IInteractionOutput<T>
    {
        private readonly Form _form;
        private readonly Stream _data;
        private bool _dataUsed;
        private readonly T _value;
        private bool _isValueSet;
        private readonly IDataSchema _schema;
        private readonly JSchema _parsedSchema;
        private readonly JsonSerializer _serializer;
        public InteractionOutput(DataSchema schema, Form form, Stream data) {
            _form = form;
            _data = data;
            _dataUsed = false;
            //_content = content;
            _schema = schema;
            string schemaString = JsonConvert.SerializeObject(schema, settings: new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            _parsedSchema = JSchema.Parse(schemaString);
            _serializer = new JsonSerializer();

        }
        public InteractionOutput(DataSchema schema, Form form, Stream data, T value)
        {
            _form = form;
            _data = data;
            _dataUsed = false;
            _value = value;
            _isValueSet = true;
            _schema = schema;
            string schemaString = JsonConvert.SerializeObject(schema, settings: new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            _parsedSchema = JSchema.Parse(schemaString);
            _serializer = new JsonSerializer();

        }
        public InteractionOutput(PropertyAffordance schema, Form form, Stream data)
        {
            _form = form;
            _data = data;
            //_content = content;
            _schema = schema;
            _parsedSchema = JSchema.Parse(schema.OriginalJson);
            _serializer = new JsonSerializer();

        }
        public InteractionOutput(PropertyAffordance schema, Form form, Stream data, T value)
        {
            _form = form;
            _data = data;
            _value = value;
            _isValueSet = true;
            _schema = schema;
            _parsedSchema = JSchema.Parse(schema.OriginalJson);
            _serializer = new JsonSerializer();

        }
        public Stream Data => _data;

        public bool DataUsed => _dataUsed;

        public Form Form => _form;

        public IDataSchema Schema => _schema;

        public async Task<byte[]> ArrayBuffer()
        {
             Task<byte[]> task = Task.Run(() =>
            {
                if (!_data.CanRead || _dataUsed)
                {
                    throw new NotReadableError();
                }
                MemoryStream ms = new MemoryStream();
                _data.CopyTo(ms);
                _dataUsed = true;
                byte[] arrayBuffer = ms.ToArray();
                return arrayBuffer;
            });
            return await task;
        }

        public async Task<T> Value()
        {
            Task<T> task = Task.Run(() =>
            {
                if (!_data.CanRead || _dataUsed)
                {
                    throw new NotReadableError();
                }
                StreamReader sr = new StreamReader(_data, Encoding.UTF8);
                string valueJson = sr.ReadToEnd();
                _dataUsed = true;
                // Intialize validating schema
                JsonTextReader reader = new JsonTextReader(new StringReader(valueJson));
                JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(reader)
                {
                    Schema = _parsedSchema
                };
                //Add Error listener
                IList<string> messages = new List<string>();
                validatingReader.ValidationEventHandler += (o, a) => messages.Add(a.Message);
                //Deserialize
                T value = _serializer.Deserialize<T>(validatingReader);

                bool isValid = (messages.Count == 0);
                if (isValid)
                {
                    return value;
                }
                else
                {
                    throw new Exception("Schema Validation failed for value of readProperty");
                }
            });
            return await task;
        }
    }
}
