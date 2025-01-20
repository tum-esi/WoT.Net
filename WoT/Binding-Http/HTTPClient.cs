using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using WoT.Core.Definitions;
using WoT.Core.Definitions.TD;

namespace WoT.Implementation
{
    /// <summary>
    /// A simple WoT Consumer that is capable of requesting TDs only from HTTP resources und consumes them to generate <see cref="SimpleConsumedThing"/>
    /// </summary>
    public class HTTPClient : IProtocolClient
    {
        private readonly JsonSerializer _serializer;
        public readonly HttpClient httpClient;

        public string Scheme { get; } = "http";

    /// <inheritdoc/>
    public HTTPClient()
        {
            httpClient = new HttpClient();
            _serializer = new JsonSerializer();

        }

        #region ReadResource
        public async Task<Content> ReadResource(Form form)
        {
            return await ReadResource(form, CancellationToken.None);
        }
        public async Task<Content> ReadResource(Form form, CancellationToken cancellationToken)
        {
            // See https://www.w3.org/TR/wot-thing-description11/#contentType-usage
            // Case: 1B
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, form.Href);
            message.Headers.Add("Accept", form.ContentType);
            HttpResponseMessage interactionResponse = await httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

            interactionResponse.EnsureSuccessStatusCode();

            string contentType = interactionResponse.Content.Headers.ContentType?.ToString() ?? ContentSerdes.DEFAULT;
            byte[] responseBuffer = await interactionResponse.Content.ReadAsByteArrayAsync();

            interactionResponse.Dispose();

            return new Content(contentType, responseBuffer);
        }
        #endregion

        #region InvokeResource
        public async Task<Content> InvokeResource(Form form)
        {
            return await InvokeResource(form, null, CancellationToken.None);
        }

        public async Task<Content> InvokeResource(Form form, CancellationToken cancellationToken)
        {
           return await InvokeResource(form, null, cancellationToken);
        }

        public async Task<Content> InvokeResource(Form form, Content content)
        {
            return await InvokeResource(form, content, CancellationToken.None);
        }
        public async Task<Content> InvokeResource(Form form, Content content, CancellationToken cancellationToken)
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, form.Href)
            {
                Content = content != null ? new ByteArrayContent(content.body) : null
            };
            if (content != null) message.Content.Headers.Add("Content-Type", content.type);

            if (form.Response?.ContentType != null)
            {
                message.Headers.Add("Accept", form.Response?.ContentType);
            }
            else if (form.ContentType != null)
            {
                message.Headers.Add("Accept", form.ContentType);
            }
            else
            {
                message.Headers.Add("Accept", ContentSerdes.DEFAULT);
            }
            var interactionResponse = await httpClient.SendAsync(message, cancellationToken);

            interactionResponse.EnsureSuccessStatusCode();

            byte[] responseBuffer = await interactionResponse.Content.ReadAsByteArrayAsync();
            string contentType = interactionResponse.Content.Headers.ContentType?.ToString() ?? ContentSerdes.DEFAULT;

            return new Content(contentType, responseBuffer);
        }
        #endregion

        public async Task WriteResource(Form form, Content content)
        {
            await WriteResource(form, content, CancellationToken.None);
        }

        public async Task WriteResource(Form form, Content content, CancellationToken cancellationToken)
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Put, form.Href)
            {
                Content = new ByteArrayContent(content.body)
            };
            HttpResponseMessage interactionResponse = await httpClient.SendAsync(message, cancellationToken);
            interactionResponse.EnsureSuccessStatusCode();
        }


        

        public async Task<Content> RequestThingDescription(string url)
        {
            Uri tdUrl = new Uri(url);
            return await RequestThingDescription(tdUrl);
        }

        public async Task<Content> RequestThingDescription(Uri tdUrl)
        {
            if (tdUrl.Scheme != "http") throw new Exception($"The protocol for accessing the TD url {tdUrl.OriginalString} is not HTTP");
            Console.WriteLine($"Info: Fetching TD from {tdUrl.OriginalString}");

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, tdUrl);
            message.Headers.Add("Accept", "application/td+json");
            HttpResponseMessage tdResponse = await httpClient.SendAsync(message);

            tdResponse.EnsureSuccessStatusCode();

            Console.WriteLine($"Info: Fetched TD from {tdUrl.OriginalString} successfully");
            Console.WriteLine($"Info: Parsing TD");
            byte[] tdBuffer = await tdResponse.Content.ReadAsByteArrayAsync();
            string contentType = tdResponse.Content.Headers.ContentType?.ToString() ?? "application/td+json";

            return new Content(contentType, tdBuffer);
        }
    }


}
