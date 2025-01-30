using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WoT.Core.Definitions;
using WoT.Core.Definitions.TD;

namespace WoT.Core.Implementation
{
    public class Consumer : IConsumer, IRequester
    {
        /// <summary>
        /// A simple WoT Consumer that according to the thing
        /// </summary>
        /// 
        private readonly Dictionary<string, IProtocolClientFactory> _clientFactories;
        private readonly Dictionary<string, Dictionary<CredentialScheme, object>> _credentialStore;
        private IConsumedThing _consumedThing;

        /// <summary>
        /// Constructor for the Consumer
        /// </summary>
        public Consumer()
        {
            _clientFactories = new Dictionary<string, IProtocolClientFactory>();
            _credentialStore = new Dictionary<string, Dictionary<CredentialScheme, object>>();
        }

        /// <summary>
        /// Consumes a Thing Description
        /// </summary>
        /// <param name="td">Thing Description to be consumed</param>
        /// <returns>An object instance implementing <see cref="IConsumedThing"/></returns>
        public IConsumedThing Consume(ThingDescription td)
        {
            ConsumedThing consumedThing = new ConsumedThing(td, this);
            _consumedThing = consumedThing;
            return _consumedThing;
        }

        /// <summary>
        /// Requests a Thing Description from a given URI
        /// </summary>
        /// <param name="uri">URI as string</param>
        /// <returns>A <see cref="ThingDescription"/> instance</returns>
        public Task<ThingDescription> RequestThingDescription(string uri)
        {
            Uri tdUri = new Uri(uri);
            return RequestThingDescription(tdUri);
        }

        /// <summary>
        /// Requests a Thing Description from a given URI
        /// </summary>
        /// <param name="uri">Resource URI</param>
        /// <returns>A <see cref="ThingDescription"/> instance</returns>
        public async Task<ThingDescription> RequestThingDescription(Uri uri)
        {

            Console.WriteLine($"Info: Fetching TD from {uri.OriginalString}");
            Content responseContent = await BuildClientFor(uri).RequestThingDescription(uri);
            Console.WriteLine($"Info: Fetched TD from {uri.OriginalString} successfully");
            Console.WriteLine($"Info: Parsing TD");
            ThingDescription td = ContentSerdes.GetInstance().ContentToValue<ThingDescription>(new ReadContent(responseContent.type, await responseContent.ToBuffer()), null);
            Console.WriteLine($"Info: Parsed TD successfully");
            return td;

        }

        /// <summary>
        /// Adds a client factory to the Consumer
        /// </summary>
        /// <param name="clientFactory">The client factory to add</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddClientFactory(IProtocolClientFactory clientFactory)
        {
            if (clientFactory == null)
            {
                throw new ArgumentNullException("Input parameter cannot be null");
            }

            if (!_clientFactories.ContainsKey(clientFactory.Scheme))
            {
                _clientFactories.Add(clientFactory.Scheme, clientFactory);
            }

        }

        /// <summary>
        /// Removes a client factory from the Consumer
        /// </summary>
        /// <param name="scheme">The scheme of the client factory</param>
        /// <returns><see langword="true"/> if removing the client factory was successful, otherwise <see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool RemoveClientFactory(string scheme)
        {
            return _clientFactories.Remove(scheme);
        }

        /// <summary>
        /// Builds a client for a given protocol scheme
        /// </summary>
        /// <param name="scheme">protocol scheme</param>
        /// <returns>An instance of an object implementing <see cref="IProtocolClient"/></returns>
        public IProtocolClient BuildClientFor(string scheme)
        {
            IProtocolClientFactory clientFactory = _clientFactories[scheme];
            return clientFactory.GetClient();
        }

        /// <summary>
        /// Builds a client for a given URI
        /// </summary>
        /// <param name="href">Absolute URI with scheme</param>
        /// <returns>An instance of an object implementing <see cref="IProtocolClient"/></returns>
        public IProtocolClient BuildClientFor(Uri href)
        {
            IProtocolClientFactory clientFactory = _clientFactories[href.Scheme];
            return clientFactory.GetClient();
        }

        /// <summary>
        /// Checks if the Consumer has a client factory for a given protocol scheme
        /// </summary>
        /// <param name="scheme">protocol scheme</param>
        /// <returns><see langword="true"/> if Consumer has a client factory for the scheme, otherwise <see langword="false"/></returns>
        public bool HasClientFactoryFor(string scheme)
        {
            return _clientFactories.ContainsKey(scheme);
        }

        /// <summary>
        /// Gets the client factory schemes supported by the Consumer
        /// </summary>
        /// <returns>an array with all protocol schemes supported by the Consumer</returns>
        public string[] GetClientFactorySchemes()
        {
            return _clientFactories.Keys.ToArray();
        }

        /// <summary>
        /// Adds credentials to the Consumer
        /// </summary>
        /// <param name="cd">an object describing the credentials</param>
        public void AddCredentials(Dictionary<string, CredentialDescription> cd)
        {
            foreach (var item in cd)
            {
                string clientId = item.Key;
                var credential = item.Value;

                bool isFound = _credentialStore.TryGetValue(clientId, out var currentSecrets);

                if (isFound)
                {
                    currentSecrets.Add(credential.Scheme, credential.Credential);
                }
                else
                {
                    var clientCredentials = new Dictionary<CredentialScheme, object>
                    {
                        [credential.Scheme] = credential.Credential
                    };

                    _credentialStore.Add(clientId, clientCredentials);
                }

            }
        }

        /// <summary>
        /// Gets the credentials for a given ConsumedThing ID
        /// </summary>
        /// <param name="id">ID of a ConsumedThing</param>
        /// <returns>Dictionary mapping a credential scheme to all credentials applying to that scheme</returns>
        public Dictionary<CredentialScheme, object> GetCredentials(string id)
        {
            bool hasCredentials = _credentialStore.TryGetValue(id, out var clientCredentials);
            if (hasCredentials)
            {
                return clientCredentials;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Starts the Consumer by initalizing all client factories.
        /// <remarks>Currently does not do anything</remarks>
        /// </summary>
        public void Start()
        {
            foreach (var item in _clientFactories)
            {
                item.Value.Init();
            }
        }
    }
}