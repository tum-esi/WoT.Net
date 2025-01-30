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
    /// <summary>
    /// Interface for content codecs. To be implemented by classes that provide serialization/deserialization of content.
    /// </summary>
    public interface IContentCodec
    {
        string GetMediaType();
        T BytesToValue<T>(byte[] bytes, IDataSchema schema = null, Dictionary<string, string> parameters = null);
        byte[] ValueToBytes<T>(T value, IDataSchema schema = null, Dictionary<string, string> parameters = null);
    }
    /// <summary>
    /// A struct for content read from a stream.
    /// </summary>
    public struct ReadContent
    {
        public string type;
        public byte[] body;
        public ReadContent(string type, byte[] body)
        {
            this.type = type;
            this.body = body;
        }
    }

    /// <summary>
    /// Content Management class for serialization/deserialization of content.
    /// </summary>
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

        /// <summary>
        /// Get the singleton instance of the ContentSerdes class.
        /// </summary>
        /// <returns>a singleton instance of ContentSerdes</returns>
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

        /// <summary>
        /// Get the media type from a content type string.
        /// </summary>
        /// <param name="contentType">content type</param>
        /// <returns>media type</returns>
        public static string GetMediaType(string contentType)
        {
            Match match = mediaTypeRegex.Match(contentType);
            if (match.Success)
            {
                return match.Groups["mediaType"].Value;
            }
            return null;
        }

        /// <summary>
        /// Get the media type parameters from a content type string.
        /// </summary>
        /// <param name="contentType">content type as a string</param>
        /// <returns>a dictionary of all media types and their parameters</returns>
        public static Dictionary<string, string> GetMediaTypeParameters(string contentType)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            Match match = mediaTypeRegex.Match(contentType);
            if (match.Success)
            {
                for (int i = 0; i < match.Groups["parameter"].Captures.Count; i++)
                {
                    parameters.Add(match.Groups["parameter"].Captures[i].Value, match.Groups["value"].Captures[i].Value);
                }
            }
            return parameters;
        }

        /// <summary>
        /// Add a codec to the ContentSerdes instance.
        /// </summary>
        /// <param name="codec">new codec</param>
        /// <param name="offered">signals if a codec should be offered by a Exposed Things</param>
        public void AddCodec(IContentCodec codec, bool offered = false)
        {
            _codecs.Add(codec.GetMediaType(), codec);
            if (offered)
            {
                _offered.Add(codec.GetMediaType());
            }
        }

        /// <summary>
        /// Get the supported media types.
        /// </summary>
        /// <returns>array of supported media types as strings</returns>
        public string[] GetSupportedMediaTypes()
        {
            return _codecs.Keys.ToArray();
        }

        /// <summary>
        /// Get the offered media types.
        /// </summary>
        /// <returns>array of offered media types as strings</returns>
        public string[] GetOfferedMediaTypes()
        {
            return _offered.ToArray();
        }

        /// <summary>
        /// Check if a content type is supported.
        /// </summary>
        /// <param name="contentType">the content type to check for</param>
        /// <returns><see langword="true"/> if supported, otherwise <see langword="false"/></returns>
        public bool IsSupported(string contentType)
        {
            return _codecs.ContainsKey(GetMediaType(contentType));
        }

        /// <summary>
        /// Transform content to a value.
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="content">content</param>
        /// <param name="schema">schema for object validation</param>
        /// <returns></returns>
        public T ContentToValue<T>(ReadContent content, IDataSchema schema)
        {
            if (content.type == null)
            {
                if (content.body.Length > 0)
                {
                    // default to application/json
                    content.type = DEFAULT;
                }
                else
                {
                    // empty payload without media type -> void/undefined (note: e.g., empty payload with text/plain -> "")
                    return default;
                }
            }

            string mt = GetMediaType(content.type);
            Dictionary<string, string> par = GetMediaTypeParameters(content.type);

            if (_codecs.ContainsKey(mt))
            {
                return _codecs[mt].BytesToValue<T>(content.body, schema, par);
            }
            else
            {
                Console.WriteLine($"ContentSerdes passthrough due to unsupported media type {mt}");
                return (T)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(content.body));
            }
        }


        /// <summary>
        /// Transform a value to content.
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="value">value to convert</param>
        /// <param name="contentType">content-type to convert to</param>
        /// <param name="schema">schema for validation</param>
        /// <returns></returns>
        public Content ValueToContent<T>(T value, string contentType, IDataSchema schema)
        {
            if (value == null || GetType() == null) Console.WriteLine("ContentSerdes valueToContent got no value");

            byte[] bytes;

            string mt = GetMediaType(contentType);
            Dictionary<string, string> par = GetMediaTypeParameters(contentType);

            if (_codecs.ContainsKey(mt))
            {
                bytes = _codecs[mt].ValueToBytes<T>(value, schema, par);
            }
            else
            {
                Console.WriteLine($"ContentSerdes passthrough due to unsupported media type {mt}");
                string valueString = JsonConvert.SerializeObject(value);
                bytes = Encoding.UTF8.GetBytes(valueString);
            }

            return new Content(contentType, new MemoryStream(bytes));
        }
    }
}
