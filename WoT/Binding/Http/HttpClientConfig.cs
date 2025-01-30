namespace WoT.Binding.Http
{
    /// <summary>
    /// Configuration for the HttpClient
    /// </summary>
    public class HttpClientConfig
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public HttpClientConfig() { }

        /// <summary>
        /// Proxy configuration
        /// </summary>
        public HttpProxyConfig Proxy { get; set; }

        /// <summary>
        /// Allow self signed certificates
        /// </summary>
        public bool AllowSelfSigned { get; set; }

    }
}
