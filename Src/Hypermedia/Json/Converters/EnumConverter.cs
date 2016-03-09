using System;
using System.Reflection;
using JsonLite.Ast;

namespace Hypermedia.Json.Converters
{
    internal sealed class EnumConverter : IJsonConverter
    {
        internal static readonly IJsonConverter Instance = new EnumConverter();

        /// <summary>
        /// Serialize the value.
        /// </summary>
        /// <param name="serializer">The serializer to utilize when serializing nested objects.</param>
        /// <param name="type">The CLR type of the value to serialize.</param>
        /// <param name="value">The value to serialize.</param>
        /// <returns>The JSON value that represents the given CLR value.</returns>
        public JsonValue SerializeValue(IJsonSerializer serializer, Type type, object value)
        {
            return new JsonString(value.ToString());
        }

        /// <summary>
        /// Deserialize a JSON value to a defined CLR type.
        /// </summary>
        /// <param name="serializer">The serializer to utilize when deserializing nested objects.</param>
        /// <param name="type">The CLR type to deserialize the JSON value to.</param>
        /// <param name="jsonValue">The JSON value to deserialize.</param>
        /// <returns>The object that represents the CLR version of the given JSON value.</returns>
        public object DeserializeValue(IJsonSerializer serializer, Type type, JsonValue jsonValue)
        {
            var text = ((JsonString)jsonValue).Value;

            return Enum.Parse(type, text, true);
        }

        /// <summary>
        /// Returns a value indicating whether or not the converter can convert the given type.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        /// <returns>true if the type can be converted by this converter, false if not.</returns>
        public bool CanConvert(Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }
    }
}
