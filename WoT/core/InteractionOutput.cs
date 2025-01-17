using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WoT.Definitions;
using WoT.Errors;
namespace WoT.Implementation
{
    public class InteractionOutput : IInteractionOutput
    {
        private readonly Form _form;
        public InteractionOutput(Form form)
        {
            _form = form;
        }
        public Stream Data => null;

        public bool DataUsed => false;

        public Form Form => _form;

        public IDataSchema Schema => null;

        public Task<byte[]> ArrayBuffer()
        {
            return null;
        }
        public Task Value()
        {
            return null;
        }
    }

    /// <summary>
    /// An implementation of <see cref="IInteractionOutput{T}"/>
    /// </summary>
    /// <typeparam name="T">output data type</typeparam>
    public class InteractionOutput<T> : IInteractionOutput<T>
    {
        private readonly Form _form;
        private readonly Stream _data;
        private bool _dataUsed;
        private readonly T _value;
        private bool _isValueSet;
        private readonly IDataSchema _schema;
        private readonly JSchema _parsedSchema;
        private readonly JsonSerializer _serializer;
        public InteractionOutput(DataSchema schema, Form form, Stream data)
        {
            _form = form;
            _data = data;
            _dataUsed = false;
            //_content = content;
            _schema = schema;
            string schemaString = JsonConvert.SerializeObject(schema, settings: new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            _parsedSchema = JSchema.Parse(schemaString);
            _serializer = new JsonSerializer();

        }
        public InteractionOutput(DataSchema schema, Form form, Stream data, T value)
        {
            _form = form;
            _data = data;
            _dataUsed = false;
            _value = value;
            _isValueSet = true;
            _schema = schema;
            string schemaString = JsonConvert.SerializeObject(schema, settings: new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            _parsedSchema = JSchema.Parse(schemaString);
            _serializer = new JsonSerializer();

        }
        public InteractionOutput(PropertyAffordance schema, Form form, Stream data)
        {
            _form = form;
            _data = data;
            //_content = content;
            _schema = schema;
            _parsedSchema = JSchema.Parse(schema.OriginalJson);
            _serializer = new JsonSerializer();

        }
        public InteractionOutput(PropertyAffordance schema, Form form, Stream data, T value)
        {
            _form = form;
            _data = data;
            _value = value;
            _isValueSet = true;
            _schema = schema;
            _parsedSchema = JSchema.Parse(schema.OriginalJson);
            _serializer = new JsonSerializer();

        }
        public Stream Data => _data;

        public bool DataUsed => _dataUsed;

        public Form Form => _form;

        public IDataSchema Schema => _schema;

        public async Task<byte[]> ArrayBuffer()
        {
            Task<byte[]> task = Task.Run(() =>
           {
               if (!_data.CanRead || _dataUsed)
               {
                   throw new NotReadableError();
               }
               MemoryStream ms = new MemoryStream();
               _data.CopyTo(ms);
               _dataUsed = true;
               byte[] arrayBuffer = ms.ToArray();
               return arrayBuffer;
           });
            return await task;
        }

        public async Task<T> Value()
        {
            Task<T> task = Task.Run(() =>
            {
                if (!_data.CanRead || _dataUsed)
                {
                    throw new NotReadableError();
                }
                StreamReader sr = new StreamReader(_data, Encoding.UTF8);
                string valueJson = sr.ReadToEnd();
                _dataUsed = true;
                // Intialize validating schema
                JsonTextReader reader = new JsonTextReader(new StringReader(valueJson));
                JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(reader)
                {
                    Schema = _parsedSchema
                };
                //Add Error listener
                IList<string> messages = new List<string>();
                validatingReader.ValidationEventHandler += (o, a) => messages.Add(a.Message);
                //Deserialize
                T value = _serializer.Deserialize<T>(validatingReader);

                bool isValid = (messages.Count == 0);
                if (isValid)
                {
                    return value;
                }
                else
                {
                    throw new Exception("Schema Validation failed for value of readProperty");
                }
            });
            return await task;
        }
    }

}