using System;
using System.Collections.Generic;
using JsonLite.Ast;

namespace Hypermedia.Json
{
    public sealed class JsonSerializer : IJsonSerializer
    {
        readonly IJsonConverterFactory _jsonConverterFactory;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public JsonSerializer() : this(new JsonConverterFactory()) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonConverterFactory">The JSON converter factory.</param>
        public JsonSerializer(IJsonConverterFactory jsonConverterFactory)
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
            readonly HashSet<object> _visited = new HashSet<object>();

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
                // TODO: what is the best way to treat an already serialized entity? should it be scoped more to a parent instance
                
                if (ReferenceEquals(value, null) || HasVisited(value))
                {
                    return JsonNull.Instance;
                }

                Visit(value);

                var type = value.GetType();
                var converter = _jsonConverterFactory.CreateInstance(type);

                return converter.SerializeValue(this, type, value);
            }

            /// <summary>
            /// Returns a value indicating whether the instance has been visited.
            /// </summary>
            /// <param name="instance">The instance to determined whether it has been visitied.</param>
            /// <returns>true if the entity has been visitied, false if not.</returns>
            bool HasVisited(object instance)
            {
                return _visited.Contains(instance);
            }

            /// <summary>
            /// Marks the entity as having being visited.
            /// </summary>
            /// <param name="instance">The instance that has been visited.</param>
            void Visit(object instance)
            {
                _visited.Add(instance);
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
