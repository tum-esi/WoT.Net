using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WoT.TDHelpers;

namespace WoT.Definitions
{
    /// <summary>
    /// A Map providing a set of human-readable texts in different languages identified by language tags described in [<see href="https://www.rfc-editor.org/rfc/rfc5646">BCP47</see>]. See 6.3.2 Human-Readable Metadata for example usages of this container in a Thing Description instance.
    /// <para>
    ///  Each name of the MultiLanguage Map MUST be a language tag as defined in [<see href="https://www.rfc-editor.org/rfc/rfc5646">BCP47</see>]. Each value of the MultiLanguage Map MUST be of type string.
    /// </para>
    /// </summary>
    public class MultiLanguage : Dictionary<string, string>
    {

    }
    public interface IDataSchema
    {
        [JsonProperty("@type")]
        string[] AtType { get; set; }
        string Title { get; set; }
        MultiLanguage Titles { get; set; }
        string Description { get; set; }
        MultiLanguage Descriptions { get; set; }
        object Const { get; set; }
        object Default { get; set; }
        string Unit { get; set; }
        IDataSchema[] OneOf { get; set; }
        object[] Enum { get; set; }
        bool ReadOnly { get; set; }
        bool WriteOnly { get; set; }
        string Format { get; set; }
        string Type { get; }

    }
    public interface IArraySchema: IDataSchema
    {
        DataSchema[] Items { get; set; }
        uint? MinItems { get; set; }
        uint? MaxItems { get; set; }

    }
    public interface IBooleanSchema
    {
        bool Const { get; set; }
        bool Default { get; set; }
        bool[] Enum { get; set; }
    }
    public interface INumberSchema
    {
        double? Minimum { get; set; }
        double? ExclusiveMinimum { get; set; }
        double? Maximum { get; set; }
        double? ExclusiveMaximum { get; set; }
        double? MultipleOf { get; set; }
        double? Const { get; set; }
        double? Default { get; set; }
        double[] Enum { get; set; }
    }
    public interface IIntegerSchema
    {
        int? Minimum { get; set; }
        int? ExclusiveMinimum { get; set; }
        int? Maximum { get; set; }
        int? ExclusiveMaximum { get; set; }
        int? MultipleOf { get; set; }
        int? Const { get; set; }
        int? Default { get; set; }
        int[] Enum { get; set; }
    }
    public interface IObjectSchema
    {
        Dictionary<string, DataSchema> Properties { get; set; }
        string[] Required { get; set; }
    }
    public interface IStringSchema
    {
        uint? MinLength { get; set; }
        uint? MaxLength { get; set; }
        string Pattern { get; set; }
        string ContentEncoding { get; set; }
        string ContentMediaType { get; set; }
        string Const { get; set; }
        string Default { get; set; }
        string[] Enum { get; set; }

    }
    public interface IInteractionAffordance
    {
        [JsonProperty("@type")]
        string[] AtType { get; set; }
        string Title { get; set; }
        MultiLanguage Titles { get; set; }
        string Description { get; set; }
        MultiLanguage Descriptions { get; set; }
        Form[] Forms { get; set; }
        Dictionary<string, DataSchema> UriVariables { get; set; }
        string OriginalJson { get; set; }


    }

