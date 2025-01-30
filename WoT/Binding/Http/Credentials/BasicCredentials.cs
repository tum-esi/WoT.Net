using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using WoT.Core.Definitions.TD;

namespace WoT.Binding.Http.Credentials
{
    public struct BasicCredentialConfig
    {
        public BasicCredentialConfig(string username, string password)
        {
            Username = username;
            Password = password;
        }
        internal string Username { get; set; }
        internal string Password { get; set; }
    }
    internal class BasicCredential : ICredential
    {
        private readonly string _username;
        private readonly string _password;
        private readonly BasicSecurityScheme _options;

        internal BasicCredential(string username, string password, BasicSecurityScheme options = null)
        {
            _username = username;
            _password = password;
            _options = options;
        }
        internal BasicCredential(BasicCredentialConfig config, BasicSecurityScheme options = null)
        {
            _username = config.Username;
            _password = config.Password;
            _options = options;
        }

        public void Sign(HttpRequestMessage requestMessage)
        {
            var authenticationString = $"{_username}:{_password}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));
            string basicAuth = $"Basic {base64EncodedAuthenticationString}";
            if (_options != null && _options.In == "header" && _options.Name != null)
            {
                requestMessage.Headers.Add(_options.Name, basicAuth);
            }
            else
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
            }
        }
    }
}
