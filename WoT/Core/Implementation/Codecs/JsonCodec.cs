using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WoT.Core.Definitions;
using WoT.Core.Definitions.TD;

namespace WoT.Core.Implementation.Codecs
{
    public class JsonCodec : IContentCodec
    {
        private readonly string _subMediaType;

        public JsonCodec(string subMediaType = null)
        {
            _subMediaType = subMediaType ?? ContentSerdes.DEFAULT;
        }

        public string GetMediaType()
        {
            return _subMediaType;
        }

        public T BytesToValue<T>(byte[] bytes, IDataSchema schema = null, Dictionary<string, string> parameters = null)
        {
            string encoding = parameters != null && parameters.ContainsKey("charset") ? parameters["charset"].ToLower() : "utf-8";
            T parsed;
            try
            {
                string valueString = Encoding.GetEncoding(encoding).GetString(bytes);
                parsed = JsonConvert.DeserializeObject<T>(valueString);
            }
            catch (Exception err)
            {
                if (bytes.Length == 0)
                {
                    parsed = default;
                }
                else
                {
                    throw err;
                }
            }
            return parsed;
        }

        public byte[] ValueToBytes<T>(T value, IDataSchema schema = null, Dictionary<string, string> parameters = null)
        {
            string encoding = parameters != null && parameters.ContainsKey("charset") ? parameters["charset"].ToLower() : "utf-8";
            string body = "";
            if (value != null)
            {
                body = JsonConvert.SerializeObject(value);
            }

            return Encoding.GetEncoding(encoding).GetBytes(body);

        }
    }
}
