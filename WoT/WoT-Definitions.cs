using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WoT.TDHelpers;

namespace WoT.Definitions
{
    /// <summary>
    /// A Map providing a set of human-readable texts in different languages identified by language tags described in [<see href="https://www.rfc-editor.org/rfc/rfc5646">BCP47</see>].
    /// </summary>
    /// <remarks>
    ///  See <see href="https://www.w3.org/TR/wot-thing-description11/#titles-descriptions-serialization-json">6.3.2 Human-Readable Metadata</see> for example usages of this container in a Thing Description instance.
    ///  Each name of the MultiLanguage Map MUST be a language tag as defined in [<see href="https://www.rfc-editor.org/rfc/rfc5646">BCP47</see>].
    ///  Each value of the MultiLanguage Map MUST be of type string.
    /// </remarks>
    public class MultiLanguage : Dictionary<string, string>
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public interface IDataSchema
    {
        [JsonProperty("@type")]
        string[] AtType { get; set; }
        string Title { get; set; }
        MultiLanguage Titles { get; set; }
        string Description { get; set; }
        MultiLanguage Descriptions { get; set; }

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        object Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        object Default { get; set; }
        string Unit { get; set; }
        IDataSchema[] OneOf { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        object[] Enum { get; set; }
        bool ReadOnly { get; set; }
        bool WriteOnly { get; set; }
        string Format { get; set; }
        string Type { get; }

    }
    public interface IArraySchema : IDataSchema
    {

        DataSchema[] Items { get; set; }
        uint? MinItems { get; set; }
        uint? MaxItems { get; set; }

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        new List<object> Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        new List<object> Default { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        new List<object>[] Enum { get; set; }

    }
    public interface IBooleanSchema : IDataSchema
    {

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        new bool Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        new bool Default { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        new bool[] Enum { get; set; }
    }
    public interface INumberSchema : IDataSchema
    {
        double? Minimum { get; set; }
        double? ExclusiveMinimum { get; set; }
        double? Maximum { get; set; }
        double? ExclusiveMaximum { get; set; }
        double? MultipleOf { get; set; }

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        new double? Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        new double? Default { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        new double[] Enum { get; set; }
    }
    public interface IIntegerSchema : IDataSchema
    {
        int? Minimum { get; set; }
        int? ExclusiveMinimum { get; set; }
        int? Maximum { get; set; }
        int? ExclusiveMaximum { get; set; }
        int? MultipleOf { get; set; }

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        new int? Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        new int? Default { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        new int[] Enum { get; set; }
    }
    public interface IObjectSchema : IDataSchema
    {
        Dictionary<string, DataSchema> Properties { get; set; }
        string[] Required { get; set; }

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        new Dictionary<string, DataSchema> Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        new Dictionary<string, DataSchema> Default { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        new Dictionary<string, DataSchema>[] Enum { get; set; }
    }
    public interface IStringSchema : IDataSchema
    {
        uint? MinLength { get; set; }
        uint? MaxLength { get; set; }
        string Pattern { get; set; }
        string ContentEncoding { get; set; }
        string ContentMediaType { get; set; }

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        new string Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        new string Default { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        new string[] Enum { get; set; }

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


    /// <summary>
    /// Metadata that describes the data format used. It can be used for validation.
    /// </summary>
    [JsonConverter(typeof(DataSchemaConverter))]
    public class DataSchema : IDataSchema
    {
        /// <summary>
        ///     Base Constructor
        /// </summary>
        public DataSchema() { }

        /// <summary>
        ///     JSON-LD keyword to label the object with semantic tags (or types)
        /// </summary>
        [JsonProperty("@type")]
        public string[] AtType { get; set; }

        /// <summary>
        ///     Provides a human-readable title (e.g., display a text for UI representation) based on a default language.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Provides multi-language human-readable titles (e.g., display a text for UI representation in different languages).
        /// </summary>
        /// <seealso cref="MultiLanguage"/>
        public MultiLanguage Titles { get; set; }

        /// <summary>
        ///     Provides additional (human-readable) information based on a default language.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Can be used to support (human-readable) information in different languages.
        /// </summary>
        /// <seealso cref="MultiLanguage"/>
        public MultiLanguage Descriptions { get; set; }

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        public object Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        public object Default { get; set; }

        /// <summary>
        ///     Provides unit information that is used, e.g., in international science, engineering, and business.
        /// </summary>
        /// <remarks>
        ///     To preserve uniqueness, it is recommended that the value of the unit points to a semantic definition (also see Section <see href="https://www.w3.org/TR/wot-thing-description11/#semantic-annotations-example-version-units">Semantic Annotations</see>).
        /// </remarks>
        public string Unit { get; set; }

        /// <summary>
        ///     Used to ensure that the data is valid against one of the specified schemas in the array.
        /// </summary>
        /// <remarks>
        ///     This can be used to describe multiple input or output schemas.
        /// </remarks>
        public IDataSchema[] OneOf { get; set; }

        /// <summary>
        ///     Used to ensure that the data is valid against all of the specified schemas in the array.
        /// </summary>
        public IDataSchema[] AllOf { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        public object[] Enum { get; set; }

        /// <summary>
        ///     Boolean value that is a hint to indicate whether a property interaction / value is read only (=true) or not (=false).
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        ///     Boolean value that is a hint to indicate whether a property interaction / value is write only (=true) or not (=false).
        /// </summary>
        public bool WriteOnly { get; set; }

        /// <summary>
        ///     Allows validation based on a format pattern such as "date-time", "email", "uri", etc.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        ///     Assignment of JSON-based data types compatible with JSON Schema (one of boolean, integer, number, string, object, array, or null).
        /// </summary>
        public string Type { get; }



    }

    /// <summary>
    ///     Metadata describing data of type <c>array</c>.
    /// </summary>
    /// <remarks>
    ///     This Subclass is indicated by the value <c>"array"</c> assigned to <c>Type</c> in <see cref="DataSchema">DataSchema</see> instances.
    /// </remarks>
    public class ArraySchema : DataSchema, IArraySchema
    {
        /// <summary>
        ///     Base Constructor
        /// </summary>
        public ArraySchema() { }

        /// <summary>
        ///     Assignment of JSON-based data types compatible with JSON Schema.
        /// </summary>
        /// <value>
        ///     <c>"array"</c>
        /// </value>
        public new readonly string Type = "array";

        /// <summary>
        ///  	Used to define the characteristics of an array.
        /// </summary>
        public DataSchema[] Items { get; set; }

        /// <summary>
        ///     Defines the minimum number of items that have to be in the array.
        /// </summary>
        public uint? MinItems { get; set; }

        /// <summary>
        ///     Defines the maximum number of items that have to be in the array.
        /// </summary>
        public uint? MaxItems { get; set; }

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        public new List<object> Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        public new List<object> Default { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        public new List<object>[] Enum { get; set; }
    }

    /// <summary>
    ///     Metadata describing data of type <c>boolean</c>.
    /// </summary>
    /// <remarks>
    ///     This Subclass is indicated by the value <c>"boolean"</c> assigned to <c>Type</c> in <see cref="DataSchema"/> instances.
    /// </remarks>
    public class BooleanSchema : DataSchema, IBooleanSchema
    {
        /// <summary>
        ///     Base Constructor
        /// </summary>
        public BooleanSchema() { }

        /// <summary>
        ///     Assignment of JSON-based data types compatible with JSON Schema.
        /// </summary>
        /// <value>
        ///     <c>"boolean"</c>
        /// </value>
        public new readonly string Type = "boolean";

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        public new bool Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        public new bool Default { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        public new bool[] Enum { get; set; }
    }

    /// <summary>
    ///     Metadata describing data of type <c>number</c>.
    /// </summary>
    /// <remarks>
    ///     This Subclass is indicated by the value <c>"number"</c> assigned to <c>Type</c> in <see cref="DataSchema"/> instances.
    /// </remarks>
    public class NumberSchema : DataSchema, INumberSchema
    {
        /// <summary>
        ///     Base Constructor
        /// </summary>
        public NumberSchema() { }

        /// <summary>
        ///     Assignment of JSON-based data types compatible with JSON Schema.
        /// </summary>
        /// <value>
        ///     <c>"number"</c>
        /// </value>
        public new readonly string Type = "number";

        /// <summary>
        ///     Specifies a minimum numeric value, representing an inclusive lower limit.
        /// </summary>
        public double? Minimum { get; set; }

        /// <summary>
        /// Specifies a minimum numeric value, representing an exclusive lower limit.
        /// </summary>
        public double? ExclusiveMinimum { get; set; }

        /// <summary>
        /// Specifies a maximum numeric value, representing an inclusive upper limit.
        /// </summary>
        public double? Maximum { get; set; }

        /// <summary>
        /// Specifies a maximum numeric value, representing an exclusive upper limit.
        /// </summary>
        public double? ExclusiveMaximum { get; set; }

        /// <summary>
        ///  	Specifies the multipleOf value number. The value must strictly greater than 0.
        /// </summary>
        public double? MultipleOf { get; set; }

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        public new double? Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        public new double? Default { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        public new double[] Enum { get; set; }  
    }

    /// <summary>
    ///     Metadata describing data of type <c>integer</c>.
    /// </summary>
    /// <remarks>
    ///     This Subclass is indicated by the value <c>"integer"</c> assigned to <c>Type</c> in <see cref="DataSchema"/> instances.
    /// </remarks>
    public class IntegerSchema : DataSchema, IIntegerSchema
    {
        /// <summary>
        ///     Base Constructor
        /// </summary>
        public IntegerSchema() { }

        /// <summary>
        ///     Assignment of JSON-based data types compatible with JSON Schema.
        /// </summary>
        /// <value>
        ///     <c>"integer"</c>
        /// </value>
        public new readonly string Type = "integer";

        /// <summary>
        ///     Specifies a minimum numeric value, representing an inclusive lower limit.
        /// </summary>
        public int? Minimum { get; set; }

        /// <summary>
        /// Specifies a minimum numeric value, representing an exclusive lower limit.
        /// </summary>
        public int? ExclusiveMinimum { get; set; }

        /// <summary>
        /// Specifies a maximum numeric value, representing an inclusive upper limit.
        /// </summary>
        public int? Maximum { get; set; }

        /// <summary>
        /// Specifies a maximum numeric value, representing an exclusive upper limit.
        /// </summary>
        public int? ExclusiveMaximum { get; set; }

        /// <summary>
        ///  	Specifies the multipleOf value number. The value must strictly greater than 0.
        /// </summary>
        public int? MultipleOf { get; set; }

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        public new int? Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        public new int? Default { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        public new int[] Enum { get; set; }
    }

    /// <summary>
    ///     Metadata describing data of type <c>object</c>.
    /// </summary>
    /// <remarks>
    ///     This Subclass is indicated by the value <c>"object"</c> assigned to <c>Type</c> in <see cref="DataSchema"/> instances.
    ///     JSON <c>object</c> type is NOT the same as <c>object</c> type in C#. JSON <c>object</c> map to <c>Dictionary<string, object></c>.
    /// </remarks>
    public class ObjectSchema : DataSchema, IObjectSchema
    {
        /// <summary>
        ///     Base Constructor
        /// </summary>
        public ObjectSchema() { }

        /// <summary>
        ///     Assignment of JSON-based data types compatible with JSON Schema.
        /// </summary>
        /// <value>
        ///     <c>"integer"</c>
        /// </value>
        public new readonly string Type = "object";

        /// <summary>
        /// Data schema nested definitions.
        /// </summary>
        public Dictionary<string, DataSchema> Properties { get; set; }

        /// <summary>
        /// Defines which members of the object type are mandatory, i.e. which members are mandatory in the payload that is to be sent (e.g. <c>input</c> of <c>invokeaction</c>, <c>writeproperty</c>) and what members will be definitely delivered in the payload that is being received (e.g. <c>output</c> of <c>invokeaction</c>, <c>readproperty</c>)
        /// </summary>
        public string[] Required { get; set; }

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        public new Dictionary<string, DataSchema> Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        public new Dictionary<string, DataSchema> Default { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        public new Dictionary<string, DataSchema>[] Enum { get; set; }
    }

    /// <summary>
    ///     Metadata describing data of type <c>string</c>.
    /// </summary>
    /// <remarks>
    ///     This Subclass is indicated by the value <c>"string"</c> assigned to <c>Type</c> in <see cref="DataSchema"/> instances.
    /// </remarks>
    public class StringSchema : DataSchema, IStringSchema
    {
        /// <summary>
        ///     Base Constructor
        /// </summary>
        public StringSchema() { }

        /// <summary>
        ///     Assignment of JSON-based data types compatible with JSON Schema.
        /// </summary>
        /// <value>
        ///     <c>"string"</c>
        /// </value>
        public new readonly string Type = "string";

        /// <summary>
        /// Specifies the minimum length of a string.
        /// </summary>
        public uint? MinLength { get; set; }

        /// <summary>
        /// Specifies the maximum length of a string.
        /// </summary>
        public uint? MaxLength { get; set; }

        /// <summary>
        /// Provides a regular expression to express constraints of the string value. The regular expression must follow the [<see href="https://tc39.es/ecma262/multipage/">ECMA-262</see>] dialect.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Specifies the encoding used to store the contents, as specified in [<see href="https://www.rfc-editor.org/rfc/rfc2045">RFC2045</see>] (Section 6.1) and [<see href="https://www.rfc-editor.org/rfc/rfc4648">RFC4648</see>].
        /// </summary>
        public string ContentEncoding { get; set; }

        /// <summary>
        /// Specifies the MIME type of the contents of a string value, as described in [<see href="https://www.rfc-editor.org/rfc/rfc2046">RFC2046</see>].
        /// </summary>
        public string ContentMediaType { get; set; }

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        public new string Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        public new string Default { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        public new string[] Enum { get; set; }

    }

    /// <summary>
    ///     Metadata describing data of type <c>null</c>.
    /// </summary>
    /// <remarks>
    ///     This Subclass is indicated by the value <c>"null"</c> assigned to <c>Type</c> in <see cref="DataSchema"/> instances.
    ///     This Subclass describes only one acceptable value, namely null.
    ///     It is important to note that null does not mean the absence of a value. It is analogous to null in JavaScript, None in Python, null in Java, null in C# and nil in Ruby programming languages.
    ///     It can be used as part of a oneOf declaration, where it is used to indicate, that the data can also be <c>null</c>.
    /// </remarks>
    public class NullSchema : DataSchema
    {
        /// <summary>
        /// Base Constructor
        /// </summary>
        public NullSchema() { }

        /// <summary>
        /// 
        /// </summary>
        public new readonly string Type = "null";

    }
    public class InteractionAffordance : IInteractionAffordance
    {
        public InteractionAffordance() { }
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
        public PropertyAffordance() { }

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        public object Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        public object Default { get; set; }
        public string Unit { get; set; }
        public IDataSchema[] OneOf { get; set; }
        public IDataSchema[] AllOf { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
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
        public ArrayPropertyAffordance() { }
        public new readonly string Type = "array";
        public DataSchema[] Items { get; set; }
        public uint? MinItems { get; set; }
        public uint? MaxItems { get; set; }

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        public new List<object> Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        public new List<object> Default { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        public new List<object>[] Enum { get; set; }
    }
    public class BooleanPropertyAffordance : PropertyAffordance, IBooleanSchema
    {
        public BooleanPropertyAffordance() { }
        public new readonly string Type = "boolean";

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        public new bool Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        public new bool Default { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        public new bool[] Enum { get; set; }
    }
    public class NumberPropertyAffordance : PropertyAffordance, INumberSchema
    {
        public NumberPropertyAffordance() { }
        public new readonly string Type = "number";
        public double? Minimum { get; set; }
        public double? ExclusiveMinimum { get; set; }
        public double? Maximum { get; set; }
        public double? ExclusiveMaximum { get; set; }
        public double? MultipleOf { get; set; }

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        public new double? Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        public new double? Default { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        public new double[] Enum { get; set; }
    }
    public class IntegerPropertyAffordance : PropertyAffordance, IIntegerSchema
    {
        public IntegerPropertyAffordance() { }
        public new readonly string Type = "integer";
        public int? Minimum { get; set; }
        public int? ExclusiveMinimum { get; set; }
        public int? Maximum { get; set; }
        public int? ExclusiveMaximum { get; set; }
        public int? MultipleOf { get; set; }

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        public new int? Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        public new int? Default { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        public new int[] Enum { get; set; }
    }
    public class ObjectPropertyAffordance : PropertyAffordance, IObjectSchema
    {
        public ObjectPropertyAffordance() { }
        public new readonly string Type = "object";
        public Dictionary<string, DataSchema> Properties { get; set; }
        public string[] Required { get; set; }
        public new Dictionary<string, DataSchema> Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        public new Dictionary<string, DataSchema> Default { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        public new Dictionary<string, DataSchema>[] Enum { get; set; }
    }
    public class StringPropertyAffordance : PropertyAffordance, IStringSchema
    {
        public StringPropertyAffordance() { }
        public new readonly string Type = "string";
        public uint? MinLength { get; set; }
        public uint? MaxLength { get; set; }
        public string Pattern { get; set; }
        public string ContentEncoding { get; set; }
        public string ContentMediaType { get; set; }

        /// <summary>
        ///     Provides a constant value.
        /// </summary>
        public new string Const { get; set; }

        /// <summary>
        ///     Supply a default value. 
        /// </summary>
        /// <remarks>
        ///     The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        public new string Default { get; set; }

        /// <summary>
        ///     Restricted set of values provided as an array.
        /// </summary>
        public new string[] Enum { get; set; }

    }
    public class NullPropertyAffordance : PropertyAffordance
    {
        public new readonly string Type = "null";
        public NullPropertyAffordance() { }

    }

    [JsonConverter(typeof(ActionAffordanceConverter))]
    public class ActionAffordance : InteractionAffordance
    {
        public ActionAffordance() { }
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
        public EventAffordance() { }
        public DataSchema Subscription { get; set; }
        public DataSchema Data { get; set; }
        public DataSchema DataResponse { get; set; }
        public DataSchema Cancellation { get; set; }
        public new EventForm[] Forms { get; set; }

    }

    [JsonConverter(typeof(FormConverter))]
    public class Form
    {
        public Form() { }
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
        public PropertyForm() { }

    }

    [JsonConverter(typeof(ActionFormConverter))]
    public class ActionForm : Form
    {
        public ActionForm() { }
    }

    [JsonConverter(typeof(EventFormConverter))]
    public class EventForm : Form
    {
        public EventForm() { }
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
        public NoSecurityScheme() { }
        public new readonly string Scheme = "nosec";
    }

    public class BasicSecurityScheme : SecurityScheme
    {
        public BasicSecurityScheme() { }
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
        /// Base Constructor
        /// </summary>
        public ThingDescription() { }

        /// <summary>
        /// JSON-LD keyword to define short-hand names called terms that are used throughout a TD document.
        /// </summary>
        [JsonProperty("@context")]
        [JsonConverter(typeof(AtContextConverter))]
        public object[] AtContext { get; set; }

        ///<summary>
        ///JSON-LD keyword to label the object with semantic tags (or types).
        ///<summary>
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

        /// <summary>
        /// Provides additional (human-readable) information based on a default language.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Can be used to support (human-readable) information in different languages. Also see <seealso cref="MultiLanguage"/>.
        /// </summary>
        public MultiLanguage Descriptions { get; set; }

        /// <summary>
        /// Provides version information.
        /// </summary>
        public VersionInfo? Version { get; set; }

        /// <summary>
        /// Provides information when the TD instance was created.
        /// </summary>
        public DateTime? Created { get; set; }

        /// <summary>
        /// Provides information when the TD instance was last modified.
        /// </summary>
        public DateTime? Modified { get; set; }

        /// <summary>
        /// Provides information about the TD maintainer as URI scheme (e.g., mailto [<see href="https://www.rfc-editor.org/rfc/rfc6068">RFC6068</see>], tel [<see href="https://www.rfc-editor.org/rfc/rfc3966">RFC3966</see>], https [<see href="https://www.rfc-editor.org/rfc/rfc9112">RFC9112</see>]).
        /// </summary>
        public Uri Support { get; set; }

        /// <summary>
        ///  Define the base URI that is used for all relative URI references throughout a TD document.
        ///  <remarks>
        ///  In TD instances, all relative URIs are resolved relative to the base URI using the algorithm defined in [<see href="https://www.rfc-editor.org/rfc/rfc3986">RFC3986</see>].
        ///  <para>
        ///  <c>base</c> does not affect the URIs used in <c>@context</c> and the IRIs used within Linked Data [<see href="https://www.w3.org/DesignIssues/LinkedData.html">LINKED-DATA</see>] graphs that are relevant when semantic processing is applied to TD instances.
        ///  </para>
        ///  </remarks>
        /// </summary>
        public Uri Base { get; set; }

        /// <summary>
        /// All Property-based Interaction Affordances of the Thing.
        /// </summary>
        /// <see cref="InteractionAffordance"/>
        /// <see cref="PropertyAffordance"/>
        public Dictionary<string, PropertyAffordance> Properties { get; set; }

        /// <summary>
        /// All Action-based Interaction Affordances of the Thing. 
        /// </summary>
        /// <see cref="InteractionAffordance"/>
        /// <see cref="ActionAffordance"/>
        public Dictionary<string, ActionAffordance> Actions { get; set; }

        /// <summary>
        /// All Event-based Interaction Affordances of the Thing. 
        /// </summary>
        /// <see cref="InteractionAffordance"/>
        /// <see cref="EventAffordance"/>
        public Dictionary<string, EventAffordance> Events { get; set; }

        /// <summary>
        /// Provides Web links to arbitrary resources that relate to the specified Thing Description.
        /// </summary>
        public Link[] Links { get; set; }

        /// <summary>
        /// Set of form hypermedia controls that describe how an operation can be performed. Forms are serializations of Protocol Bindings. Thing level forms are used to describe endpoints for a group of interaction affordances.
        /// </summary>
        public Form[] Forms { get; set; }

        [JsonConverter(typeof(StringTypeConverter))]
        /// <summary>
        /// Set of security definition names, chosen from those defined in <c>securityDefinitions</c>. These must all be satisfied for access to resources.
        /// </summary>
        /// <seealso cref="SecurityDefinitions"/>
        public string[] Security { get; set; }

        /// <summary>
        /// Set of named security configurations (<c>definitions</c> only). Not actually applied unless names are used in a security name-value pair.
        /// </summary>
        /// <seealso cref="Security"/>
        /// <seealso cref="SecurityScheme"/>
        public Dictionary<string, SecurityScheme> SecurityDefinitions { get; set; }

        [JsonConverter(typeof(StringTypeConverter))]
        /// <summary>
        /// Indicates the WoT Profile mechanisms followed by this Thing Description and the corresponding Thing implementation.
        /// </summary>
        public string[] Profile { get; set; }

        /// <summary>
        /// Set of named data schemas. To be used in a <c>schema</c> name-value pair inside an <c>AdditionalExpectedResponse</c> object.
        /// </summary>
        /// <seealso cref="DataSchema"/>
        /// <seealso cref="AdditionalExpectedResponse"/>
        public Dictionary<string, DataSchema> SchemaDefinitions { get; set; }

        /// <summary>
        /// Define URI template variables according to [<see href="https://www.rfc-editor.org/rfc/rfc6570">RFC6570</see>] as collection based on <seealso cref="DataSchema">DataSchema</seealso> declarations.
        /// </summary>
        /// <remark>
        /// The Thing level <c>uriVariables</c> can be used in Thing level <c>forms</c> or in Interaction Affordances.
        /// The individual variables DataSchema cannot be an ObjectSchema or an ArraySchema since each variable needs to be serialized to a string inside the <c>href</c> upon the execution of the operation.
        /// If the same variable is both declared in Thing level uriVariables and in Interaction Affordance level, the Interaction Affordance level variable takes precedence. 
        /// </remarks>
        /// <seealso cref="DataSchema"/>
        public Dictionary<string, DataSchema> UriVariables { get; set; }

        /// <summary>
        /// Any properties not matching the explicitly defined in properties will be captured in the AdditionalProperties dictionary.
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, JToken> AdditionalProperties { get; set; }
    }

}