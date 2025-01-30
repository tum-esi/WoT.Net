namespace WoT.Binding.Http
{
    /// <summary>
    /// HTTP Proxy configuration
    /// </summary>
    public class HttpProxyConfig
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public HttpProxyConfig() { }

        /// <summary>
        /// Proxy URL
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// Proxy security scheme
        /// </summary>
        public ProxySecurityScheme ProxyScheme { get; set; }

        /// <summary>
        /// Proxy token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Proxy username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Proxy password
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// Proxy security scheme
    /// </summary>
    public enum ProxySecurityScheme
    {
        Basic,
        Bearer
    }
}
