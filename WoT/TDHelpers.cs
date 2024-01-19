using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WoT_Definitions;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace TDHelpers
{

    //Converter to assign the corresponding DataSchema for a given schema
    public class DataSchemaConverter : JsonConverter<DataSchema>
    {

        public override void WriteJson(JsonWriter writer, DataSchema value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override DataSchema ReadJson(JsonReader reader, Type objectType, DataSchema existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JsonReader reader2 = reader;
            JToken schemaObj = JToken.Load(reader2);
            if (schemaObj.Type == JTokenType.Null) return null;
            string type = (string)schemaObj["type"]; //type of the given schema

            switch (type)
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
            }
            return null;
        }

        public NumberSchema FillNumberSchemaObject(JToken schemaObj, JsonSerializer serializer)
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

        public ArraySchema FillArraySchemaObject(JToken schemaObj, JsonSerializer serializer)
        {
            ArraySchema schema = new ArraySchema();
            CommonFiller(schema, schemaObj, serializer);

            if (schemaObj["minItems"] != null) schema.MinItems = (uint)schemaObj["minItems"];
            if (schemaObj["maxItems"] != null) schema.MaxItems = (uint)schemaObj["maxItems"];

            if (schemaObj["items"] != null)
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new DataSchemaTypeConverter());
                serializer = JsonSerializer.CreateDefault(settings);
                schema.Items = serializer.Deserialize<DataSchema[]>(new JTokenReader(schemaObj["items"]));
            }


            if (schemaObj["const"] != null) schema.Const = schemaObj["const"];
            if (schemaObj["default"] != null) schema.Default = schemaObj["default"];
            if (schemaObj["enum"] != null) schema.Enum = schemaObj["enum"].ToObject<Object[]>();

            return schema;
        }

        public BooleanSchema FillBooleanSchemaObject(JToken schemaObj, JsonSerializer serializer)
        {
            BooleanSchema schema = new BooleanSchema();
            CommonFiller(schema, schemaObj, serializer);

            // to make sure const, default, and enum types match the schema type they belong to
            if (schemaObj["const"] != null) schema.Const = schemaObj["const"].ToObject<bool>();
            if (schemaObj["default"] != null) schema.Default = schemaObj["default"].ToObject<bool>();
            if (schemaObj["enum"] != null) schema.Enum = schemaObj["enum"].ToObject<bool[]>();

            return schema;
        }

        //To Do: Throw an error if there is something wrong 

        public IntegerSchema FillIntegerSchemaObject(JToken schemaObj, JsonSerializer serializer)
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

        public ObjectSchema FillObjectSchemaObject(JToken schemaObj, JsonSerializer serializer)
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

        public StringSchema FillStringSchemaObject(JToken schemaObj, JsonSerializer serializer)
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

        public NullSchema FillNullSchemaObject(JToken schemaObj, JsonSerializer serializer)
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
        public void CommonFiller(DataSchema schema, JToken schemaObj, JsonSerializer serializer)
        {
            schema.Description = (string)schemaObj["description"];
            schema.Title = (string)schemaObj["title"];
            schema.Unit = (string)schemaObj["unit"];
            schema.Format = (string)schemaObj["format"];
            if (schemaObj["readOnly"] != null) schema.ReadOnly = (bool)schemaObj["readOnly"];
            if (schemaObj["writeOnly"] != null) schema.WriteOnly = (bool)schemaObj["writeOnly"];
            if (schemaObj["titles"] != null) schema.Titles = schemaObj["titles"].ToObject<string[]>();
            if (schemaObj["descriptions"] != null) schema.Descriptions = schemaObj["descriptions"].ToObject<string[]>();
            if (schemaObj["allOf"] != null) schema.AllOf = serializer.Deserialize(new JTokenReader(schemaObj["allOf"]), objectType: typeof(DataSchema[])) as DataSchema[];
            if (schemaObj["oneOf"] != null) schema.OneOf = serializer.Deserialize(new JTokenReader(schemaObj["oneOf"]), objectType: typeof(DataSchema[])) as DataSchema[];

            if (schemaObj["@type"] != null)
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringTypeConverter());
                serializer = JsonSerializer.CreateDefault(settings);
                schema.AtType = serializer.Deserialize<string[]>(new JTokenReader(schemaObj["@type"]));
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
                    return FillNoSecuritySchemeObject(schemaObj, serializer);
                case "basic":
                    return FillBasicSecuritySchemeObject(schemaObj, serializer);
            }
            return null;
        }

        public NoSecurityScheme FillNoSecuritySchemeObject(JToken schemaObj, JsonSerializer serializer)
        {
            NoSecurityScheme schema = new NoSecurityScheme();
            CommonFiller(schema, schemaObj, serializer);
            return schema;
        }

        public BasicSecurityScheme FillBasicSecuritySchemeObject(JToken schemaObj, JsonSerializer serializer)
        {
            BasicSecurityScheme schema = new BasicSecurityScheme();
            CommonFiller(schema, schemaObj, serializer);


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
        public void CommonFiller(SecurityScheme schema, JToken schemaObj, JsonSerializer serializer)
        {
            schema.Description = (string)schemaObj["description"];
            schema.Scheme = (string)schemaObj["scheme"];

            if (schemaObj["@type"] != null)
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringTypeConverter());
                serializer = JsonSerializer.CreateDefault(settings);
                schema.AtType = serializer.Deserialize<string[]>(new JTokenReader(schemaObj["@type"]));
            }

            if (schemaObj["descriptions"] != null) schema.Descriptions = schemaObj["descriptions"].ToObject<string[]>();
            if (schemaObj["proxy"] != null) schema.Proxy = schemaObj["proxy"].ToObject<Uri>();

        }

    }

    //Converter for Form in TD
    public class FormConverter : JsonConverter<Form>
    {

        public override void WriteJson(JsonWriter writer, Form value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override Form ReadJson(JsonReader reader, Type objectType, Form existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            //add default converter 
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringTypeConverter());
            serializer = JsonSerializer.CreateDefault(settings);


            JToken schemaObj = JToken.Load(reader);
            if (schemaObj.Type == JTokenType.Null) return null;

            Form schema = new Form();

            if (schemaObj["href"] != null) schema.Href = schemaObj["href"].ToObject<Uri>();

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


            if (schemaObj["security"] != null) schema.Security = serializer.Deserialize<string[]>(new JTokenReader(schemaObj["security"]));

            if (schemaObj["scopes"] != null) schema.Scopes = serializer.Deserialize<string[]>(new JTokenReader(schemaObj["scopes"]));

            if (schemaObj["op"] != null) schema.Op = serializer.Deserialize<string[]>(new JTokenReader(schemaObj["op"]));



            return schema;
        }

    }

    //Converter for PropertyForm 
    public class PropertyFormConverter : JsonConverter<PropertyForm>
    {

        public override void WriteJson(JsonWriter writer, PropertyForm value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override PropertyForm ReadJson(JsonReader reader, Type objectType, PropertyForm existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            //add default converter 
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringTypeConverter());
            serializer = JsonSerializer.CreateDefault(settings);

            JToken schemaObj = JToken.Load(reader);

            if (schemaObj.Type == JTokenType.Null) return null;

            PropertyForm schema = new PropertyForm();

            if (schemaObj["href"] != null) schema.Href = schemaObj["href"].ToObject<Uri>();
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

            if (schemaObj["security"] != null) schema.Security = serializer.Deserialize<string[]>(new JTokenReader(schemaObj["security"]));

            if (schemaObj["scopes"] != null) schema.Scopes = serializer.Deserialize<string[]>(new JTokenReader(schemaObj["scopes"]));

            if (schemaObj["op"] == null)
            { //default value handling

                //TODO: Default value of op needs to be handled here
            }
            else
            {
                schema.Op = serializer.Deserialize<string[]>(new JTokenReader(schemaObj["op"]));
            }

            return schema;
        }

    }

    //Converter for ActionForm 
    public class ActionFormConverter : JsonConverter<ActionForm>
    {

        public override void WriteJson(JsonWriter writer, ActionForm value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override ActionForm ReadJson(JsonReader reader, Type objectType, ActionForm existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            //add default converter 
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringTypeConverter());
            serializer = JsonSerializer.CreateDefault(settings);

            JToken schemaObj = JToken.Load(reader);

            if (schemaObj.Type == JTokenType.Null) return null;

            ActionForm schema = new ActionForm();

            if (schemaObj["href"] != null) schema.Href = schemaObj["href"].ToObject<Uri>();
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

            if (schemaObj["security"] != null) schema.Security = serializer.Deserialize<string[]>(new JTokenReader(schemaObj["security"]));

            if (schemaObj["scopes"] != null) schema.Scopes = serializer.Deserialize<string[]>(new JTokenReader(schemaObj["scopes"]));

            if (schemaObj["op"] == null)
            { //default value handling

                string tmp = "invokeaction";
                schema.Op = new string[1];
                schema.Op[0] = tmp;
            }
            else
            {
                schema.Op = serializer.Deserialize<string[]>(new JTokenReader(schemaObj["op"]));
            }

            return schema;
        }

    }

    //Converter for EventForm 
    public class EventFormConverter : JsonConverter<EventForm>
    {

        public override void WriteJson(JsonWriter writer, EventForm value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override EventForm ReadJson(JsonReader reader, Type objectType, EventForm existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            //add default converter 
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringTypeConverter());
            serializer = JsonSerializer.CreateDefault(settings);


            JToken schemaObj = JToken.Load(reader);

            if (schemaObj.Type == JTokenType.Null) return null;

            EventForm schema = new EventForm();

            if (schemaObj["href"] != null) schema.Href = schemaObj["href"].ToObject<Uri>();
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

            if (schemaObj["security"] != null) schema.Security = serializer.Deserialize<string[]>(new JTokenReader(schemaObj["security"]));

            if (schemaObj["scopes"] != null) schema.Scopes = serializer.Deserialize<string[]>(new JTokenReader(schemaObj["scopes"]));

            if (schemaObj["op"] == null)
            { //default value handling

                string[] temp = { "subscribeevent", "unsubscribeevent" };
                schema.Op = temp;
            }
            else
            {
                schema.Op = serializer.Deserialize<string[]>(new JTokenReader(schemaObj["op"]));
            }

            return schema;
        }

    }

    //Converter to assign the correct type for a given @contex
    public class AtContextConverter : JsonConverter<Object[]>
    {

        public override void WriteJson(JsonWriter writer, Object[] value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override Object[] ReadJson(JsonReader reader, Type objectType, Object[] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JToken schemaObj = JToken.Load(reader);
            if (schemaObj.Type == JTokenType.Null) return null;
            var type = schemaObj.Type;

            switch (type.ToString())
            {
                case "Array":
                    return AtContextTypeArrayFiller(schemaObj, serializer);
                case "String":
                    return AtContextTypeUriFiller(schemaObj, serializer);

            }
            return null;

        }



        public Object[] AtContextTypeArrayFiller(JToken schemaObj, JsonSerializer serializer)
        {

            Object[] schema = new Object[schemaObj.Count()];

            schema = schemaObj.ToObject<Object[]>();

            return schema;

        }

        public Object[] AtContextTypeUriFiller(JToken schemaObj, JsonSerializer serializer)
        {

            Object[] schema = new object[1];

            schema[0] = schemaObj.ToObject<Uri>();

            return schema;
        }

    }

    //Called for properties that can be either string or string[], and it returns string[].
    public class StringTypeConverter : JsonConverter<string[]>
    {

        public override void WriteJson(JsonWriter writer, string[] value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override string[] ReadJson(JsonReader reader, Type objectType, string[] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JToken schemaObj = JToken.Load(reader);
            if (schemaObj.Type == JTokenType.Null) return null;
            var type = schemaObj.Type;

            switch (type.ToString())
            {
                case "Array":
                    return ArrayFiller(schemaObj, serializer);
                case "String":
                    return StringFiller(schemaObj, serializer);

            }
            return null;

        }






        public string[] ArrayFiller(JToken schemaObj, JsonSerializer serializer)
        {

            string[] schema = new string[schemaObj.Count()];

           

            schema = schemaObj.ToObject<string[]>();

            return schema;

        }

        public string[] StringFiller(JToken schemaObj, JsonSerializer serializer)
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
            writer.WriteValue(value.ToString());
        }

        public override DataSchema[] ReadJson(JsonReader reader, Type objectType, DataSchema[] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JToken schemaObj = JToken.Load(reader);
            if (schemaObj.Type == JTokenType.Null) return null;
            var type = schemaObj.Type;

            switch (type.ToString())
            {
                case "Object":
                    return ObjectFiller(schemaObj, serializer);
                case "Array":
                    return ArrayFiller(schemaObj, serializer);

            }
            return null;

        }



        public DataSchema[] ArrayFiller(JToken schemaObj, JsonSerializer serializer)
        {

            DataSchema[] schema = new DataSchema[schemaObj.Count()];
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new DataSchemaConverter());
            serializer = JsonSerializer.CreateDefault(settings);


            schema = serializer.Deserialize(new JTokenReader(schemaObj), objectType: typeof(DataSchema[])) as DataSchema[];


            return schema;

        }

        public DataSchema[] ObjectFiller(JToken schemaObj, JsonSerializer serializer)
        {
            DataSchema[] schema = new DataSchema[1];

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new DataSchemaConverter());
            serializer = JsonSerializer.CreateDefault(settings);

            schema[0] = serializer.Deserialize(new JTokenReader(schemaObj), objectType: typeof(DataSchema)) as DataSchema;

            return schema;


        }
    }

    //Converter to assign the corresponding InteractionAffordance for a given schema
    public class InteractionAffordanceConverter : JsonConverter<InteractionAffordance>
    {

        public override void WriteJson(JsonWriter writer, InteractionAffordance value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override InteractionAffordance ReadJson(JsonReader reader, Type objectType, InteractionAffordance existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JToken schemaObj = JToken.Load(reader);
            if (schemaObj.Type == JTokenType.Null) return null;

            switch (objectType.FullName)
            {
                case "WoT_Definitions.PropertyAffordance":
                    return FillPropertyAffordanceObject(schemaObj, serializer, reader);
                case "WoT_Definitions.ActionAffordance":
                    return FillActionAffordanceObject(reader, schemaObj, serializer);
                case "WoT_Definitions.EventAffordance":
                    return FillEventAffordanceObject(schemaObj, serializer);
            }
            return null;
        }

        public PropertyAffordance FillPropertyAffordanceObject(JToken schemaObj, JsonSerializer serializer, JsonReader reader)
        {
            PropertyAffordance schema = new PropertyAffordance();
            CommonFiller(schema, schemaObj, serializer);

            if (schemaObj["const"] != null) schema.Const = schemaObj["const"];
            if (schemaObj["default"] != null) schema.Default = schemaObj["default"];
            if (schemaObj["unit"] != null) schema.Unit = schemaObj["unit"].ToObject<string>();
            if (schemaObj["enum"] != null) schema.Enum = schemaObj["enum"].ToObject<Object[]>();
            if (schemaObj["readOnly"] != null) schema.ReadOnly = schemaObj["readOnly"].ToObject<bool>();
            if (schemaObj["writeOnly"] != null) schema.WriteOnly = schemaObj["writeOnly"].ToObject<bool>();
            if (schemaObj["oneOf"] != null) schema.OneOf = serializer.Deserialize(new JTokenReader(schemaObj["oneOf"]), objectType: typeof(DataSchema[])) as DataSchema[];
            if (schemaObj["format"] != null) schema.Format = schemaObj["format"].ToObject<string>();
            if (schemaObj["type"] != null) schema.Type = schemaObj["type"].ToObject<string>();


            if (schemaObj["observable"] != null) schema.Observable = schemaObj["observable"].ToObject<Boolean>();

            if (schemaObj["forms"] != null) schema.PropertyForm = serializer.Deserialize(new JTokenReader(schemaObj["forms"]), objectType: typeof(PropertyForm[])) as PropertyForm[];

            return schema;
        }

        public ActionAffordance FillActionAffordanceObject(JsonReader reader, JToken schemaObj, JsonSerializer serializer)
        {
            ActionAffordance schema = new ActionAffordance();
            CommonFiller(schema, schemaObj, serializer);
            if (schemaObj["input"] != null) schema.Input = serializer.Deserialize(new JTokenReader(schemaObj["input"]), objectType: typeof(DataSchema)) as DataSchema;
            if (schemaObj["output"] != null) schema.Output = serializer.Deserialize(new JTokenReader(schemaObj["output"]), objectType: typeof(DataSchema)) as DataSchema;
            if (schemaObj["safe"] != null) schema.Safe = schemaObj["safe"].ToObject<Boolean>();
            if (schemaObj["idempotent"] != null) schema.Idempotent = schemaObj["idempotent"].ToObject<Boolean>();
            if (schemaObj["synchronous"] != null) schema.Synchronous = schemaObj["synchronous"].ToObject<Boolean>();




            if (schemaObj["forms"] != null) schema.ActionForm = serializer.Deserialize(new JTokenReader(schemaObj["forms"]), objectType: typeof(ActionForm[])) as ActionForm[];


            return schema;
        }

        public EventAffordance FillEventAffordanceObject(JToken schemaObj, JsonSerializer serializer)
        {
            EventAffordance schema = new EventAffordance();
            CommonFiller(schema, schemaObj, serializer);
            if (schemaObj["subscription"] != null) schema.Subscription = serializer.Deserialize(new JTokenReader(schemaObj["subscription"]), objectType: typeof(DataSchema)) as DataSchema;
            if (schemaObj["data"] != null) schema.Data = serializer.Deserialize(new JTokenReader(schemaObj["data"]), objectType: typeof(DataSchema)) as DataSchema;
            if (schemaObj["dataResponse"] != null) schema.DataResponse = serializer.Deserialize(new JTokenReader(schemaObj["dataResponse"]), objectType: typeof(DataSchema)) as DataSchema;
            if (schemaObj["cancellation"] != null) schema.Cancellation = serializer.Deserialize(new JTokenReader(schemaObj["cancellation"]), objectType: typeof(DataSchema)) as DataSchema;

            if (schemaObj["forms"] != null) schema.EventForm = serializer.Deserialize(new JTokenReader(schemaObj["forms"]), objectType: typeof(EventForm[])) as EventForm[];

            return schema;
        }

        public void CommonFiller(InteractionAffordance schema, JToken schemaObj, JsonSerializer serializer)
        {
            schema.Description = (string)schemaObj["description"];
            schema.Title = (string)schemaObj["title"];

            //if (schemaObj["@type"] != null) schema.AtType = serializer.Deserialize(new JTokenReader(schemaObj["@type"]), objectType: typeof(AtTypeType)) as AtTypeType;

            if (schemaObj["@type"] != null)
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringTypeConverter());
                serializer = JsonSerializer.CreateDefault(settings);
                schema.AtType = serializer.Deserialize<string[]>(new JTokenReader(schemaObj["@type"]));
            }

            if (schemaObj["titles"] != null) schema.Titles = schemaObj["titles"].ToObject<string[]>();
            if (schemaObj["descriptions"] != null) schema.Descriptions = schemaObj["descriptions"].ToObject<string[]>();

            schema.OriginalJson = schemaObj.ToString();





        }


    }

}


