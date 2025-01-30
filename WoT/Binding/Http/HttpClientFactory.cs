using WoT.Core.Definitions;

namespace WoT.Binding.Http
{
    /// <summary>
    /// Factory for creating HttpClient instances
    /// </summary>
    public class HttpClientFactory : IProtocolClientFactory
    {
        private readonly string _scheme = "http";
        private readonly HttpClientConfig _config;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        public HttpClientFactory(HttpClientConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// Scheme of the HttpClient instance
        /// </summary>
        public string Scheme => _scheme;

        /// <summary>
        /// Get a new HttpClient instance
        /// </summary>
        /// <returns></returns>
        public IProtocolClient GetClient()
        {
            return new HttpClient(_config);
        }

        /// <summary>
        /// Initialize the HttpClientFactory
        /// </summary>
        /// <remarks>Currently does not do anything </remarks>
        /// <returns><see langword="true"/> if initialization was successful, <see langword="false"/> otherwise</returns>
        public bool Init()
        {
            // info(`HttpClientFactory for '${HttpClientFactory.scheme}' initializing`);
            // TODO uncomment info if something is executed here
            return true;
        }

        /// <summary>
        /// Destroy the HttpClientFactory
        /// </summary>
        /// <returns><see langword="true"/> if factory was destroyed successfully, <see langword="false"/> otherwise</returns>
        public bool Destroy()
        {
            // info(`HttpClientFactory for '${HttpClientFactory.scheme}' destroyed`);
            // TODO uncomment info if something is executed here
            return true;
        }
    }
}
