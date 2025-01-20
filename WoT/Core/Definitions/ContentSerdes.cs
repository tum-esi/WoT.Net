using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WoT.Core.Definitions.TD;
using WoT.Core.Implementation.Codecs;

namespace WoT.Core.Definitions
{
    public interface IContentCodec
    {
        string GetMediaType();
        object BytesToValue(byte[] bytes, DataSchema schema = null, Dictionary<string, string> parameters = null);
        byte[] ValueToBytes(object value, DataSchema schema = null, Dictionary<string, string> parameters = null);
    }

    public class ContentSerdes
    {
        private static ContentSerdes instance;

        public static readonly string DEFAULT = "application/json";
        public static readonly string TD = "application/td+json";
        public static readonly string JSON_LD = "application/ld+json";

        private readonly Dictionary<string, IContentCodec> _codecs = new Dictionary<string, IContentCodec>();
        private readonly HashSet<string> _offered = new HashSet<string>();
        private static readonly string MediaTypePattern = @"(?<mediaType>(?<type>\w+)\/((\w+)(\+\w*)*))(?<MediaTypeParameters>(\;\s*((?<parameter>\w+)\=(?<value>[\S-["":;]]+|(?:""[\S\s]+""))))*)";
        private static readonly Regex mediaTypeRegex = new Regex(MediaTypePattern, RegexOptions.Multiline | RegexOptions.Compiled);

        public object Log { get; private set; }

        public static ContentSerdes GetInstance()
        {
            if (instance == null)
            {
                instance = new ContentSerdes();

                // add default JSON codecs
                instance.AddCodec(new JsonCodec(), true);
                instance.AddCodec(new JsonCodec("application/senml+json"));
                instance.AddCodec(new JsonCodec("application/td+json"));
                instance.AddCodec(new JsonCodec("application/ld+json"));
            }
            return instance;
        }

        public static string GetMediaType(string contentType)
        {
            Match match = mediaTypeRegex.Match(contentType);
            if (match.Success)
            {
                return match.Groups["mediaType"].Value;
            }
            return null;
        }

        public static Dictionary<string, string> GetMediaTypeParameters(string contentType)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            Match match = mediaTypeRegex.Match(contentType);
            if (match.Success)
            {
                for(int i = 0; i < match.Groups["parameter"].Captures.Count; i++)
                {
                    parameters.Add(match.Groups["parameter"].Captures[i].Value, match.Groups["value"].Captures[i].Value);
                }
            }
            return parameters;
        }

        public void AddCodec(IContentCodec codec, bool offered = false)
        {
            ContentSerdes.GetInstance()._codecs.Add(codec.GetMediaType(), codec);
            if (offered)
            {
                ContentSerdes.GetInstance()._offered.Add(codec.GetMediaType());
            }
        }

        public string[] GetSupportedMediaTypes()
        {
            return ContentSerdes.GetInstance()._codecs.Keys.ToArray();
        }

        public string[] GetOfferedMediaTypes()
        {
            return ContentSerdes.GetInstance()._offered.ToArray();
        }

        public bool IsSupported(string contentType)
        {
            return ContentSerdes.GetInstance()._codecs.ContainsKey(GetMediaType(contentType));
        }

        public object ContentToValue(Content content, DataSchema schema)
        {
            if (content.type == null)
            {
                if (content.body.Length > 0)
                {
                    // default to application/json
                    content.type = ContentSerdes.DEFAULT;
                }
                else
                {
                    // empty payload without media type -> void/undefined (note: e.g., empty payload with text/plain -> "")
                    return null;
                }
            }

            string mt = ContentSerdes.GetMediaType(content.type);
            Dictionary<string, string> par = ContentSerdes.GetMediaTypeParameters(content.type);

            if (this._codecs.ContainsKey(mt))
            {
                return this._codecs[mt].BytesToValue(content.body, schema, par);
            }
            else
            {
                Console.WriteLine($"ContentSerdes passthrough due to unsupported media type {mt}");
                return Encoding.UTF8.GetString(content.body);
            }
        }

        public Content ValueToContent(object value, string contentType, DataSchema schema)
        {
            if (value == null || value.GetType() == null) Console.WriteLine("ContentSerdes valueToContent got no value");

            byte[] bytes;

            string mt = ContentSerdes.GetMediaType(contentType);
            Dictionary<string, string> par = ContentSerdes.GetMediaTypeParameters(contentType);

            if (this._codecs.ContainsKey(mt))
            {
                bytes = this._codecs[mt].ValueToBytes(value, schema, par);
            }
            else
            {
                Console.WriteLine($"ContentSerdes passthrough due to unsupported media type {mt}");
                string valueString = JsonConvert.SerializeObject(value);
                bytes = Encoding.UTF8.GetBytes(valueString);
            }

            return new Content(contentType, bytes);
        }
    }
}
