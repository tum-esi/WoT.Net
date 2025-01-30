using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WoT.Binding.Http.Credentials;
using WoT.Binding.Http.SubscriptionProtocols;
using WoT.Core.Definitions;
using WoT.Core.Definitions.TD;

namespace WoT.Binding.Http
{
    /// <summary>
    /// An HTTP client implementation of <see cref="IProtocolClient"/> for the WoT Consumer
    /// </summary>
    public class HttpClient : IProtocolClient
    {
        /// <summary>
        /// The underlying <see cref="System.Net.Http.HttpClient"/> used by the <see cref="HttpClient"/>
        /// </summary>
        public readonly System.Net.Http.HttpClient httpClient;
        private bool _allowSelfSigned = false;
        private ICredential _credential;

        /// <summary>
        /// The protocol scheme of this client
        /// </summary>
        public string Scheme { get; internal set; } = "http";

        /// <summary>
        /// Create a new <see cref="HttpClient"/>
        /// </summary>
        /// <param name="clientConfig"></param>
        /// <param name="secure"></param>
        public HttpClient(HttpClientConfig clientConfig, bool secure = false)
        {
            HttpClientHandler handler = null;
            _allowSelfSigned = clientConfig.AllowSelfSigned;
            if (secure)
            {
                Scheme = "https";
                if (_allowSelfSigned)
                {
                    handler = new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = (HttpRequestMessage, certificate, chain, sslPolicyErrors) => true
                    };
                }
            }


            httpClient = handler != null ? new System.Net.Http.HttpClient(handler: handler) : new System.Net.Http.HttpClient();
        }

        #region ReadResource
        /// <summary>
        /// Perform a "read" on the resource with the given URI
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public async Task<Content> ReadResource(Form form)
        {
            return await ReadResource(form, CancellationToken.None);
        }

        /// <summary>
        /// Request a "read" on the resource with the given URI
        /// </summary>
        /// <param name="form"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Content> ReadResource(Form form, CancellationToken cancellationToken)
        {
            // See https://www.w3.org/TR/wot-thing-description11/#contentType-usage
            // Case: 1B
            HttpRequestMessage message = GenerateHttpRequest(
                form: form,
                defaultMethod: HttpMethod.Get,
                requestHeaders: new Dictionary<string, List<string>>
                {
                   { "Accept", new List<string> { form.ContentType } }
                },
                contentHeaders: null
             );

            HttpResponseMessage interactionResponse = await httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

            interactionResponse.EnsureSuccessStatusCode();

            string contentType = interactionResponse.Content.Headers.ContentType?.ToString() ?? ContentSerdes.DEFAULT;
            Stream responseStream = await interactionResponse.Content.ReadAsStreamAsync();
            var memStream = new MemoryStream();
            responseStream.CopyTo(memStream);

            return new Content(contentType, memStream);
        }
        #endregion

        #region InvokeResource
        /// <summary>
        /// Perform an "invoke" on the resource with the given URI
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public async Task<Content> InvokeResource(Form form)
        {
            return await InvokeResource(form, null, CancellationToken.None);
        }

        /// <summary>
        /// Perform an "invoke" on the resource with the given URI
        /// </summary>
        /// <param name="form"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Content> InvokeResource(Form form, CancellationToken cancellationToken)
        {
            return await InvokeResource(form, null, cancellationToken);
        }

        /// <summary>
        /// Perform an "invoke" on the resource with the given URI
        /// </summary>
        /// <param name="form"></param>
        /// <param name="content"></param>
        /// <returns></returns>

        public async Task<Content> InvokeResource(Form form, Content content)
        {
            return await InvokeResource(form, content, CancellationToken.None);
        }

