using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WoT.Definitions;

namespace WoT.Implementation
{
    public class SimpleHTTPConsumer : IConsumer
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializer _serializer;
        public SimpleHTTPConsumer()
        {
            _httpClient = new HttpClient();
            _serializer = new JsonSerializer();
        }
        public Task<IConsumedThing> Consume(ThingDescription td)
        {
            throw new NotImplementedException();
        }

        public async Task<ThingDescription> RequestThingDescription(string url)
        {
            Uri tdUrl = new Uri(url);
            if (tdUrl.Scheme != "http") throw new Exception($"The protocol for accessing the TD url {url} is not HTTP");
            Console.WriteLine($"Info: Fetching TD from {url}");
            HttpResponseMessage tdResponse = await _httpClient.GetAsync(tdUrl);
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
    }
}
