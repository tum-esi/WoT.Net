using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WoT.Core.Helpers;

namespace WoT.Core.Definitions.TD
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
    /// An interface for <c>DataSchema</c> as described in the Thing Description (TD) <see href="https://www.w3.org/TR/wot-thing-description11/">Spec</see>.
    /// Is needed mainly to ensure that <see cref="PropertyAffordance">PropertyAffordances</see> also implement <see cref="DataSchema"/> interface.
    /// </summary>
    public interface IDataSchema
    {
        /// <summary>
        /// JSON-LD keyword to label the object with semantic tags (or types)
        /// </summary>
        [JsonProperty("@type")]
        string[] AtType { get; set; }

        /// <summary>
        /// Provides a human-readable title (e.g., display a text for UI representation) based on a default language.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Provides multi-language human-readable titles (e.g., display a text for UI representation in different languages).
        /// </summary>
        /// <seealso cref="MultiLanguage"/>
        MultiLanguage Titles { get; set; }

        /// <summary>
        /// Provides additional (human-readable) information based on a default language.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Can be used to support (human-readable) information in different languages.
        /// </summary>
        /// <seealso cref="MultiLanguage"/>
        MultiLanguage Descriptions { get; set; }

        /// <summary>
        /// Provides a constant value.
        /// </summary>
        object Const { get; set; }

        /// <summary>
        /// Supply a default value. 
        /// </summary>
        /// <remarks>
        /// The value SHOULD validate against the data schema in which it resides.
        /// </remarks>
        object Default { get; set; }

        /// <summary>
        /// Provides unit information that is used, e.g., in international science, engineering, and business.
        /// </summary>
        /// <remarks>
        /// To preserve uniqueness, it is recommended that the value of the unit points to a semantic definition (also see Section <see href="https://www.w3.org/TR/wot-thing-description11/#semantic-annotations-example-version-units">Semantic Annotations</see>).
        /// </remarks>
        string Unit { get; set; }

        /// <summary>
        /// Used to ensure that the data is valid against one of the specified schemas in the array.
        /// </summary>
        /// <remarks>
        /// This can be used to describe multiple input or output schemas.
        /// </remarks>
        IDataSchema[] OneOf { get; set; }

        /// <summary>
        /// Restricted set of values provided as an array.
        /// </summary>
        object[] Enum { get; set; }

        /// <summary>
        /// Boolean value that is a hint to indicate whether a property interaction / value is read only (=true) or not (=false).
        /// </summary>
        bool ReadOnly { get; set; }

        /// <summary>
        /// Boolean value that is a hint to indicate whether a property interaction / value is write only (=true) or not (=false).
        /// </summary>
        bool WriteOnly { get; set; }

        /// <summary>
        /// Allows validation based on a format pattern such as <c>"date-time"</c>, <c>"email"</c>, <c>"uri"</c>, etc.
        /// </summary>
        string Format { get; set; }

        /// <summary>
        /// Assignment of JSON-based data types compatible with JSON Schema (one of <c>"boolean"</c>, <c>"integer"</c>, <c>"number"</c>, <c>"string"</c>, <c>"object"</c>, <c>"array"</c>, or <c>"null"</c>).
        /// </summary>
        string Type { get; }

    }

    /// <summary>
    /// An interface for <c>ArraySchema</c> as described in the Thing Description (TD) <see href="https://www.w3.org/TR/wot-thing-description11/">Spec</see>.
    /// Is needed mainly to ensure that  <see cref="ArrayPropertyAffordance">ArrayPropertyAffordances</see> also implement <see cref="ArraySchema"/> interface.
    /// </summary>
    public interface IArraySchema : IDataSchema
    {
        /// <summary>
        /// Assignment of JSON-based data types compatible with JSON Schema.
        /// </summary>
        /// <value>
        ///     Schema type: <c>"array"</c>
        /// </value>
        new string Type { get; }

        /// <summary>
        /// Used to define the characteristics of an array.
        /// </summary>
        DataSchema[] Items { get; set; }

        /// <summary>
        /// Defines the minimum number of items that have to be in the array.
        /// </summary>
        uint? MinItems { get; set; }

        /// <summary>
        /// Defines the maximum number of items that have to be in the array.
        /// </summary>
        uint? MaxItems { get; set; }

        /// <inheritdoc cref="IDataSchema.Const"/>
        new List<object> Const { get; set; }

        /// <inheritdoc cref="IDataSchema.Default"/>
        new List<object> Default { get; set; }

        /// <inheritdoc cref="IDataSchema.Enum"/>
        new List<object>[] Enum { get; set; }

    }

    /// <summary>
    /// An interface for <c>ArraySchema</c> as described in the Thing Description (TD) <see href="https://www.w3.org/TR/wot-thing-description11/">Spec</see>.
    /// Is needed mainly to ensure that <see cref="BooleanPropertyAffordance">BooleanPropertyAffordances</see> also implement <see cref="BooleanSchema"/> interface.
    /// </summary>
    public interface IBooleanSchema : IDataSchema
    {
        /// <summary>
        /// Assignment of JSON-based data types compatible with JSON Schema.
        /// </summary>
        /// <value>
        ///     Schema type: <c>"boolean"</c>
        /// </value>
        new string Type { get; }

        /// <inheritdoc cref="IDataSchema.Const"/>
        new bool Const { get; set; }

        /// <inheritdoc cref="IDataSchema.Default"/>
        new bool Default { get; set; }
        /// <inheritdoc cref="IDataSchema.Enum"/>
        new bool[] Enum { get; set; }
    }

    /// <summary>
    /// An interface for <c>NumberSchema</c> as described in the Thing Description (TD) <see href="https://www.w3.org/TR/wot-thing-description11/">Spec</see>.
    /// Is needed mainly to ensure that <see cref="NumberPropertyAffordance">NumberPropertyAffordances</see> also implement <see cref="NumberSchema"/> interface.
    /// </summary>
    public interface INumberSchema : IDataSchema
    {
        /// <summary>
        /// Assignment of JSON-based data types compatible with JSON Schema.
        /// </summary>
        /// <value>
        ///     Schema type: <c>"number"</c>
        /// </value>
        new string Type { get; }

        /// <summary>
        /// Specifies a minimum numeric value, representing an inclusive lower limit.
        /// </summary>
        double? Minimum { get; set; }

        /// <summary>
        /// Specifies a minimum numeric value, representing an exclusive lower limit.
        /// </summary>
        double? ExclusiveMinimum { get; set; }

        /// <summary>
        /// Specifies a maximum numeric value, representing an inclusive upper limit.
        /// </summary>
        double? Maximum { get; set; }

        /// <summary>
        /// Specifies a maximum numeric value, representing an exclusive upper limit.
        /// </summary>
        double? ExclusiveMaximum { get; set; }


        /// <summary>
        /// Specifies the multipleOf value number. The value must strictly greater than 0.
        /// </summary>
        double? MultipleOf { get; set; }

        /// <inheritdoc cref="IDataSchema.Const"/>
        new double? Const { get; set; }

        /// <inheritdoc cref="IDataSchema.Default"/>
        new double? Default { get; set; }

        /// <inheritdoc cref="IDataSchema.Enum"/>
        new double[] Enum { get; set; }
    }

    /// <summary>
    /// An interface for <c>NumberSchema</c> as described in the Thing Description (TD) <see href="https://www.w3.org/TR/wot-thing-description11/">Spec</see>.
    /// Is needed mainly to ensure that <see cref="IntegerPropertyAffordance">IntegerPropertyAffordances</see> also implement <see cref="IntegerSchema"/> interface.
    /// </summary>
    public interface IIntegerSchema : IDataSchema
    {
        /// <summary>
        /// Assignment of JSON-based data types compatible with JSON Schema.
        /// </summary>
        /// <value>
        ///     Schema type: <c>"integer"</c>
        /// </value>
        new string Type { get; }

        /// <summary>
        /// Specifies a minimum numeric value, representing an inclusive lower limit.
        /// </summary>
        int? Minimum { get; set; }

        /// <summary>
        /// Specifies a minimum numeric value, representing an exclusive lower limit.
        /// </summary>
        int? ExclusiveMinimum { get; set; }

        /// <summary>
        /// Specifies a maximum numeric value, representing an inclusive upper limit.
        /// </summary>
        int? Maximum { get; set; }

        /// <summary>
        /// Specifies a maximum numeric value, representing an exclusive upper limit.
        /// </summary>
        int? ExclusiveMaximum { get; set; }

        /// <summary>
        /// Specifies the multipleOf value number. The value must strictly greater than 0.
        /// </summary>
        int? MultipleOf { get; set; }

        /// <inheritdoc cref="IDataSchema.Const"/>
        new int? Const { get; set; }

        /// <inheritdoc cref="IDataSchema.Default"/>
        new int? Default { get; set; }

        /// <inheritdoc cref="IDataSchema.Enum"/>
        new int[] Enum { get; set; }
    }

    /// <summary>
    /// An interface for <c>ObjectSchema</c> as described in the Thing Description (TD) <see href="https://www.w3.org/TR/wot-thing-description11/">Spec</see>.
    /// Is needed mainly to ensure that <see cref="ObjectPropertyAffordance">ObjectPropertyAffordances</see> also implement <see cref="ObjectSchema"/> interface.
    /// </summary>
    /// <remarks>
    /// JSON <c>object</c> type is NOT the same as <c>object</c> type in C&#35;. JSON <c>object</c> maps to C&#35; <c> Dictionary&lt;string, object&gt; </c>.
    /// </remarks>
    public interface IObjectSchema : IDataSchema
    {
        /// <summary>
        /// Assignment of JSON-based data types compatible with JSON Schema.
        /// </summary>
        /// <value>
        ///     Schema type: <c>"object"</c>
        /// </value>
        new string Type { get; }

        /// <summary>
        /// Data schema nested definitions.
        /// </summary>
        Dictionary<string, DataSchema> Properties { get; set; }


        /// <summary>
        /// Defines which members of the object type are mandatory, i.e. which members are mandatory in the payload that is to be sent (e.g. <c>input</c> of <c>invokeaction</c>, <c>writeproperty</c>) and what members will be definitely delivered in the payload that is being received (e.g. <c>output</c> of <c>invokeaction</c>, <c>readproperty</c>)
        /// </summary>
        string[] Required { get; set; }

        /// <inheritdoc cref="IDataSchema.Const"/>
        new Dictionary<string, object> Const { get; set; }

        /// <inheritdoc cref="IDataSchema.Default"/>
        new Dictionary<string, object> Default { get; set; }

        /// <inheritdoc cref="IDataSchema.Enum"/>
        new Dictionary<string, object>[] Enum { get; set; }
    }

    /// <summary>
    /// An interface for <c>StringSchema</c> as described in the Thing Description (TD) <see href="https://www.w3.org/TR/wot-thing-description11/">Spec</see>.
    /// Is needed mainly to ensure that <see cref="StringPropertyAffordance">StringPropertyAffordances</see> also implement <see cref="StringSchema"/> interface.
    /// </summary>
    public interface IStringSchema : IDataSchema
    {
        /// <summary>
        /// Assignment of JSON-based data types compatible with JSON Schema.
        /// </summary>
        /// <value>
        ///     Schema type: <c>"string"</c>
        /// </value>
        new string Type { get; }

        /// <summary>
        /// Specifies the minimum length of a string.
        /// </summary>
        uint? MinLength { get; set; }

        /// <summary>
        /// Specifies the maximum length of a string.
        /// </summary>
        uint? MaxLength { get; set; }

        /// <summary>
        /// Provides a regular expression to express constraints of the string value. The regular expression must follow the [<see href="https://tc39.es/ecma262/multipage/">ECMA-262</see>] dialect.
        /// </summary>
        string Pattern { get; set; }

        /// <summary>
        /// Specifies the encoding used to store the contents, as specified in [<see href="https://www.rfc-editor.org/rfc/rfc2045">RFC2045</see>] (Section 6.1) and [<see href="https://www.rfc-editor.org/rfc/rfc4648">RFC4648</see>].
        /// </summary>
        string ContentEncoding { get; set; }

        /// <summary>
        /// Specifies the MIME type of the contents of a string value, as described in [<see href="https://www.rfc-editor.org/rfc/rfc2046">RFC2046</see>].
        /// </summary>
        string ContentMediaType { get; set; }

        /// <inheritdoc cref="IDataSchema.Const"/>
        new string Const { get; set; }

        /// <inheritdoc cref="IDataSchema.Default"/>
        new string Default { get; set; }

        /// <inheritdoc cref="IDataSchema.Enum"/>
        new string[] Enum { get; set; }

    }

    /// <summary>
    /// An interface for <c>InteractionAffordances</c> as described in the Thing Description (TD) <see href="https://www.w3.org/TR/wot-thing-description11/">Spec</see>.
    /// </summary>
    public interface IInteractionAffordance
    {

        /// <summary>
        /// JSON-LD keyword to label the object with semantic tags (or types).
        /// </summary>
        [JsonProperty("@type")]
        string[] AtType { get; set; }

        /// <summary>
        /// Provides a human-readable title (e.g., display a text for UI representation) based on a default language.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Provides multi-language human-readable titles (e.g., display a text for UI representation in different languages).
        /// </summary>
        /// <seealso cref="MultiLanguage"/>
        MultiLanguage Titles { get; set; }

        /// <summary>
        /// Provides additional (human-readable) information based on a default language.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Provides multi-language human-readable titles (e.g., display a text for UI representation in different languages).
        /// </summary>
        /// <seealso cref="MultiLanguage"/>
        MultiLanguage Descriptions { get; set; }

        /// <summary>
        /// Set of form hypermedia controls that describe how an operation can be performed. Forms are serializations of Protocol Bindings.
        /// </summary>
        /// <remarks>
        /// The array cannot be empty.
        /// </remarks>
        Form[] Forms { get; set; }

        /// <summary>
        /// Define URI template variables according to [<see href="https://www.rfc-editor.org/rfc/rfc6570">RFC6570</see>] as collection based on <see cref="DataSchema"/> declarations.
        /// </summary>
        /// <remarks>
        /// The Thing level <c>uriVariables</c> can be used in Thing level <c>forms</c> or in Interaction Affordances.
        /// The individual variables DataSchema cannot be an ObjectSchema or an ArraySchema since each variable needs to be serialized to a string inside the <c>href</c> upon the execution of the operation.
        /// If the same variable is both declared in Thing level uriVariables and in Interaction Affordance level, the Interaction Affordance level variable takes precedence. 
        /// </remarks>
        /// <seealso cref="DataSchema"/>
        Dictionary<string, DataSchema> UriVariables { get; set; }

        /// <summary>
        /// A property to hold the original string of the InteractionAffordance JSON
        /// </summary>
        /// <value>
        /// Original JSON String
        /// </value>
        string OriginalJson { get; set; }
    }


    /// <summary>
    /// Metadata that describes the data format used. It can be used for validation.
    /// </summary>
    [JsonConverter(typeof(DataSchemaConverter))]
    public class DataSchema : IDataSchema
    {
        /// <inheritdoc/>
        public DataSchema() { }

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
        public object[] Enum { get; set; }
        public bool ReadOnly { get; set; }
        public bool WriteOnly { get; set; }
        public string Format { get; set; }
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
        /// <inheritdoc/>
        public ArraySchema() { }
        public new string Type { get => "array"; }
        public DataSchema[] Items { get; set; }
        public uint? MinItems { get; set; }
        public uint? MaxItems { get; set; }
        public new List<object> Const { get; set; }
        public new List<object> Default { get; set; }
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
        public BooleanSchema() { }
        public new string Type { get => "boolean"; }
        public new bool Const { get; set; }
        public new bool Default { get; set; }
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
        /// <inheritdoc/>
        public NumberSchema() { }
        public new string Type { get => "number"; }
        public double? Minimum { get; set; }
        public double? ExclusiveMinimum { get; set; }
        public double? Maximum { get; set; }
        public double? ExclusiveMaximum { get; set; }
        public double? MultipleOf { get; set; }
        public new double? Const { get; set; }
        public new double? Default { get; set; }
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
        /// <inheritdoc/>
        public IntegerSchema() { }
        public new string Type { get => "integer"; }
        public int? Minimum { get; set; }
        public int? ExclusiveMinimum { get; set; }
        public int? Maximum { get; set; }
        public int? ExclusiveMaximum { get; set; }
        public int? MultipleOf { get; set; }
        public new int? Const { get; set; }
        public new int? Default { get; set; }
        public new int[] Enum { get; set; }
    }

    /// <summary>
    ///     Metadata describing data of type <c>object</c>.
    /// </summary>
    /// <remarks>
    ///     This Subclass is indicated by the value <c>"object"</c> assigned to <c>Type</c> in <see cref="DataSchema"/> instances.
    ///     <para>
    ///     JSON <c>object</c> type is NOT the same as <c>object</c> type in C&#35;. JSON <c>object</c> maps to C&#35; <c> Dictionary&lt;string, object&gt; </c>.
    ///     </para>
    /// </remarks>
    public class ObjectSchema : DataSchema, IObjectSchema
    {
        /// <inheritdoc/>
        public ObjectSchema() { }
        public new string Type { get => "object"; }
        public Dictionary<string, DataSchema> Properties { get; set; }
        public string[] Required { get; set; }
        public new Dictionary<string, object> Const { get; set; }
        public new Dictionary<string, object> Default { get; set; }
        public new Dictionary<string, object>[] Enum { get; set; }
    }

    /// <summary>
    ///     Metadata describing data of type <c>string</c>.
    /// </summary>
    /// <remarks>
    ///     This Subclass is indicated by the value <c>"string"</c> assigned to <c>Type</c> in <see cref="DataSchema"/> instances.
    /// </remarks>
    public class StringSchema : DataSchema, IStringSchema
    {
        /// <inheritdoc/>
        public StringSchema() { }
        public new string Type { get => "string"; }
        public uint? MinLength { get; set; }
        public uint? MaxLength { get; set; }
        public string Pattern { get; set; }
        public string ContentEncoding { get; set; }
        public string ContentMediaType { get; set; }
        public new string Const { get; set; }
        public new string Default { get; set; }
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
        /// <inheritdoc/>
        public NullSchema() { }
        /// <summary>
        /// Assignment of JSON-based data types compatible with JSON Schema.
        /// </summary>
        /// <value>
        ///     Schema type: <c>"null"</c>
        /// </value>
        public new string Type { get => "null"; }

    }

    /// <summary>
    /// Metadata of a Thing that shows the possible choices to Consumers, thereby suggesting how Consumers may interact with the Thing. There are many types of potential affordances, but W3C WoT defines three types of Interaction Affordances: Properties, Actions, and Events.
    /// </summary>
    public class InteractionAffordance : IInteractionAffordance
    {
        /// <inheritdoc/>
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

    /// <summary>
    /// A Subclass of <see cref="InteractionAffordance"/> that exposes state of the Thing. This state can then be retrieved (read) and/or updated (write). Things can also choose to make Properties observable by pushing the new state after a change.
    /// </summary>
    /// <remarks>
    /// Property instances are also instances of the class <see cref="DataSchema"/>.
    /// Therefore, it can contain the <c>type</c>, <c>unit</c>, <c>readOnly</c> and <c>writeOnly</c> members, among others.
    /// </remarks>
    [JsonConverter(typeof(PropertyAffordanceConverter))]
    public class PropertyAffordance : InteractionAffordance, IDataSchema
    {
        /// <inheritdoc/>
        public PropertyAffordance() { }
        public object Const { get; set; }
        public object Default { get; set; }
        public string Unit { get; set; }
        public IDataSchema[] OneOf { get; set; }
        public object[] Enum { get; set; }
        public bool ReadOnly { get; set; }
        public bool WriteOnly { get; set; }
        public string Format { get; set; }
        public string Type { get; set; }

        /// <summary>
        /// A hint that indicates whether Servients hosting the Thing and Intermediaries should provide a Protocol Binding that supports the <c>observeproperty</c> and <c>unobserveproperty</c> operations for this Property.
        /// </summary>
        /// <value>
        /// If Property is observable or not.
        /// </value>
        public bool Observable { get; set; }

        /// <inheritdoc cref="InteractionAffordance.Forms"/>
        public new PropertyForm[] Forms { get; set; }

    }

    /// <summary>
    /// A Subclass of <see cref="PropertyAffordance"/> that also implements the DataSchema interface <see cref="IArraySchema"/>
    /// </summary>
    public class ArrayPropertyAffordance : PropertyAffordance, IArraySchema
    {
        /// <inheritdoc/>
        public ArrayPropertyAffordance() { }
        public new string Type { get => "array"; }
        public DataSchema[] Items { get; set; }
        public uint? MinItems { get; set; }
        public uint? MaxItems { get; set; }
        public new List<object> Const { get; set; }
        public new List<object> Default { get; set; }
        public new List<object>[] Enum { get; set; }
    }

    /// <summary>
    /// A Subclass of <see cref="PropertyAffordance"/> that also implements the DataSchema interface <see cref="IBooleanSchema"/>
    /// </summary>
    public class BooleanPropertyAffordance : PropertyAffordance, IBooleanSchema
    {
        /// <inheritdoc/>
        public BooleanPropertyAffordance() { }
        public new string Type { get => "boolean"; }
        public new bool Const { get; set; }
        public new bool Default { get; set; }
        public new bool[] Enum { get; set; }
    }

    /// <summary>
    /// A Subclass of <see cref="PropertyAffordance"/> that also implements the DataSchema interface <see cref="INumberSchema"/>
    /// </summary>
    public class NumberPropertyAffordance : PropertyAffordance, INumberSchema
    {
        /// <inheritdoc/>
        public NumberPropertyAffordance() { }
        public new string Type { get => "number"; }
        public double? Minimum { get; set; }
        public double? ExclusiveMinimum { get; set; }
        public double? Maximum { get; set; }
        public double? ExclusiveMaximum { get; set; }
        public double? MultipleOf { get; set; }
        public new double? Const { get; set; }
        public new double? Default { get; set; }
        public new double[] Enum { get; set; }
    }

    /// <summary>
    /// A Subclass of <see cref="PropertyAffordance"/> that also implements the DataSchema interface <see cref="IIntegerSchema"/>
    /// </summary>
    public class IntegerPropertyAffordance : PropertyAffordance, IIntegerSchema
    {
        /// <inheritdoc/>
        public IntegerPropertyAffordance() { }
        public new string Type { get => "integer"; }
        public int? Minimum { get; set; }
        public int? ExclusiveMinimum { get; set; }
        public int? Maximum { get; set; }
        public int? ExclusiveMaximum { get; set; }
        public int? MultipleOf { get; set; }
        public new int? Const { get; set; }
        public new int? Default { get; set; }
        public new int[] Enum { get; set; }
    }

    /// <summary>
    /// A Subclass of <see cref="PropertyAffordance"/> that also implements the DataSchema interface <see cref="IObjectSchema"/>
    /// </summary>
    public class ObjectPropertyAffordance : PropertyAffordance, IObjectSchema
    {
        /// <inheritdoc/>
        public ObjectPropertyAffordance() { }
        public new string Type { get => "object"; }
        public Dictionary<string, DataSchema> Properties { get; set; }
        public string[] Required { get; set; }
        public new Dictionary<string, object> Const { get; set; }
        public new Dictionary<string, object> Default { get; set; }
        public new Dictionary<string, object>[] Enum { get; set; }
    }

    /// <summary>
    /// A Subclass of <see cref="PropertyAffordance"/> that also implements the DataSchema interface <see cref="IStringSchema"/>
    /// </summary>
    public class StringPropertyAffordance : PropertyAffordance, IStringSchema
    {
        /// <inheritdoc/>
        public StringPropertyAffordance() { }
        public new string Type { get => "string"; }
        public uint? MinLength { get; set; }
        public uint? MaxLength { get; set; }
        public string Pattern { get; set; }
        public string ContentEncoding { get; set; }
        public string ContentMediaType { get; set; }
        public new string Const { get; set; }
        public new string Default { get; set; }
        public new string[] Enum { get; set; }

    }

    /// <summary>
    /// A Subclass of <see cref="PropertyAffordance"/> that represents a <c>null</c> Property
    /// </summary>
    public class NullPropertyAffordance : PropertyAffordance
    {
        public new string Type { get => "null"; }
        /// <inheritdoc/>
        public NullPropertyAffordance() { }
    }



    /// <summary>
    /// A Subclass of <see cref="InteractionAffordance"/> representing an Interaction Affordance that allows to invoke a function of the Thing, which manipulates state (e.g., toggling a lamp on or off) or triggers a process on the Thing (e.g., dim a lamp over time).
    /// </summary>
    [JsonConverter(typeof(ActionAffordanceConverter))]
    public class ActionAffordance : InteractionAffordance
    {
        /// <inheritdoc/>
        public ActionAffordance() { }

        /// <summary>
        /// Used to define the input data schema of the Action.
        /// </summary>
        /// <value>
        /// metadata describing input payload
        /// </value>
        public DataSchema Input { get; set; }

        /// <summary>
        /// Used to define the output data schema of the Action.
        /// </summary>
        /// <value>
        /// metadata describing output payload
        /// </value> 
        public DataSchema Output { get; set; }

        /// <summary>
        /// Signals if the Action is safe (=true) or not. Used to signal if there is no internal state (cf. resource state) is changed when invoking an Action. In that case responses can be cached as example.
        /// </summary>
        /// <value>
        /// indication of safety
        /// </value>
        public bool Safe { get; set; }

        /// <summary>
        /// Indicates whether the Action is idempotent (=true) or not. Informs whether the Action can be called repeatedly with the same result, if present, based on the same input.
        /// </summary>
        /// <value>
        /// indication of idempotency
        /// </value>
        public bool Idempotent { get; set; }

        /// <summary>
        /// Indicates whether the action is synchronous (=true) or not. A synchronous action means that the response of action contains all the information about the result of the action and no further querying about the status of the action is needed. Lack of this keyword means that no claim on the synchronicity of the action can be made.
        /// </summary>
        /// <value>
        /// indication of synchrony
        /// </value>
        public bool? Synchronous { get; set; }

        /// <inheritdoc cref="InteractionAffordance.Forms"/>
        public new ActionForm[] Forms { get; set; }


    }

    /// <summary>
    /// A Subclass of <see cref="InteractionAffordance"/> that describes an event source, which asynchronously pushes event data to Consumers (e.g., overheating alerts).
    /// </summary>
    [JsonConverter(typeof(EventAffordanceConverter))]
    public class EventAffordance : InteractionAffordance
    {
        /// <inheritdoc/>
        public EventAffordance() { }

        /// <summary>
        /// Defines data that needs to be passed upon subscription, e.g., filters or message format for setting up Webhooks.
        /// </summary>
        /// <value>
        /// metadata describing subscription payload
        /// </value>
        public DataSchema Subscription { get; set; }

        /// <summary>
        /// Defines the data schema of the Event instance messages pushed by the Thing.
        /// </summary>
        /// <value>
        /// metadata describing notification payload (data received)
        /// </value>
        public DataSchema Data { get; set; }

        /// <summary>
        /// Defines the data schema of the Event response messages sent by the consumer in a response to a data message.
        /// </summary>
        /// <value>
        /// metadata describing notification response payload (data sent after each notification)
        /// </value>
        public DataSchema DataResponse { get; set; }

        /// <summary>
        /// Defines any data that needs to be passed to cancel a subscription, e.g., a specific message to remove a Webhook.
        /// </summary>
        /// <value>
        /// metadata describing cancellation payload
        /// </value>
        public DataSchema Cancellation { get; set; }

        /// <inheritdoc cref="InteractionAffordance.Forms"/>
        public new EventForm[] Forms { get; set; }

    }

    /// <summary>
    /// A form can be viewed as a statement of "To perform an <i><b>operation type</b></i> operation on <i><b>form context</b></i>, make a <i><b>request method</b></i> request to <i><b>submission target</b></i>" where the optional <i><b>form fields</b></i> may further describe the required request.
    /// In Thing Descriptions, the <i><b>form context</b></i> is the surrounding Object, such as Properties, Actions, and Events or the Thing itself for meta-interactions.
    /// </summary>
    [JsonConverter(typeof(FormConverter))]
    public class Form
    {
        /// <inheritdoc/>
        public Form() { }

        /// <summary>
        /// Target IRI of a link or submission target of a form.
        /// </summary>
        /// <value>
        /// target IRI
        /// </value>
        public Uri Href { get; set; }

        /// <summary>
        /// Assign a content type based on a media type (e.g., text/plain) and potential parameters (e.g., charset=utf-8) for the media type [<see href="https://www.rfc-editor.org/rfc/rfc2046">RFC2046</see>].
        /// </summary>
        /// <value>
        /// content media type
        /// </value>
        public string ContentType { get; set; }

        /// <summary>
        /// Content coding values indicate an encoding transformation that has been or can be applied to a representation. Content codings are primarily used to allow a representation to be compressed or otherwise usefully transformed without losing the identity of its underlying media type and without loss of information. Examples of content coding include "gzip", "deflate", etc. 
        /// </summary>
        /// <value>
        /// encoding type
        /// </value>
        public string ContentCoding { get; set; }

        /// <summary>
        /// Set of security definition names, chosen from those defined in securityDefinitions. These must all be satisfied for access to resources.
        /// </summary>
        /// <value>
        /// security definition names
        /// </value>
        public string[] Security { get; set; }

        /// <summary>
        /// Set of authorization scope identifiers provided as an array. These are provided in tokens returned by an authorization server and associated with forms in order to identify what resources a client may access and how. The values associated with a form SHOULD be chosen from those defined in an OAuth2SecurityScheme active on that form.
        /// </summary>
        /// <value>
        /// scope identifiers
        /// </value>
        public string[] Scopes { get; set; }

        /// <summary>
        /// This optional term can be used if, e.g., the output communication metadata differ from input metadata (e.g., output contentType differ from the input contentType). The response name contains metadata that is only valid for the primary response messages.
        /// </summary>
        /// <value>
        /// metadata describing primary response payload
        /// </value>
        public ExpectedResponse? Response { get; set; }

        /// <summary>
        /// This optional term can be used if additional expected responses are possible, e.g. for error reporting. Each additional response needs to be distinguished from others in some way (for example, by specifying a protocol-specific error code), and may also have its own data schema.
        /// </summary>
        /// <value>
        /// metadata describing additional expected responses
        /// </value> 
        public AdditionalExpectedResponse[] AdditionalExpectedResponse { get; set; }

        /// <summary>
        /// Indicates the exact mechanism by which an interaction will be accomplished for a given protocol when there are multiple options. For example, for HTTP and Events, it indicates which of several available mechanisms should be used for asynchronous notifications such as long polling (longpoll), WebSub [<see href="https://www.w3.org/TR/websub/">websub</see>] (websub), Server-Sent Events (sse) [<see href="https://html.spec.whatwg.org/multipage/">html</see>] (also known as EventSource). Please note that there is no restriction on the subprotocol selection and other mechanisms can also be announced by this subprotocol term.
        /// </summary>
        /// <value>
        /// the subprotocol
        /// </value>
        public string Subprotocol { get; set; }

        /// <summary>
        /// A property to hold the original string of the Form JSON
        /// </summary>
        /// <value>
        /// original JSON string
        /// </value>
        public string OriginalJson { get; set; }

        /// <summary>
        /// Indicates the semantic intention of performing the operation(s) described by the form. For example, the Property interaction allows get and set operations. The protocol binding may contain a form for the get operation and a different form for the set operation. The op attribute indicates which form is for which and allows the client to select the correct form for the operation required. op can be assigned one or more interaction verb(s) each representing a semantic intention of an operation.
        /// </summary>
        /// <value>
        /// the operation
        /// </value>
        public string[] Op { get; set; }


    }

    public struct ClientAndForm
    {
        public IProtocolClient protocolClient { get; }
        public Form form { get; }

        public ClientAndForm(IProtocolClient protocolClient, Form form) { this.protocolClient = protocolClient; this.form = form; }
    }

    /// <summary>
    /// A Helper Subclass of <see cref="Form"/> for properties used for serialization
    /// </summary>
    [JsonConverter(typeof(PropertyFormConverter))]
    public class PropertyForm : Form
    {
        /// <inheritdoc/>
        public PropertyForm() { }

    }

    /// <summary>
    /// A Helper Subclass of <see cref="Form"/> for actions used for serialization
    /// </summary>
    [JsonConverter(typeof(ActionFormConverter))]
    public class ActionForm : Form
    {
        /// <inheritdoc/>
        public ActionForm() { }
    }

    /// <summary>
    /// A Helper Subclass of <see cref="Form"/> for events used for serialization
    /// </summary>
    [JsonConverter(typeof(EventFormConverter))]
    public class EventForm : Form
    {
        /// <inheritdoc/>
        public EventForm() { }
    }

    /// <summary>
    /// A link can be viewed as a statement of the form "<i><b>link context</b></i> has a <i><b>relation type</b></i> resource at <i><b>link target</b></i>", where the optional <i><b>target attributes</b></i> may further describe the resource.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/wot-thing-description11/#link">TD Specification</seealso>
    public struct Link
    {
        /// <summary>
        /// Target IRI of a link or submission target of a form.
        /// </summary>
        /// <value>
        /// target IRI
        /// </value>
        public Uri Href { get; set; }

        /// <summary>
        /// Target attribute providing a hint indicating what the media type [<see href="https://www.rfc-editor.org/rfc/rfc2046">RFC2046</see>] of the result of dereferencing the link should be.
        /// </summary>
        /// <value>
        /// target media type
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// A link relation type identifies the semantics of a link.
        /// </summary>
        /// <value>
        /// link relation type
        /// </value>
        public string Rel { get; set; }

        /// <summary>
        /// Overrides the link context (by default the Thing itself identified by its id) with the given URI or IRI.
        /// </summary>
        /// <value>
        /// anchor URI
        /// </value>
        public Uri Anchor { get; set; }

        /// <summary>
        /// Target attribute that specifies one or more sizes for the referenced icon. Only applicable for relation type "icon". The value pattern follows {Height}x{Width} (e.g., "16x16", "16x16 32x32").
        /// </summary>
        /// <value>
        /// icon size following pattern {Height}x{Width}
        /// </value>
        public string Sizes { get; set; }

        /// <summary>
        /// The hreflang attribute specifies the language of a linked document. The value of this must be a valid language tag [<see href="https://www.rfc-editor.org/rfc/rfc5646">BCP47</see>].
        /// </summary>
        /// <value>
        /// language tag
        /// </value>
        [JsonConverter(typeof(StringTypeConverter))]
        public string[] Hreflang { get; set; }
    }


    /// <summary>
    /// Metadata of a Thing that provides version information about the TD document. 
    /// If required, additional version information such as firmware and hardware version (term definitions outside of the TD namespace) can be extended via the TD Context Extension mechanism.
    /// </summary>
    public struct VersionInfo
    {
        /// <summary>
        /// Provides a version indicator of this TD.
        /// </summary>
        /// <value>
        /// version of this TD instance
        /// </value>
        public string Instance { get; set; }

        /// <summary>
        /// Provides a version indicator of the underlying TM.
        /// </summary>
        /// <value>
        /// version of underlying model
        /// </value>
        public string Model { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="VersionInfo"/> with the given instance version.
        /// Sets <see cref="Model"/> to <c>null</c>
        /// </summary>
        /// <param name="instance">version of TD instance</param>
        public VersionInfo(string instance)
        {
            Instance = instance;
            Model = null;
        }


        /// <summary>
        /// Creates a new instance of <see cref="VersionInfo"/> with the given instance and model version.
        /// </summary>
        /// <param name="instance">version of TD instance</param>
        /// <param name="model">version of underlying TM</param>
        public VersionInfo(string instance, string model)
        {
            Instance = instance;
            Model = model;
        }

        /// <summary>
        /// Creates a new instance of <see cref="VersionInfo"/> with the given <c>VersionInfo</c>
        /// </summary>
        /// <param name="versionInfo">info struct</param>
        public VersionInfo(VersionInfo versionInfo)
        {
            this = versionInfo;
        }
    }


    /// <summary>
    /// Communication metadata describing the expected response message for the primary response.
    /// </summary>
    public struct ExpectedResponse
    {
        /// <summary>
        /// Assign a content type based on a media type (e.g., text/plain) and potential parameters (e.g., charset=utf-8) for the media type [<see href="https://www.rfc-editor.org/rfc/rfc2046">RFC2046</see>].
        /// </summary>
        /// <value>
        /// content media type
        /// </value>
        public string ContentType { get; set; }

        /// <summary>
        /// Creates a new <see cref="ExpectedResponse"/>. Takes <c>contentType</c> as input.
        /// </summary>
        /// <param name="contentType">content type of response</param>
        public ExpectedResponse(string contentType)
        {
            ContentType = contentType;
        }
    }

    /// <summary>
    /// Communication metadata describing the expected response message for additional responses.
    /// </summary>
    public struct AdditionalExpectedResponse
    {
        /// <summary>
        /// Assign a content type based on a media type (e.g., text/plain) and potential parameters (e.g., charset=utf-8) for the media type [<see href="https://www.rfc-editor.org/rfc/rfc2046">RFC2046</see>].
        /// </summary>
        /// <value>
        /// content media type
        /// </value>
        public string ContentType { get; set; }

        /// <summary>
        /// Signals if an additional response should not be considered an error.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Used to define the output data schema for an additional response if it differs from the default output data schema. Rather than a DataSchema object, the name of a previous definition given in a schemaDefinitions map must be used.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Creates a new <see cref="AdditionalExpectedResponse"/>. Takes <c>contentType</c> as input.
        /// </summary>
        /// <param name="contentType">content type of response</param>
        public AdditionalExpectedResponse(string contentType)
        {
            ContentType = contentType;
            Success = false;
            Schema = null;
        }
    }


    /// <summary>
    /// Metadata describing the configuration of a security mechanism.
    /// </summary>
    /// <seealso cref="https://www.w3.org/TR/wot-thing-description11/#securityscheme"/>
    [JsonConverter(typeof(SecuritySchemeConverter))]
    public abstract class SecurityScheme
    {
        /// <summary>
        /// JSON-LD keyword to label the object with semantic tags (or types).
        /// </summary>
        /// <value>
        /// semantic tags
        /// </value>
        [JsonProperty("@type")]
        public string[] AtType { get; set; }

        /// <summary>
        /// Provides additional (human-readable) information based on a default language.
        /// </summary>
        /// <value>
        /// scheme description
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Can be used to support (human-readable) information in different languages. Also see <seealso cref="MultiLanguage"/>
        /// </summary>
        /// <value>
        /// a mapping of language tags and corresponding scheme descriptions
        /// </value>
        public MultiLanguage Descriptions { get; set; }

        /// <summary>
        /// URI of the proxy server this security configuration provides access to. If not given, the corresponding security configuration is for the endpoint.
        /// </summary>
        /// <value>
        /// URI of proxy server
        /// </value>
        public Uri Proxy { get; set; }

        /// <summary>
        /// Identification of the security mechanism being configured.
        /// </summary>
        /// <value>
        /// type of security mechanism
        /// </value>
        public string Scheme { get; set; }
    }

    /// <summary>
    /// A security configuration corresponding to identified by the Vocabulary Term <c>nosec</c> (i.e., <c>"scheme": "nosec"</c>), indicating there is no authentication or other mechanism required to access the resource.
    /// </summary>
    public class NoSecurityScheme : SecurityScheme
    {
        /// <inheritdoc/>
        public NoSecurityScheme() { }

        /// <summary>
        /// Identification of the security mechanism being configured.
        /// </summary>
        /// <value>
        /// type of security mechanism: <c>"nosec"</c>
        /// </value>
        public new string Scheme { get => "nosec"; }
    }

    /// <summary>
    /// Basic Authentication [<see href="https://httpwg.org/specs/rfc7617.html">RFC7617</see>] security configuration identified by the Vocabulary Term <c>basic</c> (i.e., <c>"scheme": "basic"</c>), using an unencrypted username and password.
    /// </summary>
    public class BasicSecurityScheme : SecurityScheme
    {
        /// <inheritdoc/>
        public BasicSecurityScheme() { }

        /// <summary>
        /// Identification of the security mechanism being configured.
        /// </summary>
        /// <value>
        /// type of security mechanism: <c>"basic"</c>
        /// </value>
        public new string Scheme { get => "basic"; }

        /// <summary>
        /// Name for query, header, cookie, or uri parameters.
        /// </summary>
        /// <value>
        /// designated name
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Specifies the location of security authentication information.
        /// </summary>
        /// <value>
        /// location of security authentication information.
        /// <para>
        /// Possible values:
        /// <list type="bullet">
        ///     <item><c>"header"</c></item>
        ///     <item><c>"query"</c></item>
        ///     <item><c>"body"</c></item>
        ///     <item><c>"cookie"</c></item>
        ///     <item><c>"auto"</c></item>
        /// </list>
        /// </para>
        /// </value>
        public string In { get; set; }
    }

    /// <summary>
    /// Class <c> ThingDescription </c> is a DTO representing metadata and interfaces of a Thing
    /// </summary>
    public class ThingDescription
    {
        /// <inheritdoc/>
        public ThingDescription() { }

        /// <summary>
        /// JSON-LD keyword to define short-hand names called terms that are used throughout a TD document.
        /// </summary>
        [JsonProperty("@context")]
        [JsonConverter(typeof(AtContextConverter))]
        public object[] AtContext { get; set; }

        ///<summary>
        ///JSON-LD keyword to label the object with semantic tags (or types).
        ///</summary>
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
        /// Define URI template variables according to [<see href="https://www.rfc-editor.org/rfc/rfc6570">RFC6570</see>] as collection based on <see cref="DataSchema"/> declarations.
        /// </summary>
        /// <remarks>
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