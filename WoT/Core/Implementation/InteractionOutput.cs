using Json.Schema;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using WoT.Core.Definitions;
using WoT.Core.Definitions.TD;
using WoT.Core.Errors;

namespace WoT.Core.Implementation
{

    /// <summary>
    /// An implementation of <see cref="IInteractionOutputValue{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct InteractionOutputValue<T> : IInteractionOutputValue<T>
    {
        /// <summary>
        /// Indicates if the output is empty
        /// </summary>
        public bool IsEmpty { get; internal set; }

        /// <summary>
        /// The value of the output
        /// </summary>
        public T Value { get; internal set; }
    }

    /// <summary>
    /// An implementation of <see cref="IInteractionOutput"/>
    /// </summary>
    public class InteractionOutput : IInteractionOutput
    {
        private readonly Form _form;
        private bool _ignoreValidation = false;
        public InteractionOutput(Form form, bool ignoreValidation = false)
        {
            _form = form;
            _ignoreValidation = ignoreValidation;
        }
        public Stream Data => null;

        public bool DataUsed => false;

        public Form Form => _form;

        public IDataSchema Schema => null;

        public bool IgnoreValidation => _ignoreValidation;

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
    /// <typeparam name="T">output Data type</typeparam>
    public class InteractionOutput<T> : IInteractionOutput<T>
    {
        private readonly Content _content;
        private T _value;
        private byte[] _buffer;
        private readonly Form _form;
        private readonly IDataSchema _schema;
        private bool _dataUsed;
        private bool _ignoreValidation;
        public InteractionOutput(Content content, Form form, IDataSchema schema, bool ignoreValidation = false)
        {
            _content = content;
            _form = form;
            _schema = schema;
            _ignoreValidation = ignoreValidation;
        }
        public Stream Data
        {
            get
            {
                if (_dataUsed)
                {
                    throw new NotReadableError("Can't read the stream once it has been already used");
                }

                // Once the stream is created Data might be pulled unpredictably
                // therefore we assume that it is going to be used to be safe.
                _dataUsed = true;


                return _content.body;
            }
        }

        public bool DataUsed => _dataUsed;

        public Form Form => _form;

        public IDataSchema Schema => _schema;

        public async Task<byte[]> ArrayBuffer()
        {
            if (_buffer != null)
            {
                return _buffer;
            }

            if (!Data.CanRead || _dataUsed)
            {
                throw new NotReadableError("Can't read the stream once it has been already used");
            }


            return await this._content.ToBuffer();
        }

        public async Task<T> Value()
        {
            if (_schema == null)
            {
                throw new NotAllowedError("No schema defined; assuming empty payload");
            }

            if (_value != null && !_value.Equals(default(T))) { return _value;  }

            if (_dataUsed)
            {
                throw new NotReadableError("Can't read the stream once it has been already used");
            }

            if (_form == null)
            {
                throw new NotReadableError("No form defined");
            }

            if (_schema.Const == null && _schema.Enum == null && _schema.OneOf == null && _schema.Type == null)
            {
                throw new NotReadableError("No schema type defined");
            }

            if (!ContentSerdes.GetInstance().IsSupported(_content.type))
            {
                throw new NotSupportedError($"Content type {_content.type} not supported");
            }

            var bytes = await _content.ToBuffer();
            _dataUsed = true;
            _buffer = bytes;

            _value = ContentSerdes.GetInstance().ContentToValue<T>(new ReadContent(type: _content.type, body: bytes), _schema);

            EvaluationResults result = null;
            if (!_ignoreValidation)
            {
                string schemaString = JsonConvert.SerializeObject(_schema);
                string valueString = JsonConvert.SerializeObject(_value);

                var schema = JsonSchema.FromText(schemaString);
                var valueNode = JsonNode.Parse(valueString);
                result = schema.Evaluate(valueNode);
            }


            if (result == null || result.IsValid)
            {
                if (bytes.Length > 0)
                {
                    return _value;
                }
                else
                {
                    throw new EvalError("No value found in the content");
                }
            }
            else
            {
                throw new EvalError("Invalid value according to DataSchema: " + result.Errors);
            }
        }

