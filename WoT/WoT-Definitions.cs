using System.Collections.Generic;
using System;
using Newtonsoft;
using Newtonsoft.Json;

namespace WoT_Definitions
{
    public interface IDataSchema
    {
        string[] AtType { get; set; }
        string Title { get; set; }
        string[] Titles { get; set; }
        string Description { get; set; }
        string[] Descriptions { get; set; }
        Object Const { get; set; }
        Object Default { get; set; }
        string Unit { get; set; }
        IDataSchema[] OneOf { get; set; }
        Object[] Enum { get; set; }
        Boolean ReadOnly { get; set; }
        Boolean WriteOnly { get; set; }
        string Format { get; set; }
        string Type { get; }

    }
    public interface INteractionAffordance
    {
        string[] AtType { get; set; }
        string Title { get; set; }
        string[] Titles { get; set; }
        string Description { get; set; }
        string[] Descriptions { get; set; }
        Form[] Forms { get; set; }
        Dictionary<string, DataSchema> UriVariables { get; set; }

    }
    public abstract class DataSchema : IDataSchema
    {
        public string[] AtType { get; set; }
        public string Title { get; set; }
        public string[] Titles { get; set; }
        public string Description { get; set; }
        public string[] Descriptions { get; set; }
        public object Const { get; set; }
        public object Default { get; set; }
        public string Unit { get; set; }
        public IDataSchema[] OneOf { get; set; }
        public IDataSchema[] AllOf { get; set; }
        public object[] Enum { get; set; }
        public bool ReadOnly { get; set; }
        public bool WriteOnly { get; set; }
        public string Format { get; set; }
        public string Type { get; }
    }
    public class ArraySchema : DataSchema
    {
        public new readonly string Type = "array";
        public DataSchema[] Items { get; set; }
        public uint MinItems { get; set; }
        public uint MaxItems { get; set; }
    }
    public class BooleanSchema : DataSchema
    {
        public new readonly string Type = "boolean";
    }

    public class NumberSchema : DataSchema
    {
        public new readonly string Type = "number";
        public double Minimum { get; set; }
        public double ExclusiveMinimum { get; set; }
        public double Maximum { get; set; }
        public double ExclusiveMaximum { get; set; }
        public double MultipleOf { get; set; }

    }
    public class IntegerSchema : DataSchema
    {
        public new readonly string Type = "integer";
        public int Minimum { get; set; }
        public int ExclusiveMinimum { get; set; }
        public int Maximum { get; set; }
        public int ExclusiveMaximum { get; set; }
        public int MultipleOf { get; set; }
        public new int Const { get; set; }
}
    public class ObjectSchema : DataSchema
    {
        public new readonly string Type = "object";
        public Dictionary<string, DataSchema> Properties { get; set; }
        public string[] Required { get; set; }
    }
    public class StringSchema : DataSchema
    {
        public new readonly string Type = "string";
        public uint MinLength { get; set; }
        public uint MaxLength { get; set; }
        public string Pattern { get; set; }
        public string ContentEncoding { get; set; }
        public string ContentMediaType { get; set; }
    }
    public class NullSchema : DataSchema
    {
        public new readonly string Type = "null";
    }
    public class InteractionAffordance : INteractionAffordance
    {
        public string[] AtType { get; set; }
        public string Title { get; set; }
        public string[] Titles { get; set; }
        public string Description { get; set; }
        public string[] Descriptions { get; set; }
        public Form[] Forms { get; set; }
        public Dictionary<string, DataSchema> UriVariables { get; set; }
    }
    public class PropertyAffordance : InteractionAffordance, IDataSchema
    {
        public object Const { get; set; }
        public object Default { get; set; }
        public string Unit { get; set; }
        public IDataSchema[] OneOf { get; set; }
        public object[] Enum { get; set; }
        public bool ReadOnly { get; set; }
        public bool WriteOnly { get; set; }
        public string Format { get; set; }
        public string Type { get; set; }
        public Boolean Observable { get; set; }
    }

    public class ActionAffordance : InteractionAffordance
    {
        public DataSchema Input { get; set; }
        public DataSchema Output { get; set; }
        public Boolean Safe { get; set; }
        public Boolean Idempotent { get; set; }
        public Boolean Synchronous { get; set; }
    }

    public class EventAffordance : InteractionAffordance
    {
        public DataSchema Subscription { get; set; }
        public DataSchema Data { get; set; }
        public DataSchema DataResponse { get; set; }
        public DataSchema Cancellation { get; set; }
    }

    public class Form
    {
        public Uri Href { get; set; }
        public string ContentType { get; set; }
        public string ContentCoding { get; set; }
        public string[] Security { get; set; }
        public string[] Scopes { get; set; }


    }

    public struct Link
    {
        public Uri Href { get; set; }
        public string Type { get; set; }
        public string Rel { get; set; }
        public Uri Anchor { get; set; }
        public string Uri { get; set; }
        public string[] Hreflang { get; set; }
    }

    public struct VersionInfo
    {
        public string Instance { get; set; }
        public string Model { get; set; }

        public VersionInfo(string instance)
        {
            this.Instance = instance;
            this.Model = null;
        }

        public VersionInfo(string instance, string model)
        {
            this.Instance = instance;
            this.Model = model;
        }

        public VersionInfo(VersionInfo versionInfo)
        {
            this = versionInfo;
        }
    }

    public struct ExpectedRespone
    {
        public string ContentType { get; set; }
        public ExpectedRespone(string contentType)
        {
            this.ContentType = contentType;
        }
    }

    public struct AdditionalExpectedRespone
    {
        public string ContentType { get; set; }
        public Boolean Success { get; set; }
        public string Schema { get; set; }
        public AdditionalExpectedRespone(string contentType)
        {
            this.ContentType = contentType;
            this.Success = false;
            this.Schema = null;
        }
    }

    public class SecurityScheme
    {
        public string[] AtType { get; set; }
        public string Description { get; set; }
        public string[] Descriptions { get; set; }
        public Uri Proxy { get; set; }
        public string Scheme { get; set; }
    }

    public class NoSecurityScheme : SecurityScheme
    {
        public readonly string Scheme = "nosec";
    }

    public class BasicSecurityScheme : SecurityScheme
    {
        public readonly string Scheme = "basic";
        public string Name { get; set; }
        public string In { get; set; }
    }

    public class ThingDescription
    {
        //[JsonConverter()]
        public Object[] AtContext { get; set; }
        public string[] AtType { get; set; }
        /**
         * Identifier of the Thing in form of a URI [RFC3986] (e.g., stable URI, temporary and mutable URI, URI with local IP address, URN, etc.).
         **/
        public string Id { get; set; }
        /**
         * Provides a human-readable title (e.g., display a text for UI representation) based on a default language.
         **/
        public string Title { get; set; }
        public string[] Titles { get; set; }
        public string Description { get; set; }
        public string[] Descriptions { get; set; }
        public VersionInfo Version { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public Uri Support { get; set; }
        public Uri Base { get; set; }
        public Dictionary<string, PropertyAffordance> Properties { get; set; }
        public Dictionary<string, ActionAffordance> Actions { get; set; }
        public Dictionary<string, EventAffordance> Events { get; set; }
        public Link[] Links { get; set; }
        public Form[] Forms { get; set; }
        public string[] Security { get; set; }
        public Dictionary<string, SecurityScheme> SecurityDefinitions { get; set; }
        public Uri[] Profile { get; set; }
        public Dictionary<string, DataSchema> SchemaDefinitions { get; set; }
        public Dictionary<string, DataSchema> UriVariables { get; set; }

    }

}