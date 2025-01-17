using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WoT.Definitions;
using WoT.Errors;
using WoT.ProtocolBindings;

namespace WoT.Implementation
{
    public class Consumer : IConsumer, IRequester
    {
        /// <summary>
        /// A simple WoT Consumer that according to the thing
        /// </summary>
        /// 
        private Dictionary<string, IProtocolClient> _clients;
        private IConsumedThing _consumedThing;

        public Consumer() 
        {
            _clients = new Dictionary<string, IProtocolClient>();
        }
        public IConsumedThing Consume(ThingDescription td)
        {
            ConsumedThing consumedThing = new ConsumedThing(td, this);
            _consumedThing = consumedThing;
            return _consumedThing;
        }


        

        public Task<ThingDescription> RequestThingDescription(string url)
        {
            Uri tdURL = new Uri(url);
            return RequestThingDescription(tdURL);
        }

        public async Task<ThingDescription> RequestThingDescription(Uri tdUrl)
        {
           
               Console.WriteLine($"Info: Fetching TD from {tdUrl.OriginalString}");
               Form tdForm = new Form();
               tdForm.Href = tdUrl;
               Stream responseStream = await GetClientFor(tdUrl).SendGetRequest(tdForm);
               Console.WriteLine($"Info: Fetched TD from {tdUrl.OriginalString} successfully");
               Console.WriteLine($"Info: Parsing TD");
               StreamReader streamReader = new StreamReader(responseStream);
               JsonTextReader jsonReader = new JsonTextReader(streamReader);
               JsonSerializer serializer = new JsonSerializer();
               ThingDescription td = serializer.Deserialize<ThingDescription>(jsonReader);
               Console.WriteLine($"Info: Parsed TD successfully");
               return td;
           
        }



        public void AddClient(IProtocolClient protocolClient)
        {
            _clients.Add(protocolClient.Scheme, protocolClient);
        }

        public void RemoveClient(string scheme)
        { 
            _clients.Remove(scheme); 
        }
        public ClientAndForm GetClientFor(Form[] forms, string op, InteractionOptions? options = null, string contentType = "application/json", string subprotocol = "null")
        {
            Form form = null;
            IProtocolClient protocolClient = null;

            if (options != null && options.Value.formIndex.HasValue)
            {
                uint formIndex = options.Value.formIndex.Value;
                if (formIndex < forms.Length) form = forms[formIndex];
                if ((op != null && !form.Op.Contains(op)) ||
                (contentType != null && form.ContentType != contentType) ||
                (subprotocol != "null" && form.Subprotocol != subprotocol))
                    throw new NotFoundError($"Form at index {formIndex} does not support the given specifications.");
                else if (!this._clients.TryGetValue(form.Href.Scheme, out var pc))
                {
                    throw new NotFoundError($"No Protocol Client for Form with href \"{form.Href}\".");
                }
                else
                {
                    protocolClient = pc;
                }
            }
            else
            {
                Form[] filteredForms = forms;
                if (op != null) { filteredForms = filteredForms.Where((f) => f.Op.Contains(op)).ToArray(); }
                if (contentType != null) { filteredForms = filteredForms.Where((f) => f.ContentType == contentType).ToArray(); }
                if (subprotocol != "null") { 
                    filteredForms = filteredForms.Where((f) => f.Subprotocol == subprotocol).ToArray(); 
                }
                if (filteredForms.Length == 0)
                {
                    throw new NotFoundError($"No suitable form found for the given specifications.");
                }

                bool foundMatchingScheme = false;
                foreach (Form f in filteredForms)
                {
                    if (this._clients.TryGetValue(f.Href.Scheme, out var pc))
                    {
                        form = f;
                        protocolClient = pc;
                        foundMatchingScheme = true;
                        break;
                    }
                }
                if (!foundMatchingScheme)
                {
                    throw new NotFoundError($"No Protocol Client found for the given input forms.");
                }
            }

            return new ClientAndForm(protocolClient, form);
        }
        
        public IProtocolClient GetClientFor(Uri href)
        {
            if (!this._clients.TryGetValue(href.Scheme, out var pc))
            {
                throw new NotFoundError($"No Protocol Client for href \"{href.OriginalString}\".");
            }
            else
            {
                return pc;
            }
        }
        

    }
}