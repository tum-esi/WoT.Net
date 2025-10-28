using WoT.Core.Definitions;

namespace WoT.Binding.CoAP
{
    /// <summary>
    /// Factory for creating CoapClient instances
    /// </summary>
    public class CoapClientFactory : IProtocolClientFactory
    {
        private readonly string _scheme = "coap";
        private readonly CoapClientConfig _config;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        public CoapClientFactory(CoapClientConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// Scheme of the CoapClient instance
        /// </summary>
        public string Scheme => _scheme;

        /// <summary>
        /// Get a new WotCoapClient instance
        /// </summary>
        /// <returns></returns>
        public IProtocolClient GetClient()
        {
            return new WotCoapClient(_config);
        }

        /// <summary>
        /// Initialize the CoapClientFactory
        /// </summary>
        /// <returns><see langword="true"/> if initialization was successful, <see langword="false"/> otherwise</returns>
        public bool Init()
        {
            return true;
        }

        /// <summary>
        /// Destroy the CoapClientFactory
        /// </summary>
        /// <returns><see langword="true"/> if factory was destroyed successfully, <see langword="false"/> otherwise</returns>
        public bool Destroy()
        {
            return true;
        }
    }
}
