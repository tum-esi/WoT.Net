using WoT.Core.Definitions;

namespace WoT.Binding.Http
{
    /// <summary>
    /// Factory for creating HTTPS Client instances
    /// </summary>
    public class HttpsClientFactory : IProtocolClientFactory
    {
        private readonly string _scheme = "https";
        private readonly HttpClientConfig _config;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">client configuration</param>
        public HttpsClientFactory(HttpClientConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// Scheme of the client instance
        /// </summary>
        public string Scheme => _scheme;


        /// <summary>
        /// Get a new secured HttpClient instance
        /// </summary>
        /// <returns></returns>
        public IProtocolClient GetClient()
        {
            if (_config != null && _config.Proxy != null && _config.Proxy.Href != null && _config.Proxy.Href.StartsWith("http:"))
            {
                return new HttpClient(_config);
            }
            else
            {
                return new HttpClient(_config, true);
            }
        }

        /// <summary>
        /// Initialize the HttpsClientFactory
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            // info(`HttpClientFactory for '${HttpClientFactory.scheme}' initializing`);
            // TODO uncomment info if something is executed here
            return true;
        }

        /// <summary>
        /// Destroy the HttpsClientFactory
        /// </summary>
        /// <returns></returns>
        public bool Destroy()
        {
            // info(`HttpClientFactory for '${HttpClientFactory.scheme}' destroyed`);
            // TODO uncomment info if something is executed here
            return true;
        }
    }
}
