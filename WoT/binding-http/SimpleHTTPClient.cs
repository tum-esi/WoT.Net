using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tavis.UriTemplates;
using WoT.Definitions;
using WoT.Errors;
using System.Threading;
using WoT.ProtocolBindings;

namespace WoT.Implementation
{
    /// <summary>
    /// A simple WoT Consumer that is capable of requesting TDs only from HTTP resources und consumes them to generate <see cref="SimpleConsumedThing"/>
    /// </summary>
    public class SimpleHTTPClient : IProtocolClient
    {
        private readonly JsonSerializer _serializer;
        public readonly HttpClient httpClient;

        public string Scheme { get; } = "http";

    /// <inheritdoc/>
    public SimpleHTTPClient()
        {
            httpClient = new HttpClient();
            _serializer = new JsonSerializer();

        }

        
        public async Task<Stream> SendGetRequest(Form form)
        {
                
            HttpResponseMessage interactionResponse = await httpClient.GetAsync(form.Href);
            interactionResponse.EnsureSuccessStatusCode();
            Stream responseStream = await interactionResponse.Content.ReadAsStreamAsync();
            return responseStream;
            
        }

       

        public async Task<Stream> SendGetRequest(Form form, CancellationToken cancellationToken)
        {
            HttpResponseMessage interactionResponse = await httpClient.GetAsync(form.Href, cancellationToken);
            interactionResponse.EnsureSuccessStatusCode();
            Stream responseStream = await interactionResponse.Content.ReadAsStreamAsync();
            return responseStream;

        }
        public async Task<Stream> SendPostRequest(Form form)
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, form.Href);
            var interactionResponse = await httpClient.SendAsync(message);
            interactionResponse.EnsureSuccessStatusCode();
            Stream responseStream = await interactionResponse.Content.ReadAsStreamAsync();
            return responseStream;
        }
        public async Task<Stream> SendPostRequest<U>(Form form, U parameters)
        {

            string payloadString = JsonConvert.SerializeObject(parameters);
            StringContent payload = new StringContent(payloadString, Encoding.UTF8, "application/json");
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, form.Href);
            message.Content = payload;
            var interactionResponse = await httpClient.SendAsync(message);
            interactionResponse.EnsureSuccessStatusCode();
            Stream responseStream = await interactionResponse.Content.ReadAsStreamAsync();
            return responseStream;
        }

        public async Task SendPutRequest<T>(Form form, T value)
        {
            string payloadString = JsonConvert.SerializeObject(value);
            var payload = new StringContent(payloadString, Encoding.UTF8, "application/json");
            var message = new HttpRequestMessage(HttpMethod.Put, form.Href);
            message.Content = payload;
            HttpResponseMessage interactionResponse = await httpClient.SendAsync(message);
            interactionResponse.EnsureSuccessStatusCode();
            
        }


        

        public async Task<ThingDescription> RequestThingDescription(string url)
        {
            Uri tdUrl = new Uri(url);
            HttpResponseMessage tdResponse = await httpClient.GetAsync(tdUrl);
            tdResponse.EnsureSuccessStatusCode();
            Console.WriteLine($"Info: Fetched TD from {url} successfully");
            Console.WriteLine($"Info: Parsing TD");
            HttpContent body = tdResponse.Content;
            string tdData = await body.ReadAsStringAsync();
            TextReader reader = new StringReader(tdData);
            ThingDescription td = _serializer.Deserialize(reader, typeof(ThingDescription)) as ThingDescription;
            Console.WriteLine($"Info: Parsed TD successfully");
            return td;
        }

        public async Task<ThingDescription> RequestThingDescription(Uri tdUrl)
        {
            if (tdUrl.Scheme != "http") throw new Exception($"The protocol for accessing the TD url {tdUrl.OriginalString} is not HTTP");
            Console.WriteLine($"Info: Fetching TD from {tdUrl.OriginalString}");
            HttpResponseMessage tdResponse = await httpClient.GetAsync(tdUrl);
            tdResponse.EnsureSuccessStatusCode();
            Console.WriteLine($"Info: Fetched TD from {tdUrl.OriginalString} successfully");
            Console.WriteLine($"Info: Parsing TD");
            HttpContent body = tdResponse.Content;
            string tdData = await body.ReadAsStringAsync();
            TextReader reader = new StringReader(tdData);
            ThingDescription td = _serializer.Deserialize(reader, typeof(ThingDescription)) as ThingDescription;
            Console.WriteLine($"Info: Parsed TD successfully");
            return td;
        }
    }


}