    [JsonConverter(typeof(DataSchemaConverter))]
    public class DataSchema : IDataSchema
    {
        [JsonProperty("@type")]
        public string[] AtType { get; set; }
        public string Title { get; set; }
        public MultiLanguage Titles { get; set; }
        public string Description { get; set; }
        public MultiLanguage Descriptions { get; set; }
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
    public class ArraySchema : DataSchema, IArraySchema
    {
        public new readonly string Type = "array";
        public DataSchema[] Items { get; set; }
        public uint? MinItems { get; set; }
        public uint? MaxItems { get; set; }
    }
    public class BooleanSchema : DataSchema, IBooleanSchema
    {
        public new readonly string Type = "boolean";
        public new bool Const { get; set; }
        public new bool Default { get; set; }
        public new bool[] Enum { get; set; }
    }
    public class NumberSchema : DataSchema, INumberSchema
    {
        public new readonly string Type = "number";
        public double? Minimum { get; set; }
        public double? ExclusiveMinimum { get; set; }
        public double? Maximum { get; set; }
        public double? ExclusiveMaximum { get; set; }
        public double? MultipleOf { get; set; }
        public new double? Const { get; set; }
        public new double? Default { get; set; }
        public new double[] Enum { get; set; }  
    }
    public class IntegerSchema : DataSchema, IIntegerSchema
    {
        public new readonly string Type = "integer";
        public int? Minimum { get; set; }
        public int? ExclusiveMinimum { get; set; }
        public int? Maximum { get; set; }
        public int? ExclusiveMaximum { get; set; }
        public int? MultipleOf { get; set; }
        public new int? Const { get; set; }
        public new int? Default { get; set; }
        public new int[] Enum { get; set; }
    }
    public class ObjectSchema : DataSchema, IObjectSchema
    {
        public new readonly string Type = "object";
        public Dictionary<string, DataSchema> Properties { get; set; }
        public string[] Required { get; set; }
    }
    public class StringSchema : DataSchema, IStringSchema
    {
        public new readonly string Type = "string";
        public uint? MinLength { get; set; }
        public uint? MaxLength { get; set; }
        public string Pattern { get; set; }
        public string ContentEncoding { get; set; }
        public string ContentMediaType { get; set; }
        public new string Const { get; set; }
        public new string Default { get; set; }
        public new string[] Enum { get; set; }

    }
    public class NullSchema : DataSchema
    {
        public new readonly string Type = "null";

    }
    public class InteractionAffordance : IInteractionAffordance
    {
        [JsonProperty("@type")]
        public string[] AtType { get; set; }
        public string Title { get; set; }
        public MultiLanguage Titles { get; set; }
        public string Description { get; set; }
        public MultiLanguage Descriptions { get; set; }
        public string OriginalJson { get; set; }
        public Form[] Forms { get; set; }
        public Dictionary<string, DataSchema> UriVariables { get; set; }
    }
    [JsonConverter(typeof(PropertyAffordanceConverter))]
    public class PropertyAffordance : InteractionAffordance, IDataSchema
    {
        public object Const { get; set; }
        public object Default { get; set; }
        public string Unit { get; set; }
        public IDataSchema[] OneOf { get; set; }
        public IDataSchema[] AllOf { get; set; }
        public object[] Enum { get; set; }
        public bool ReadOnly { get; set; }
        public bool WriteOnly { get; set; }
        public string Format { get; set; }
        public string Type { get; set; }
        public bool Observable { get; set; }
        public new PropertyForm[] Forms { get; set; }

    }
    public class ArrayPropertyAffordance : PropertyAffordance, IArraySchema
    {
        public new readonly string Type = "array";
        public DataSchema[] Items { get; set; }
        public uint? MinItems { get; set; }
        public uint? MaxItems { get; set; }
    }
    public class BooleanPropertyAffordance : PropertyAffordance, IBooleanSchema
    {
        public new readonly string Type = "boolean";
        public new bool Const { get; set; }
        public new bool Default { get; set; }
        public new bool[] Enum { get; set; }
    }
    public class NumberPropertyAffordance : PropertyAffordance, INumberSchema
    {
        public new readonly string Type = "number";
        public double? Minimum { get; set; }
        public double? ExclusiveMinimum { get; set; }
        public double? Maximum { get; set; }
        public double? ExclusiveMaximum { get; set; }
        public double? MultipleOf { get; set; }
        public new double? Const { get; set; }
        public new double? Default { get; set; }
        public new double[] Enum { get; set; }
    }
    public class IntegerPropertyAffordance : PropertyAffordance, IIntegerSchema
    {
        public new readonly string Type = "integer";
        public int? Minimum { get; set; }
        public int? ExclusiveMinimum { get; set; }
        public int? Maximum { get; set; }
        public int? ExclusiveMaximum { get; set; }
        public int? MultipleOf { get; set; }
        public new int? Const { get; set; }
        public new int? Default { get; set; }
        public new int[] Enum { get; set; }
    }
    public class ObjectPropertyAffordance : PropertyAffordance, IObjectSchema
    {
        public new readonly string Type = "object";
        public Dictionary<string, DataSchema> Properties { get; set; }
        public string[] Required { get; set; }
    }
    public class StringPropertyAffordance : PropertyAffordance, IStringSchema
    {
        public new readonly string Type = "string";
        public uint? MinLength { get; set; }
        public uint? MaxLength { get; set; }
        public string Pattern { get; set; }
        public string ContentEncoding { get; set; }
        public string ContentMediaType { get; set; }
        public new string Const { get; set; }
        public new string Default { get; set; }
        public new string[] Enum { get; set; }

    }
    public class NullPropertyAffordance : PropertyAffordance
    {
        public new readonly string Type = "null";

    }

    [JsonConverter(typeof(ActionAffordanceConverter))]
    public class ActionAffordance : InteractionAffordance
    {
        public DataSchema Input { get; set; }
        public DataSchema Output { get; set; }
        public bool Safe { get; set; }
        public bool Idempotent { get; set; }
        public bool? Synchronous { get; set; }
        public new ActionForm[] Forms { get; set; }


    }