        /// <summary>
        /// Perform an "invoke" on the resource with the given URI
        /// </summary>
        /// <param name="form"></param>
        /// <param name="content"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Content> InvokeResource(Form form, Content content, CancellationToken cancellationToken)
        {
            string acceptHeader;
            if (form.Response?.ContentType != null)
            {
                acceptHeader = form.Response?.ContentType;
            }
            else {
                acceptHeader = form.ContentType ?? ContentSerdes.DEFAULT;
            }

            HttpRequestMessage message = GenerateHttpRequest
            (
                form: form,
                defaultMethod: HttpMethod.Post,
                requestHeaders: new Dictionary<string, List<string>> 
                {
                    { "Accept", new List<string> { acceptHeader } }
                },
                contentHeaders: new Dictionary<string, List<string>> 
                {
                    {"Content-Type", new List<string> { content.type } }

                }
             );
            message.Content = content != null ? new StreamContent(content.body) : null;

            var interactionResponse = await httpClient.SendAsync(message, cancellationToken);

            interactionResponse.EnsureSuccessStatusCode();

            Stream responseStream = await interactionResponse.Content.ReadAsStreamAsync();
            string contentType = interactionResponse.Content.Headers.ContentType?.ToString() ?? ContentSerdes.DEFAULT;
            var memStream = new MemoryStream();
            responseStream.CopyTo(memStream);

            return new Content(contentType, memStream);
        }
        #endregion

        #region WriteResource

        /// <summary>
        /// Perform a "write" on the resource with the given URI
        /// </summary>
        /// <param name="form"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task WriteResource(Form form, Content content)
        {
            await WriteResource(form, content, CancellationToken.None);
        }

        /// <summary>
        /// Perform a "write" on the resource with the given URI
        /// </summary>
        /// <param name="form"></param>
        /// <param name="content"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task WriteResource(Form form, Content content, CancellationToken cancellationToken)
        {
            HttpRequestMessage message = GenerateHttpRequest
            (
                form: form,
                defaultMethod: HttpMethod.Put,
                content: content,
                requestHeaders: null,
                contentHeaders: new Dictionary<string, List<string>>
                {
                    {"Content-Type", new List<string> { content.type } }

                }
             );

            HttpResponseMessage interactionResponse = await httpClient.SendAsync(message, cancellationToken);
            interactionResponse.EnsureSuccessStatusCode();
        }
        #endregion

        #region subscribeResource

        /// <summary>
        /// Subscribe to a resource
        /// </summary>
        /// <param name="form"></param>
        /// <param name="nextHandler"></param>
        /// <param name="errorHandler"></param>
        /// <param name="complete"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IProtocolSubscription> SubscribeResource(Form form, Action<Content> nextHandler, Action<Exception> errorHandler = null, Action complete = null)
        {
            string defaultSubProtocol = "longpoll";
            string subprotocol = form.Subprotocol;

            if (subprotocol == null)
            {
                Console.WriteLine($"`Subscribing to {form.Href} using long polling for form without subprotocol");
                subprotocol = defaultSubProtocol;
            }

            IProtocolSubscription subscription = null;
            if (subprotocol == defaultSubProtocol)
            {
                subscription = new LongPollingSubscription(form, this);
            }
            else if (subprotocol == "sse")
            {
                subscription = new SSESubscription(form);
            }
            else
            {
                throw new Exception($"HttpClient does not support subprotocol {form.Subprotocol}");
            }

            await subscription.Open(nextHandler, errorHandler, complete);
            return subscription;
        }

        /// <summary>
        /// Unlink a resource
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public Task UnlinkResource(Form form)
        {
            return Task.CompletedTask;
        }

        #endregion

