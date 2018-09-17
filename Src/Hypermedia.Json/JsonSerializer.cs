using System;
using System.Collections.Generic;
using Hypermedia.Json.Converters;
using JsonLite.Ast;

namespace Hypermedia.Json
{
    public sealed class JsonSerializer : IJsonSerializer
    {
        readonly IJsonConverterFactory _jsonConverterFactory;

        /// <summary>
        /// Constructor.
        /// </summary>
        public JsonSerializer() : this(new DefaultFieldNamingStrategy()) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonConverterFactory">The JSON converter factory.</param>
        public JsonSerializer(IJsonConverterFactory jsonConverterFactory)
        {
            _jsonConverterFactory = jsonConverterFactory;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fieldNamingStrategy">The field naming strategy.</param>
        public JsonSerializer(IFieldNamingStrategy fieldNamingStrategy)
        {
            _jsonConverterFactory = new JsonConverterFactory(
                JsonConverterFactory.Default,
                new ComplexConverter(fieldNamingStrategy));
        }

        /// <summary>
        /// Serialize an inline object.
        /// </summary>
        /// <param name="value">The value to serialization inline.</param>
        /// <returns>The JSON value which represents the inline serialization of the value.</returns>
        public JsonValue SerializeValue(object value)
        {
            return new Serializer(_jsonConverterFactory).SerializeValue(value);
        }

        /// <summary>
        /// Deserialize the given JSON value according to the specified CLR type.
        /// </summary>
        /// <param name="type">The CLR type to deserialize the JSON value to.</param>
        /// <param name="jsonValue">The JSON value to deserialize.</param>
        /// <returns>The CLR object that the JSON value was deserialized from.</returns>
        public object DeserializeValue(Type type, JsonValue jsonValue)
        {
            if (ReferenceEquals(jsonValue, JsonNull.Instance))
            {
                return null;
            }

            var converter = _jsonConverterFactory.CreateInstance(type);

            return converter.DeserializeValue(this, type, jsonValue);
        }

        #region Serializer

        class Serializer : IJsonSerializer
        {
            readonly IJsonConverterFactory _jsonConverterFactory;
            readonly Stack<object> _visited = new Stack<object>();

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="jsonConverterFactory">The JSON converter factory.</param>
            public Serializer(IJsonConverterFactory jsonConverterFactory)
            {
                _jsonConverterFactory = jsonConverterFactory;
            }

            /// <summary>
            /// Serialize an inline object.
            /// </summary>
            /// <param name="value">The value to serialization inline.</param>
            /// <returns>The JSON value which represents the inline serialization of the value.</returns>
            public JsonValue SerializeValue(object value)
            {
                if (ReferenceEquals(value, null) || _visited.Contains(value))
                {
                    return JsonNull.Instance;
                }

                _visited.Push(value);

                var type = value.GetType();
                var converter = _jsonConverterFactory.CreateInstance(type);

                var jsonValue = converter.SerializeValue(this, type, value);

                _visited.Pop();

                return jsonValue;
            }

            /// <summary>
            /// Deserialize the given JSON value according to the specified CLR type.
            /// </summary>
            /// <param name="type">The CLR type to deserialize the JSON value to.</param>
            /// <param name="jsonValue">The JSON value to deserialize.</param>
            /// <returns>The CLR object that the JSON value was deserialized from.</returns>
            object IJsonSerializer.DeserializeValue(Type type, JsonValue jsonValue)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}