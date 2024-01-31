using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WoT.Definitions;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json.Schema;

namespace WoT.TDHelpers
{

    //Converter to assign the corresponding DataSchema for a given schema
    public class DataSchemaConverter : JsonConverter<DataSchema>
    {

        public override void WriteJson(JsonWriter writer, DataSchema value, JsonSerializer serializer)
        {
            JObject jo = new JObject();
            if (value.AtType != null )          jo.Add("@type", JToken.FromObject(value.AtType));
            if (value.Title != null)            jo.Add("title",  JToken.FromObject(value.Title));
            if (value.Description != null)      jo.Add("description", JToken.FromObject(value.Description));
            if (value.Descriptions != null)     jo.Add("descriptions", JToken.FromObject(value.Descriptions));
            if (value.Const != null)            jo.Add("const", JToken.FromObject(value.Const));
            if (value.Default != null)          jo.Add("default", JToken.FromObject(value.Default));
            if (value.Unit != null)             jo.Add("unit", JToken.FromObject(value.Unit));
            if (value.OneOf != null)            jo.Add("oneOf", JToken.FromObject(value.OneOf));
            if (value.AllOf != null)            jo.Add("allOf", JToken.FromObject(value.AllOf));
            if (value.Enum != null)             jo.Add("enum", JToken.FromObject(value.Enum));
            if (value.Format != null)           jo.Add("format", JToken.FromObject(value.Format));
            if (value.Type != null)             jo.Add("type", JToken.FromObject(value.Type));
            jo.Add("readOnly", JToken.FromObject(value.ReadOnly));
            jo.Add("writeOnly", JToken.FromObject(value.WriteOnly));
            switch (value.Type)
            {
                case "object":
                    if ((value as ObjectSchema).Properties != null) jo.Add("items", JToken.FromObject((value as ObjectSchema).Properties));
                    if ((value as ObjectSchema).Required != null)   jo.Add("minItems", JToken.FromObject((value as ObjectSchema).Required));
                    break;
                case "array":
                    if ((value as ArraySchema).Items != null)       jo.Add("items", JToken.FromObject((value as ArraySchema).Items));
                    if ((value as ArraySchema).MinItems != null)    jo.Add("minItems", JToken.FromObject((value as ArraySchema).MinItems));
                    if ((value as ArraySchema).MaxItems != null)    jo.Add("maxItems", JToken.FromObject((value as ArraySchema).MaxItems));
                    break;
                case "string":
                    if ((value as StringSchema).MinLength != null)          jo.Add("minimum", JToken.FromObject((value as StringSchema).MinLength));
                    if ((value as StringSchema).MaxLength != null)          jo.Add("exclusiveMinimum", JToken.FromObject((value as StringSchema).MaxLength));
                    if ((value as StringSchema).Pattern != null)            jo.Add("maximum", JToken.FromObject((value as StringSchema).Pattern));
                    if ((value as StringSchema).ContentEncoding != null)    jo.Add("exclusiveMaximum", JToken.FromObject((value as StringSchema).ContentEncoding));
                    if ((value as StringSchema).ContentMediaType != null)   jo.Add("multipleOf", JToken.FromObject((value as StringSchema).ContentMediaType));
                    break;
                case "boolean":
                    break;
                case "number":
                    if ((value as NumberSchema).Minimum != null)            jo.Add("minimum", JToken.FromObject((value as NumberSchema).Minimum));
                    if ((value as NumberSchema).ExclusiveMinimum != null)   jo.Add("exclusiveMinimum", JToken.FromObject((value as NumberSchema).ExclusiveMinimum));
                    if ((value as NumberSchema).Maximum != null)            jo.Add("maximum", JToken.FromObject((value as NumberSchema).Maximum));
                    if ((value as NumberSchema).ExclusiveMaximum != null)   jo.Add("exclusiveMaximum", JToken.FromObject((value as NumberSchema).ExclusiveMaximum));
                    if ((value as NumberSchema).MultipleOf != null)         jo.Add("multipleOf", JToken.FromObject((value as NumberSchema).MultipleOf));
                    break;
                case "integer":
                    if ((value as IntegerSchema).Minimum != null)           jo.Add("minimum", JToken.FromObject((value as IntegerSchema).Minimum));
                    if ((value as IntegerSchema).ExclusiveMinimum != null)  jo.Add("exclusiveMinimum", JToken.FromObject((value as IntegerSchema).ExclusiveMinimum));
                    if ((value as IntegerSchema).Maximum != null)           jo.Add("maximum", JToken.FromObject((value as IntegerSchema).Maximum));
                    if ((value as IntegerSchema).ExclusiveMaximum != null)  jo.Add("exclusiveMaximum", JToken.FromObject((value as IntegerSchema).ExclusiveMaximum));
                    if ((value as IntegerSchema).MultipleOf != null)        jo.Add("multipleOf", JToken.FromObject((value as IntegerSchema).MultipleOf));
                    break;
                case "null":
                    break;
                default:
                    break;
            };
            jo.WriteTo(writer);
        }

        public override DataSchema ReadJson(JsonReader reader, Type objectType, DataSchema existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JsonReader reader2 = reader;
            JToken schemaObj = JToken.Load(reader2);
            string type = (string)schemaObj["type"]; //type of the given schema

            switch(type)
            {
                case "object":
                    return FillObjectSchemaObject(schemaObj, serializer);
                case "array":
                    return FillArraySchemaObject(schemaObj, serializer);
                case "string":
                    return FillStringSchemaObject(schemaObj, serializer);
                case "boolean":
                    return FillBooleanSchemaObject(schemaObj, serializer);
                case "number":
                    return FillNumberSchemaObject(schemaObj, serializer);
                case "integer":
                    return FillIntegerSchemaObject(schemaObj, serializer);
                case "null":
                    return FillNullSchemaObject(schemaObj, serializer);
                default:
                    return FillDataSchemaObject(schemaObj, serializer);
            };
        }