        public async Task<object> ValueAsObject()
        {
            if (_schema == null)
            {
                Console.WriteLine("No schema defined. Hence null is reported for Value() function.If you are invoking an action with no output that is on purpose, otherwise consider using arrayBuffer().");
                return null;
            }

            if (_value != null && !_value.Equals(default(T))) { return _value; }

            if (_dataUsed)
            {
                throw new NotReadableError("Can't read the stream once it has been already used");
            }

            if (_form == null)
            {
                throw new NotReadableError("No form defined");
            }

            if (_schema.Const == null && _schema.Enum == null && _schema.OneOf == null && _schema.Type == null)
            {
                throw new NotReadableError("No schema type defined");
            }

            if (!ContentSerdes.GetInstance().IsSupported(_content.type))
            {
                throw new NotSupportedError($"Content type {_content.type} not supported");
            }

            var bytes = await _content.ToBuffer();
            _dataUsed = true;
            _buffer = bytes;

            _value = ContentSerdes.GetInstance().ContentToValue<T>(new ReadContent(type: _content.type, body: bytes), _schema);

            EvaluationResults result = null;
            if (!_ignoreValidation)
            {
                string schemaString = JsonConvert.SerializeObject(_schema);
                string valueString = JsonConvert.SerializeObject(_value);

                var schema = JsonSchema.FromText(schemaString);
                var valueNode = JsonNode.Parse(valueString);
                result = schema.Evaluate(valueNode);
            }


            if (result == null || result.IsValid)
            {
                if (bytes.Length > 0)
                {
                    return _value;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new EvalError("Invalid value according to DataSchema" + result.Errors);
            }
        }

        public async Task<IInteractionOutputValue<T>> ValueInContainer()
        {
            if (_schema == null)
            {
                Console.WriteLine("No schema defined. Hence null is reported for Value() function.If you are invoking an action with no output that is on purpose, otherwise consider using arrayBuffer().");
                return new InteractionOutputValue<T> { IsEmpty = true, Value = default };
            }

            if (_value != null && !_value.Equals(default(T))) { return new InteractionOutputValue<T>() { IsEmpty = false, Value = _value }; }

            if (_dataUsed)
            {
                throw new NotReadableError("Can't read the stream once it has been already used");
            }

            if (_form == null)
            {
                throw new NotReadableError("No form defined");
            }

            if (_schema.Const == null && _schema.Enum == null && _schema.OneOf == null && _schema.Type == null)
            {
                throw new NotReadableError("No schema type defined");
            }

            if (!ContentSerdes.GetInstance().IsSupported(_content.type))
            {
                throw new NotSupportedError($"Content type {_content.type} not supported");
            }

            var bytes = await _content.ToBuffer();
            _dataUsed = true;
            _buffer = bytes;

            _value = ContentSerdes.GetInstance().ContentToValue<T>(new ReadContent(type: _content.type, body: bytes), _schema);

            EvaluationResults result = null;
            if (!_ignoreValidation)
            {
                string schemaString = JsonConvert.SerializeObject(_schema);
                string valueString = JsonConvert.SerializeObject(_value);

                var schema = JsonSchema.FromText(schemaString);
                var valueNode = JsonNode.Parse(valueString);
                result = schema.Evaluate(valueNode);
            }


            if (result == null || result.IsValid)
            {
                if (bytes.Length > 0)
                {
                    return new InteractionOutputValue<T> { IsEmpty = false, Value = _value };
                }
                else
                {
                    return new InteractionOutputValue<T> { IsEmpty = true, Value = default };
                }
            }
            else
            {
                throw new EvalError("Invalid value according to DataSchema" + result.Errors);
            }
        }
    }
}