        public Task Start()
        {
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Set the security for the client
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public bool SetSecurity(SecurityScheme[] metadata, Dictionary<CredentialScheme, object> credentials)
        {
            if (metadata == null || metadata.Length == 0)
            {
                return false;
            }

            SecurityScheme security = metadata[0];

            switch (security.Scheme)
            {
                case "basic":
                    BasicSecurityScheme basicSecurityScheme = (BasicSecurityScheme)security;
                    var credential = credentials[CredentialScheme.Basic];
                    _credential = new BasicCredential((BasicCredentialConfig)credential, basicSecurityScheme);
                    break;
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Request a Thing Description from a URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<Content> RequestThingDescription(string url)
        {
            Uri tdUrl = new Uri(url);
            return await RequestThingDescription(tdUrl);
        }

        /// <summary>
        /// Request a Thing Description from a URL
        /// </summary>
        /// <param name="tdUrl"></param>
        /// <returns></returns>
        public async Task<Content> RequestThingDescription(Uri tdUrl)
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, tdUrl);
            message.Headers.Add("Accept", "application/td+json");
            HttpResponseMessage tdResponse = await httpClient.SendAsync(message);

            tdResponse.EnsureSuccessStatusCode();

            Stream tdStream = await tdResponse.Content.ReadAsStreamAsync();
            string contentType = tdResponse.Content.Headers.ContentType?.ToString() ?? "application/td+json";
            MemoryStream memoryStream = new MemoryStream();
            tdStream.CopyTo(memoryStream);

            return new Content(contentType, memoryStream);
        }

        /// <summary>
        /// Generate an HTTP request
        /// </summary>
        /// <param name="form"></param>
        /// <param name="defaultMethod"></param>
        /// <param name="content"></param>
        /// <param name="requestHeaders"></param>
        /// <param name="contentHeaders"></param>
        /// <returns></returns>
        public HttpRequestMessage GenerateHttpRequest(
            Form form,
            HttpMethod defaultMethod,
            Content content = null,
            Dictionary<string, List<string>> requestHeaders = null,
            Dictionary<string, List<string>> contentHeaders = null)
        {
            string formMethod = form.AdditionalData?["htv:methodName"].Value<string>();
            HttpMethod httpMethod;
            switch (formMethod)
            {
                case "GET": httpMethod = HttpMethod.Get; break;
                case "PUT": httpMethod = HttpMethod.Put; break;
                case "POST": httpMethod = HttpMethod.Post; break;
                case "DELETE": httpMethod = HttpMethod.Delete; break;
                case "HEAD": httpMethod = HttpMethod.Head; break;
                case "PATCH": httpMethod = new HttpMethod("PATCH"); break;
                default: httpMethod = defaultMethod; break;
            }

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, form.Href)
            {
                Content = content != null ? new StreamContent(content.body) : null
            };

            if (requestHeaders != null) {
                foreach (var item in requestHeaders)
                {
                    httpRequestMessage.Headers.Add(item.Key, item.Value);
                }
            }

            if(contentHeaders != null)
            {
                foreach (var item in contentHeaders)
                {
                    httpRequestMessage.Content.Headers.Add(item.Key, item.Value);
                }
            }
            

            var formHeaders = form.AdditionalData?["htv:headers"].Value<List<Dictionary<string, List<string>>>>();

            // Handle form headers
            if (formHeaders != null)
            {
                foreach (var item in formHeaders)
                {
                    foreach (var key in item.Keys)
                    {
                        if (key.ToLower().Contains("content") || key.ToLower().Contains("transfer") )
                        {
                            if(httpRequestMessage.Content.Headers.Contains(key))
                            {
                                httpRequestMessage.Content.Headers.Remove(key);
                            }

                            httpRequestMessage.Content.Headers.Add(key, item[key]);
                        }
                        else
                        {
                            if (httpRequestMessage.Headers.Contains(key))
                            {
                                httpRequestMessage.Content.Headers.Remove(key);
                            }

                            httpRequestMessage.Headers.Add(key, item[key]);
                        }
                    }
                }
            }

            _credential?.Sign(httpRequestMessage);

            return httpRequestMessage;
        }

        /// <summary>
        /// Send an HTTP request
        /// </summary>
        /// <param name="httpRequestMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Content> SendRequest(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            var interactionResponse = await httpClient.SendAsync(httpRequestMessage, cancellationToken);

            interactionResponse.EnsureSuccessStatusCode();

            Stream responseStream = await interactionResponse.Content.ReadAsStreamAsync();
            string contentType = interactionResponse.Content.Headers.ContentType?.ToString() ?? ContentSerdes.DEFAULT;
            var memStream = new MemoryStream();
            responseStream.CopyTo(memStream);

            return new Content(contentType, memStream);
        }
    }
}