        protected static DataSchema FillDataSchemaObject(JToken schemaObj, JsonSerializer serializer)
        {
            DataSchema schema = new DataSchema();
            CommonFiller(schema, schemaObj, serializer);

            return schema;
        }

        protected static NumberSchema FillNumberSchemaObject(JToken schemaObj, JsonSerializer serializer)
        {
            NumberSchema schema = new NumberSchema();
            CommonFiller(schema, schemaObj, serializer);


            if (schemaObj["minimum"] != null) schema.Minimum = (double)schemaObj["minimum"];
            if (schemaObj["maximum"] != null) schema.Maximum = (double)schemaObj["maximum"];
            if (schemaObj["multipleOf"] != null) schema.MultipleOf = (double)schemaObj["multipleOf"];
            if (schemaObj["exclusiveMinimum"] != null) schema.ExclusiveMinimum = (double)schemaObj["exclusiveMinimum"];
            if (schemaObj["exclusiveMaximum"] != null) schema.ExclusiveMaximum = (double)schemaObj["exclusiveMaximum"];

            // to make sure const, default, and enum types match the schema type they belong to
            if (schemaObj["const"] != null) schema.Const = schemaObj["const"].ToObject<double>();
            if (schemaObj["default"] != null) schema.Default = schemaObj["default"].ToObject<double>();
            if (schemaObj["enum"] != null) schema.Enum = schemaObj["enum"].ToObject<double[]>();


            return schema;
        }

        protected static ArraySchema FillArraySchemaObject(JToken schemaObj, JsonSerializer serializer)
        {
            ArraySchema schema = new ArraySchema();
            CommonFiller(schema, schemaObj, serializer);

            if (schemaObj["minItems"] != null) schema.MinItems = (uint)schemaObj["minItems"];
            if (schemaObj["maxItems"] != null) schema.MaxItems = (uint)schemaObj["maxItems"];

            if (schemaObj["items"] != null)
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new DataSchemaTypeConverter());
                var newSerializer = JsonSerializer.CreateDefault(settings);
                schema.Items = newSerializer.Deserialize<DataSchema[]>(new JTokenReader(schemaObj["items"]));
            }


            if (schemaObj["const"] != null) schema.Const = schemaObj["const"];
            if (schemaObj["default"] != null) schema.Default = schemaObj["default"];
            if (schemaObj["enum"] != null) schema.Enum = schemaObj["enum"].ToObject<Object[]>();

