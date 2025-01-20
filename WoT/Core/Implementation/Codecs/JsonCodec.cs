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
        private string _subMediaType;

        public JsonCodec(string subMediaType = null)
        {
            _subMediaType = subMediaType ?? ContentSerdes.DEFAULT;
        }

        public string GetMediaType()
        {
            return _subMediaType;
        }

        public object BytesToValue(byte[] bytes, DataSchema schema = null, Dictionary<string, string> parameters = null)
        {
            string encoding = parameters != null && parameters.ContainsKey("charset") ? parameters["charset"].ToLower() : "utf-8";
            object parsed;
            try
            {
                parsed = JsonConvert.DeserializeObject(Encoding.GetEncoding(encoding).GetString(bytes));
            }
            catch (Exception err)
            {
                if (bytes.Length == 0)
                {
                    parsed = null;
                } else
                {
                    throw err;
                }
            }
            return parsed;
        }

        public byte[] ValueToBytes(object value, DataSchema schema = null, Dictionary<string, string> parameters = null)
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