    [JsonConverter(typeof(EventAffordanceConverter))]
    public class EventAffordance : InteractionAffordance
    {
        public DataSchema Subscription { get; set; }
        public DataSchema Data { get; set; }
        public DataSchema DataResponse { get; set; }
        public DataSchema Cancellation { get; set; }
        public new EventForm[] Forms { get; set; }

    }

    [JsonConverter(typeof(FormConverter))]
    public class Form
    {
        public Uri Href { get; set; }
        public string ContentType { get; set; }
        public string ContentCoding { get; set; }
        public string[] Security { get; set; }
        public string[] Scopes { get; set; }
        public AdditionalExpectedResponse? AdditionalExpectedResponse { get; set; }
        public string Subprotocol { get; set; }
        public string OriginalJson { get; set; }
        public string[] Op { get; set; }


    }

    [JsonConverter(typeof(PropertyFormConverter))]
    public class PropertyForm : Form
    {


    }

    [JsonConverter(typeof(ActionFormConverter))]
    public class ActionForm : Form
    {

    }

    [JsonConverter(typeof(EventFormConverter))]
    public class EventForm : Form
    {

    }


    public struct Link
    {
        public Uri Href { get; set; }
        public string Type { get; set; }
        public string Rel { get; set; }
        public Uri Anchor { get; set; }
        public string Uri { get; set; }
        [JsonConverter(typeof(StringTypeConverter))]
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

    public struct AdditionalExpectedResponse
    {
        public string ContentType { get; set; }
        public bool Success { get; set; }
        public string Schema { get; set; }
        public AdditionalExpectedResponse(string contentType)
        {
            this.ContentType = contentType;
            this.Success = false;
            this.Schema = null;
        }
    }


    [JsonConverter(typeof(SecuritySchemeConverter))]
    public abstract class SecurityScheme
    {
        [JsonProperty("@type")]
        public string[] AtType { get; set; }
        public string Description { get; set; }
        public MultiLanguage Descriptions { get; set; }
        public Uri Proxy { get; set; }
        public string Scheme { get; set; }
    }

    public class NoSecurityScheme : SecurityScheme
    {
        public new readonly string Scheme = "nosec";
    }

    public class BasicSecurityScheme : SecurityScheme
    {
        public new readonly string Scheme = "basic";
        public string Name { get; set; }
        public string In { get; set; }
    }

    /// <summary>
    /// Class <c> ThingDescription </c> is a DTO representing metadata and interfaces of a Thing
    /// </summary>
    public class ThingDescription
    {
        /// <summary>
        /// JSON-LD keyword to define short-hand names called terms that are used throughout a TD document.
        /// </summary>
        [JsonProperty("@context")]
        [JsonConverter(typeof(AtContextConverter))]
        public object[] AtContext { get; set; }
        [JsonProperty("@type")]
        [JsonConverter(typeof(StringTypeConverter))]
        public string[] AtType { get; set; }
        /// <summary>
        /// Identifier of the Thing in form of a URI [<see href="https://www.rfc-editor.org/rfc/rfc3986">RFC3986</see>] (e.g., stable URI, temporary and mutable URI, URI with local IP address, URN, etc.).
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Provides a human-readable title (e.g., display a text for UI representation) based on a default language.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Provides multi-language human-readable titles (e.g., display a text for UI representation in different languages). Also see <seealso cref="MultiLanguage"/>
        /// </summary>
        public MultiLanguage Titles { get; set; }
        public string Description { get; set; }
        public MultiLanguage Descriptions { get; set; }
        public VersionInfo? Version { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public Uri Support { get; set; }
        public Uri Base { get; set; }
        public Dictionary<string, PropertyAffordance> Properties { get; set; }
        public Dictionary<string, ActionAffordance> Actions { get; set; }
        public Dictionary<string, EventAffordance> Events { get; set; }
        public Link[] Links { get; set; }
        public Form[] Forms { get; set; }
        [JsonConverter(typeof(StringTypeConverter))]
        public string[] Security { get; set; }
        public Dictionary<string, SecurityScheme> SecurityDefinitions { get; set; }
        [JsonConverter(typeof(StringTypeConverter))]
        public string[] Profile { get; set; }
        public Dictionary<string, DataSchema> SchemaDefinitions { get; set; }
        public Dictionary<string, DataSchema> UriVariables { get; set; }

        //any properties not matching the explicitly defined in properties will be captured in the AdditionalProperties dictionary.
        [JsonExtensionData]
        public Dictionary<string, JToken> AdditionalProperties { get; set; }
    }

}