            return schema;
        }

        protected static BooleanSchema FillBooleanSchemaObject(JToken schemaObj, JsonSerializer serializer)
        {
            BooleanSchema schema = new BooleanSchema();
            CommonFiller(schema, schemaObj, serializer);

            // to make sure const, default, and enum types match the schema type they belong to
            if (schemaObj["const"] != null) schema.Const = schemaObj["const"].ToObject<bool>();
            if (schemaObj["default"] != null) schema.Default = schemaObj["default"].ToObject<bool>();
            if (schemaObj["enum"] != null) schema.Enum = schemaObj["enum"].ToObject<bool[]>();

            return schema;
        }

        protected static IntegerSchema FillIntegerSchemaObject(JToken schemaObj, JsonSerializer serializer)
        {
            IntegerSchema schema = new IntegerSchema();

            CommonFiller(schema, schemaObj, serializer);


            if (schemaObj["minimum"] != null) schema.Minimum = (int)schemaObj["minimum"];
            if (schemaObj["maximum"] != null) schema.Maximum = (int)schemaObj["maximum"];
            if (schemaObj["multipleOf"] != null) schema.MultipleOf = (int)schemaObj["multipleOf"];
            if (schemaObj["exclusiveMinimum"] != null) schema.ExclusiveMinimum = (int)schemaObj["exclusiveMinimum"];
            if (schemaObj["exclusiveMaximum"] != null) schema.ExclusiveMaximum = (int)schemaObj["exclusiveMaximum"];

            // to make sure const, default, and enum types match the schema type they belong to
            if (schemaObj["const"] != null) schema.Const = schemaObj["const"].ToObject<int>();
            if (schemaObj["default"] != null) schema.Default = schemaObj["default"].ToObject<int>();
            if (schemaObj["enum"] != null) schema.Enum = schemaObj["enum"].ToObject<int[]>();

            return schema;
        }

        protected static ObjectSchema FillObjectSchemaObject(JToken schemaObj, JsonSerializer serializer)
        {
            ObjectSchema schema = new ObjectSchema();
            CommonFiller(schema, schemaObj, serializer);

            if (schemaObj["required"] != null) schema.Required = schemaObj["required"].ToObject<string[]>();
            if (schemaObj["properties"] != null) schema.Properties = serializer.Deserialize(new JTokenReader(schemaObj["properties"]), objectType: typeof(Dictionary<string, DataSchema>)) as Dictionary<string, DataSchema>;

            if (schemaObj["const"] != null) schema.Const = schemaObj["const"];
            if (schemaObj["default"] != null) schema.Default = schemaObj["default"];
            if (schemaObj["enum"] != null) schema.Enum = schemaObj["enum"].ToObject<Object[]>();

            return schema;
        }

        protected static StringSchema FillStringSchemaObject(JToken schemaObj, JsonSerializer serializer)
        {
            StringSchema schema = new StringSchema();
            CommonFiller(schema, schemaObj, serializer);

            schema.Pattern = (string)schemaObj["pattern"];
            schema.ContentEncoding = (string)schemaObj["contentEncoding"];
            schema.ContentMediaType = (string)schemaObj["contentMediaType"];

            if (schemaObj["minLength"] != null) schema.MinLength = (uint)schemaObj["minLength"];
            if (schemaObj["maxLength"] != null) schema.MaxLength = (uint)schemaObj["maxLength"];

            // to make sure const, default, and enum types match the schema type they belong to
            if (schemaObj["const"] != null) schema.Const = schemaObj["const"].ToObject<string>();
            if (schemaObj["default"] != null) schema.Default = schemaObj["default"].ToObject<string>();
            if (schemaObj["enum"] != null) schema.Enum = schemaObj["enum"].ToObject<string[]>();

            return schema;
        }

        protected static NullSchema FillNullSchemaObject(JToken schemaObj, JsonSerializer serializer)
        {
            NullSchema schema = new NullSchema();
            CommonFiller(schema, schemaObj, serializer);


            // to make sure const, default, and enum types match the schema type they belong to
            if (schemaObj["const"] != null) schema.Const = schemaObj["const"];
            if (schemaObj["default"] != null) schema.Default = schemaObj["default"];
            if (schemaObj["enum"] != null) schema.Enum = schemaObj["enum"].ToObject<Object[]>();
            return schema;
        }

        // to fill the common properties of DataSchemas
        protected static void CommonFiller<T>(T schema, JToken schemaObj, JsonSerializer serializer) where T: DataSchema
        {
            schema.Description = (string)schemaObj["description"];
            schema.Title = (string)schemaObj["title"];
            schema.Unit = (string)schemaObj["unit"];
            schema.Format = (string)schemaObj["format"];
            if (schemaObj["readOnly"] != null) schema.ReadOnly = (bool)schemaObj["readOnly"];
            if (schemaObj["writeOnly"] != null) schema.WriteOnly = (bool)schemaObj["writeOnly"];
            if (schemaObj["titles"] != null) schema.Titles = schemaObj["titles"].ToObject<MultiLanguage>();
            if (schemaObj["descriptions"] != null) schema.Descriptions = schemaObj["descriptions"].ToObject<MultiLanguage>();
            if (schemaObj["allOf"] != null) schema.AllOf = serializer.Deserialize(new JTokenReader(schemaObj["allOf"]), objectType: typeof(DataSchema[])) as DataSchema[];
            if (schemaObj["oneOf"] != null) schema.OneOf = serializer.Deserialize(new JTokenReader(schemaObj["oneOf"]), objectType: typeof(DataSchema[])) as DataSchema[];

            if (schemaObj["@type"] != null)
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringTypeConverter());
                var newSerializer = JsonSerializer.CreateDefault(settings);
                schema.AtType = newSerializer.Deserialize<string[]>(new JTokenReader(schemaObj["@type"]));
            }
        }
    }

    //Converter to assign the corresponding SecurityScheme for a given schema
    public class SecuritySchemeConverter : JsonConverter<SecurityScheme>
    {

        public override void WriteJson(JsonWriter writer, SecurityScheme value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override SecurityScheme ReadJson(JsonReader reader, Type objectType, SecurityScheme existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JToken schemaObj = JToken.Load(reader);
            if (schemaObj.Type == JTokenType.Null) return null;
            string type = (string)schemaObj["scheme"];

            switch (type)
            {
                case "nosec":
                    return FillNoSecuritySchemeObject(schemaObj);
                case "basic":
                    return FillBasicSecuritySchemeObject(schemaObj);
            }
            return null;
        }

        protected static NoSecurityScheme FillNoSecuritySchemeObject(JToken schemaObj)
        {
            NoSecurityScheme schema = new NoSecurityScheme();
            CommonFiller(schema, schemaObj);
            return schema;
        }

        protected static BasicSecurityScheme FillBasicSecuritySchemeObject(JToken schemaObj)
        {
            BasicSecurityScheme schema = new BasicSecurityScheme();
            CommonFiller(schema, schemaObj);


            //assigning default value
            schema.Name = (string)schemaObj["name"];
            if (schemaObj["in"] != null)
            {
                schema.In = (string)schemaObj["in"];
            }
            else
            {
                schema.In = "header";
            }

            return schema;
        }

        // to fill the common properties of SecuritySchemas
        protected static void CommonFiller<T>(T schema, JToken schemaObj) where T: SecurityScheme
        {
            schema.Description = (string)schemaObj["description"];
            schema.Scheme = (string)schemaObj["scheme"];

            if (schemaObj["@type"] != null)
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringTypeConverter());
                var newSerializer = JsonSerializer.CreateDefault(settings);
                schema.AtType = newSerializer.Deserialize<string[]>(new JTokenReader(schemaObj["@type"]));
            }

            if (schemaObj["descriptions"] != null) schema.Descriptions = schemaObj["descriptions"].ToObject<MultiLanguage>();

            if (schemaObj["proxy"] != null)
            {

                Uri temp = new Uri(schemaObj["proxy"].ToObject<string>(), UriKind.RelativeOrAbsolute);
                schema.Proxy = temp;
            }



        }

    }

    //Converter for Form in TD
    public class FormConverter : JsonConverter<Form>
    {

        public override void WriteJson(JsonWriter writer, Form value, JsonSerializer serializer)
        {
            var jo = JObject.Parse(value.OriginalJson);
            jo.WriteTo(writer);
        }

        public override Form ReadJson(JsonReader reader, Type objectType, Form existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            //add default converter 
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringTypeConverter());
            var newSerializer = JsonSerializer.CreateDefault(settings);


            JToken schemaObj = JToken.Load(reader);
            if (schemaObj.Type == JTokenType.Null) return null;

            Form schema = new Form();

            if (schemaObj["href"] != null)
            {

                Uri temp = new Uri(schemaObj["href"].ToObject<string>(), UriKind.RelativeOrAbsolute);
                schema.Href = temp;
            }

            if (schemaObj["contentType"] != null)
            { schema.ContentType = schemaObj["contentType"].ToObject<string>(); }
            else //Default value handling
            {
                schema.ContentType = "application/json";
            }
            schema.OriginalJson = schemaObj.ToString();
            if (schemaObj["contentCoding"] != null) schema.ContentCoding = schemaObj["contentCoding"].ToObject<string>();
            if (schemaObj["subprotocol"] != null) schema.Subprotocol = schemaObj["subprotocol"].ToObject<string>();
            if (schemaObj["additionalExpectedResponse"] != null)
            {
                schema.AdditionalExpectedResponse = schemaObj["additionalExpectedResponse"].ToObject<AdditionalExpectedResponse>();
            }
            else
            { //Default value handling
                string contentType = schema.ContentType;
                schema.AdditionalExpectedResponse = new AdditionalExpectedResponse(contentType);
            }


            if (schemaObj["security"] != null) schema.Security = newSerializer.Deserialize<string[]>(new JTokenReader(schemaObj["security"]));

            if (schemaObj["scopes"] != null) schema.Scopes = newSerializer.Deserialize<string[]>(new JTokenReader(schemaObj["scopes"]));

            if (schemaObj["op"] != null) schema.Op = newSerializer.Deserialize<string[]>(new JTokenReader(schemaObj["op"]));



            return schema;
        }

    }

    /// <summary>
    /// Converter for PropertyForm
    /// </summary>
    public class PropertyFormConverter : JsonConverter<PropertyForm>
    {

        public override void WriteJson(JsonWriter writer, PropertyForm value, JsonSerializer serializer)
        {
            var jo = JObject.Parse(value.OriginalJson);
            jo.WriteTo(writer);
        }

        public override PropertyForm ReadJson(JsonReader reader, Type objectType, PropertyForm existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            //add default converter 
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringTypeConverter());
            var newSerializer = JsonSerializer.CreateDefault(settings);

            JToken schemaObj = JToken.Load(reader);

            if (schemaObj.Type == JTokenType.Null) return null;

            PropertyForm schema = new PropertyForm();

            if (schemaObj["href"] != null)
            {

                Uri temp = new Uri(schemaObj["href"].ToObject<string>(), UriKind.RelativeOrAbsolute);
                schema.Href = temp;
            }

            if (schemaObj["contentType"] != null)
            { schema.ContentType = schemaObj["contentType"].ToObject<string>(); }
            else
            {
                //Default value handling
                schema.ContentType = "application/json";
            }
            if (schemaObj["contentCoding"] != null) schema.ContentCoding = schemaObj["contentCoding"].ToObject<string>();
            if (schemaObj["subprotocol"] != null) schema.Subprotocol = schemaObj["subprotocol"].ToObject<string>();

            if (schemaObj["additionalExpectedResponse"] != null)
            {
                schema.AdditionalExpectedResponse = schemaObj["additionalExpectedResponse"].ToObject<AdditionalExpectedResponse>();
            }
            else
            {//Default value handling
                string contentType = schema.ContentType;
                schema.AdditionalExpectedResponse = new AdditionalExpectedResponse(contentType);
            }

            schema.OriginalJson = schemaObj.ToString();

            if (schemaObj["security"] != null) schema.Security = newSerializer.Deserialize<string[]>(new JTokenReader(schemaObj["security"]));

            if (schemaObj["scopes"] != null) schema.Scopes = newSerializer.Deserialize<string[]>(new JTokenReader(schemaObj["scopes"]));

            if (schemaObj["op"] == null)
            { //default value handling

                //TODO: Default value of op needs to be handled here
            }
            else
            {
                schema.Op = newSerializer.Deserialize<string[]>(new JTokenReader(schemaObj["op"]));
            }

            return schema;
        }

    }

    //Converter for ActionForm 
    public class ActionFormConverter : JsonConverter<ActionForm>
    {

        public override void WriteJson(JsonWriter writer, ActionForm value, JsonSerializer serializer)
        {
            var jo = JObject.Parse(value.OriginalJson);
            jo.WriteTo(writer);
        }

        public override ActionForm ReadJson(JsonReader reader, Type objectType, ActionForm existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            //add default converter 
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringTypeConverter());
            var newSerializer = JsonSerializer.CreateDefault(settings);

            JToken schemaObj = JToken.Load(reader);

            if (schemaObj.Type == JTokenType.Null) return null;

            ActionForm schema = new ActionForm();

            if (schemaObj["href"] != null)
            {

                Uri temp = new Uri(schemaObj["href"].ToObject<string>(), UriKind.RelativeOrAbsolute);
                schema.Href = temp;
            }

            if (schemaObj["contentType"] != null)
            { schema.ContentType = schemaObj["contentType"].ToObject<string>(); }
            else
            {//Default value handling
                schema.ContentType = "application/json";
            }
            if (schemaObj["contentCoding"] != null) schema.ContentCoding = schemaObj["contentCoding"].ToObject<string>();
            if (schemaObj["subprotocol"] != null) schema.Subprotocol = schemaObj["subprotocol"].ToObject<string>();

            if (schemaObj["additionalExpectedResponse"] != null)
            {
                schema.AdditionalExpectedResponse = schemaObj["additionalExpectedResponse"].ToObject<AdditionalExpectedResponse>();
            }
            else
            {//Default value handling
                string contentType = schema.ContentType;
                schema.AdditionalExpectedResponse = new AdditionalExpectedResponse(contentType);
            }

            schema.OriginalJson = schemaObj.ToString();

            if (schemaObj["security"] != null) schema.Security = newSerializer.Deserialize<string[]>(new JTokenReader(schemaObj["security"]));

            if (schemaObj["scopes"] != null) schema.Scopes = newSerializer.Deserialize<string[]>(new JTokenReader(schemaObj["scopes"]));

            if (schemaObj["op"] == null)
            { //default value handling

                string tmp = "invokeaction";
                schema.Op = new string[1];
                schema.Op[0] = tmp;
            }
            else
            {
                schema.Op = newSerializer.Deserialize<string[]>(new JTokenReader(schemaObj["op"]));
            }

            return schema;
        }

    }

    //Converter for EventForm 
    public class EventFormConverter : JsonConverter<EventForm>
    {

        public override void WriteJson(JsonWriter writer, EventForm value, JsonSerializer serializer)
        {
            var jo = JObject.Parse(value.OriginalJson);
            jo.WriteTo(writer);
        }

        public override EventForm ReadJson(JsonReader reader, Type objectType, EventForm existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            //add default converter 
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringTypeConverter());
            var newSerializer = JsonSerializer.CreateDefault(settings);


            JToken schemaObj = JToken.Load(reader);

            if (schemaObj.Type == JTokenType.Null) return null;

            EventForm schema = new EventForm();


            if (schemaObj["href"] != null)
            {

                Uri temp = new Uri(schemaObj["href"].ToObject<string>(), UriKind.RelativeOrAbsolute);
                schema.Href = temp;
            }

            if (schemaObj["contentType"] != null)
            { schema.ContentType = schemaObj["contentType"].ToObject<string>(); }
            else
            {//Default value handling
                schema.ContentType = "application/json";
            }
            if (schemaObj["contentCoding"] != null) schema.ContentCoding = schemaObj["contentCoding"].ToObject<string>();
            if (schemaObj["subprotocol"] != null) schema.Subprotocol = schemaObj["subprotocol"].ToObject<string>();

            if (schemaObj["additionalExpectedResponse"] != null)
            {
                schema.AdditionalExpectedResponse = schemaObj["additionalExpectedResponse"].ToObject<AdditionalExpectedResponse>();
            }
            else
            {//Default value handling
                string contentType = schema.ContentType;
                schema.AdditionalExpectedResponse = new AdditionalExpectedResponse(contentType);
            }

            schema.OriginalJson = schemaObj.ToString();

            if (schemaObj["security"] != null) schema.Security = newSerializer.Deserialize<string[]>(new JTokenReader(schemaObj["security"]));

            if (schemaObj["scopes"] != null) schema.Scopes = newSerializer.Deserialize<string[]>(new JTokenReader(schemaObj["scopes"]));

            if (schemaObj["op"] == null)
            { //default value handling

                string[] temp = { "subscribeevent", "unsubscribeevent" };
                schema.Op = temp;
            }
            else
            {
                schema.Op = newSerializer.Deserialize<string[]>(new JTokenReader(schemaObj["op"]));
            }

            return schema;
        }

    }

    //Converter to assign the correct type for a given @contex
    public class AtContextConverter : JsonConverter<object[]>
    {

        public override void WriteJson(JsonWriter writer, object[] value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);
            serializer.Serialize(writer, t);
        }

        public override object[] ReadJson(JsonReader reader, Type objectType, object[] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JToken schemaObj = JToken.Load(reader);
            if (schemaObj.Type == JTokenType.Null) return null;
            var type = schemaObj.Type;

            switch (type.ToString())
            {
                case "Array":
                    return AtContextTypeArrayFiller(schemaObj);
                case "String":
                    return AtContextTypeUriFiller(schemaObj);

            }
            return null;

        }



        protected static object[] AtContextTypeArrayFiller(JToken schemaObj)
        {
            object[] schema = schemaObj.ToObject<object[]>();
            return schema;
        }

        protected static object[] AtContextTypeUriFiller(JToken schemaObj)
        {

            object[] schema = new object[1];

            if (schemaObj != null)
            {
                Uri temp = new Uri(schemaObj.ToObject<string>(), UriKind.RelativeOrAbsolute);
                schema[0] = temp;
            }
            return schema;
        }

    }

    //Called for properties that can be either string or string[], and it returns string[].
    public class StringTypeConverter : JsonConverter<string[]>
    {

        public override void WriteJson(JsonWriter writer, string[] value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);
            t.WriteTo(writer);
        }

        public override string[] ReadJson(JsonReader reader, Type objectType, string[] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JToken schemaObj = JToken.Load(reader);
            if (schemaObj.Type == JTokenType.Null) return null;
            var type = schemaObj.Type;

            switch (type.ToString())
            {
                case "Array":
                    return ArrayFiller(schemaObj);
                case "String":
                    return StringFiller(schemaObj);

            }
            return null;

        }

        protected static string[] ArrayFiller(JToken schemaObj)
        {
            string[] schema = schemaObj.ToObject<string[]>();
            return schema;

        }

        protected static string[] StringFiller(JToken schemaObj)
        {
            string[] schema = new string[1];
            string tmp = schemaObj.ToObject<string>();
            schema[0] = tmp;

            return schema;
        }
    }

    //Called for properties that can be either DataSchema or DataSchema[], and it returns DataSchema[].
    public class DataSchemaTypeConverter : JsonConverter<DataSchema[]>
    {

        public override void WriteJson(JsonWriter writer, DataSchema[] value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);
            serializer.Serialize(writer, t);
        }

        public override DataSchema[] ReadJson(JsonReader reader, Type objectType, DataSchema[] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JToken schemaObj = JToken.Load(reader);
            if (schemaObj.Type == JTokenType.Null) return null;
            var type = schemaObj.Type;

            switch (type.ToString())
            {
                case "Object":
                    return ObjectFiller(schemaObj);
                case "Array":
                    return ArrayFiller(schemaObj);

            }
            return null;

        }



        protected static DataSchema[] ArrayFiller(JToken schemaObj)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new DataSchemaConverter());
            var newSerializer = JsonSerializer.CreateDefault(settings);
            var schema = newSerializer.Deserialize(new JTokenReader(schemaObj), objectType: typeof(DataSchema[])) as DataSchema[];

            return schema;

        }

        protected static DataSchema[] ObjectFiller(JToken schemaObj)
        {
            DataSchema[] schema = new DataSchema[1];

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new DataSchemaConverter());
            var newSerializer = JsonSerializer.CreateDefault(settings);

            schema[0] = newSerializer.Deserialize(new JTokenReader(schemaObj), objectType: typeof(DataSchema)) as DataSchema;

            return schema;


        }
    }
    public class PropertyAffordanceConverter : JsonConverter<PropertyAffordance>
    {
        public override void WriteJson(JsonWriter writer, PropertyAffordance value, JsonSerializer serializer)
        {
            var jo = JObject.Parse(value.OriginalJson);
            jo.WriteTo(writer);
        }

        public override PropertyAffordance ReadJson(JsonReader reader, Type objectType, PropertyAffordance existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JsonReader reader2 = reader;
            JToken propertyObj = JToken.Load(reader2);
            string type = (string)propertyObj["type"]; //type of the given schema

            switch (type)
            {
                case "object":
                    return FillObjectPropertyAffordanceObject(propertyObj, serializer);
                case "array":
                    return FillArrayPropertyAffordanceObject(propertyObj, serializer);
                case "string":
                    return FillStringPropertyAffordanceObject(propertyObj, serializer);
                case "boolean":
                    return FillBooleanPropertyAffordanceObject(propertyObj, serializer);
                case "number":
                    return FillNumberPropertyAffordanceObject(propertyObj, serializer);
                case "integer":
                    return FillIntegerPropertyAffordanceObject(propertyObj, serializer);
                case "null":
                    return FillNullPropertyAffordanceObject(propertyObj, serializer);
                default:
                    return FillPropertyAffordanceObject(propertyObj, serializer);
            };
        }

        protected static PropertyAffordance FillPropertyAffordanceObject(JToken propObj, JsonSerializer serializer)
        {
            PropertyAffordance propertyAffordance = new PropertyAffordance();
            CommonFiller(propertyAffordance, propObj, serializer);

            return propertyAffordance;
        }
        protected static NumberPropertyAffordance FillNumberPropertyAffordanceObject(JToken propObj, JsonSerializer serializer)
        {
            NumberPropertyAffordance schema = new NumberPropertyAffordance();
            CommonFiller(schema, propObj, serializer);


            if (propObj["minimum"] != null) schema.Minimum = (double)propObj["minimum"];
            if (propObj["maximum"] != null) schema.Maximum = (double)propObj["maximum"];
            if (propObj["multipleOf"] != null) schema.MultipleOf = (double)propObj["multipleOf"];
            if (propObj["exclusiveMinimum"] != null) schema.ExclusiveMinimum = (double)propObj["exclusiveMinimum"];
            if (propObj["exclusiveMaximum"] != null) schema.ExclusiveMaximum = (double)propObj["exclusiveMaximum"];

            // to make sure const, default, and enum types match the schema type they belong to
            if (propObj["const"] != null) schema.Const = propObj["const"].ToObject<double>();
            if (propObj["default"] != null) schema.Default = propObj["default"].ToObject<double>();
            if (propObj["enum"] != null) schema.Enum = propObj["enum"].ToObject<double[]>();


            return schema;
        }

        protected static ArrayPropertyAffordance FillArrayPropertyAffordanceObject(JToken propObj, JsonSerializer serializer)
        {
            ArrayPropertyAffordance schema = new ArrayPropertyAffordance();
            CommonFiller(schema, propObj, serializer);

            if (propObj["minItems"] != null) schema.MinItems = (uint)propObj["minItems"];
            if (propObj["maxItems"] != null) schema.MaxItems = (uint)propObj["maxItems"];

            if (propObj["items"] != null)
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new DataSchemaTypeConverter());
                var newSerializer = JsonSerializer.CreateDefault(settings);
                schema.Items = newSerializer.Deserialize<DataSchema[]>(new JTokenReader(propObj["items"]));
            }


            if (propObj["const"] != null) schema.Const = propObj["const"];
            if (propObj["default"] != null) schema.Default = propObj["default"];
            if (propObj["enum"] != null) schema.Enum = propObj["enum"].ToObject<Object[]>();

            return schema;
        }

        protected static BooleanPropertyAffordance FillBooleanPropertyAffordanceObject(JToken propObj, JsonSerializer serializer)
        {
            BooleanPropertyAffordance schema = new BooleanPropertyAffordance();
            CommonFiller(schema, propObj, serializer);

            // to make sure const, default, and enum types match the schema type they belong to
            if (propObj["const"] != null) schema.Const = propObj["const"].ToObject<bool>();
            if (propObj["default"] != null) schema.Default = propObj["default"].ToObject<bool>();
            if (propObj["enum"] != null) schema.Enum = propObj["enum"].ToObject<bool[]>();

            return schema;
        }

        protected static IntegerPropertyAffordance FillIntegerPropertyAffordanceObject(JToken propObj, JsonSerializer serializer)
        {
            IntegerPropertyAffordance schema = new IntegerPropertyAffordance();

            CommonFiller(schema, propObj, serializer);


            if (propObj["minimum"] != null) schema.Minimum = (int)propObj["minimum"];
            if (propObj["maximum"] != null) schema.Maximum = (int)propObj["maximum"];
            if (propObj["multipleOf"] != null) schema.MultipleOf = (int)propObj["multipleOf"];
            if (propObj["exclusiveMinimum"] != null) schema.ExclusiveMinimum = (int)propObj["exclusiveMinimum"];
            if (propObj["exclusiveMaximum"] != null) schema.ExclusiveMaximum = (int)propObj["exclusiveMaximum"];

            // to make sure const, default, and enum types match the schema type they belong to
            if (propObj["const"] != null) schema.Const = propObj["const"].ToObject<int>();
            if (propObj["default"] != null) schema.Default = propObj["default"].ToObject<int>();
            if (propObj["enum"] != null) schema.Enum = propObj["enum"].ToObject<int[]>();

            return schema;
        }

        protected static ObjectPropertyAffordance FillObjectPropertyAffordanceObject(JToken propObj, JsonSerializer serializer)
        {
            ObjectPropertyAffordance schema = new ObjectPropertyAffordance();
            CommonFiller(schema, propObj, serializer);

            if (propObj["required"] != null) schema.Required = propObj["required"].ToObject<string[]>();
            if (propObj["properties"] != null) schema.Properties = serializer.Deserialize(new JTokenReader(propObj["properties"]), objectType: typeof(Dictionary<string, DataSchema>)) as Dictionary<string, DataSchema>;

            if (propObj["const"] != null) schema.Const = propObj["const"];
            if (propObj["default"] != null) schema.Default = propObj["default"];
            if (propObj["enum"] != null) schema.Enum = propObj["enum"].ToObject<Object[]>();

            return schema;
        }

        protected static StringPropertyAffordance FillStringPropertyAffordanceObject(JToken propObj, JsonSerializer serializer)
        {
            StringPropertyAffordance schema = new StringPropertyAffordance();
            CommonFiller(schema, propObj, serializer);

            schema.Pattern = (string)propObj["pattern"];
            schema.ContentEncoding = (string)propObj["contentEncoding"];
            schema.ContentMediaType = (string)propObj["contentMediaType"];

            if (propObj["minLength"] != null) schema.MinLength = (uint)propObj["minLength"];
            if (propObj["maxLength"] != null) schema.MaxLength = (uint)propObj["maxLength"];

            // to make sure const, default, and enum types match the schema type they belong to
            if (propObj["const"] != null) schema.Const = propObj["const"].ToObject<string>();
            if (propObj["default"] != null) schema.Default = propObj["default"].ToObject<string>();
            if (propObj["enum"] != null) schema.Enum = propObj["enum"].ToObject<string[]>();

            return schema;
        }

        protected static NullPropertyAffordance FillNullPropertyAffordanceObject(JToken propObj, JsonSerializer serializer)
        {
            NullPropertyAffordance schema = new NullPropertyAffordance();
            CommonFiller(schema, propObj, serializer);
            return schema;
        }

        // to fill the common properties of PropertyAffordance
        protected static void CommonFiller<T>(T propertyAffordance, JToken propObj, JsonSerializer serializer) where T: PropertyAffordance
        {
            propertyAffordance.Description = (string)propObj["description"];
            propertyAffordance.Title = (string)propObj["title"];
            propertyAffordance.Unit = (string)propObj["unit"];
            propertyAffordance.Format = (string)propObj["format"];
            if (propObj["readOnly"] != null) propertyAffordance.ReadOnly = (bool)propObj["readOnly"];
            if (propObj["observable"] != null) propertyAffordance.Observable = (bool)propObj["observable"];
            if (propObj["writeOnly"] != null) propertyAffordance.WriteOnly = (bool)propObj["writeOnly"];
            if (propObj["titles"] != null) propertyAffordance.Titles = propObj["titles"].ToObject<MultiLanguage>();
            if (propObj["descriptions"] != null) propertyAffordance.Descriptions = propObj["descriptions"].ToObject<MultiLanguage>();
            if (propObj["allOf"] != null) propertyAffordance.AllOf = serializer.Deserialize(new JTokenReader(propObj["allOf"]), objectType: typeof(DataSchema[])) as DataSchema[];
            if (propObj["oneOf"] != null) propertyAffordance.OneOf = serializer.Deserialize(new JTokenReader(propObj["oneOf"]), objectType: typeof(DataSchema[])) as DataSchema[];
            if (propObj["forms"] != null) propertyAffordance.Forms = serializer.Deserialize(new JTokenReader(propObj["forms"]), objectType: typeof(PropertyForm[])) as PropertyForm[];
            if (propObj["@type"] != null)
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringTypeConverter());
                var newSerializer = JsonSerializer.CreateDefault(settings);
                propertyAffordance.AtType = newSerializer.Deserialize<string[]>(new JTokenReader(propObj["@type"]));
            }
            propertyAffordance.OriginalJson = propObj.ToString();
        }
    }
    public class ActionAffordanceConverter : JsonConverter<ActionAffordance>
    {
        public override ActionAffordance ReadJson(JsonReader reader, Type objectType, ActionAffordance existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JToken actionAffordanceObj = JToken.Load(reader);
            if (actionAffordanceObj.Type == JTokenType.Null) return null;
            ActionAffordance actionAffordance = new ActionAffordance
            {
                Description = (string)actionAffordanceObj["description"],
                Title = (string)actionAffordanceObj["title"]
            };

            if (actionAffordanceObj["@type"] != null)
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringTypeConverter());
                var newSerializer = JsonSerializer.CreateDefault(settings);
                actionAffordance.AtType = newSerializer.Deserialize<string[]>(new JTokenReader(actionAffordanceObj["@type"]));
            }

            if (actionAffordanceObj["titles"] != null)          actionAffordance.Titles = actionAffordanceObj["titles"].ToObject<MultiLanguage>();
            if (actionAffordanceObj["descriptions"] != null)    actionAffordance.Descriptions = actionAffordanceObj["descriptions"].ToObject<MultiLanguage>();
            if (actionAffordanceObj["input"] != null)           actionAffordance.Input = serializer.Deserialize(new JTokenReader(actionAffordanceObj["input"]), objectType: typeof(DataSchema)) as DataSchema;
            if (actionAffordanceObj["output"] != null)          actionAffordance.Output = serializer.Deserialize(new JTokenReader(actionAffordanceObj["output"]), objectType: typeof(DataSchema)) as DataSchema;
            if (actionAffordanceObj["safe"] != null)            actionAffordance.Safe = actionAffordanceObj["safe"].ToObject<bool>();
            if (actionAffordanceObj["idempotent"] != null)      actionAffordance.Idempotent = actionAffordanceObj["idempotent"].ToObject<bool>();
            if (actionAffordanceObj["synchronous"] != null)     actionAffordance.Synchronous = actionAffordanceObj["synchronous"].ToObject<bool>();
            if (actionAffordanceObj["forms"] != null)           actionAffordance.Forms = serializer.Deserialize(new JTokenReader(actionAffordanceObj["forms"]), objectType: typeof(ActionForm[])) as ActionForm[];
            actionAffordance.OriginalJson = actionAffordanceObj.ToString();
            return actionAffordance;
        }

        public override void WriteJson(JsonWriter writer, ActionAffordance value, JsonSerializer serializer)
        {
            var jo = JObject.Parse(value.OriginalJson);
            jo.WriteTo(writer);
        }
    }
    public class EventAffordanceConverter : JsonConverter<EventAffordance>
    {
        public override EventAffordance ReadJson(JsonReader reader, Type objectType, EventAffordance existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JToken eventAffordanceObj = JToken.Load(reader);
            if (eventAffordanceObj.Type == JTokenType.Null) return null;
            EventAffordance eventAffordance = new EventAffordance
            {
                Description = (string)eventAffordanceObj["description"],
                Title = (string)eventAffordanceObj["title"]
            };

            if (eventAffordanceObj["@type"] != null)
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringTypeConverter());
                var newSerializer = JsonSerializer.CreateDefault(settings);
                eventAffordance.AtType = newSerializer.Deserialize<string[]>(new JTokenReader(eventAffordanceObj["@type"]));
            }
            if (eventAffordanceObj["titles"] != null)       eventAffordance.Titles = eventAffordanceObj["titles"].ToObject<MultiLanguage>();
            if (eventAffordanceObj["descriptions"] != null) eventAffordance.Descriptions = eventAffordanceObj["descriptions"].ToObject<MultiLanguage>();
            if (eventAffordanceObj["subscription"] != null) eventAffordance.Subscription = serializer.Deserialize(new JTokenReader(eventAffordanceObj["subscription"]), objectType: typeof(DataSchema)) as DataSchema;
            if (eventAffordanceObj["data"] != null)         eventAffordance.Data = serializer.Deserialize(new JTokenReader(eventAffordanceObj["data"]), objectType: typeof(DataSchema)) as DataSchema;
            if (eventAffordanceObj["dataResponse"] != null) eventAffordance.DataResponse = serializer.Deserialize(new JTokenReader(eventAffordanceObj["dataResponse"]), objectType: typeof(DataSchema)) as DataSchema;
            if (eventAffordanceObj["cancellation"] != null) eventAffordance.Cancellation = serializer.Deserialize(new JTokenReader(eventAffordanceObj["cancellation"]), objectType: typeof(DataSchema)) as DataSchema;
            if (eventAffordanceObj["forms"] != null)        eventAffordance.Forms = serializer.Deserialize(new JTokenReader(eventAffordanceObj["forms"]), objectType: typeof(EventForm[])) as EventForm[];
            eventAffordance.OriginalJson =                  eventAffordanceObj.ToString();
            return eventAffordance;
        }

        public override void WriteJson(JsonWriter writer, EventAffordance value, JsonSerializer serializer)
        {
            var jo = JObject.Parse(value.OriginalJson);
            jo.WriteTo(writer);
        }
    }
}


