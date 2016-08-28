using System;
using System.Reflection;
using JsonLite.Ast;

namespace Hypermedia.Json.Converters
{
    internal class PrimitiveConverter : IJsonConverter
    {
        internal static readonly IJsonConverter Instance = new PrimitiveConverter();

        /// <summary>
        /// Serialize the value.
        /// </summary>
        /// <param name="serializer">The serializer to utilize when serializing nested objects.</param>
        /// <param name="type">The CLR type of the value to serialize.</param>
        /// <param name="value">The value to serialize.</param>
        /// <returns>The JSON value that represents the given CLR value.</returns>
        public JsonValue SerializeValue(IJsonSerializer serializer, Type type, object value)
        {
            if (type == typeof(string))
            {
                return new JsonString((string)value);
            }

            if (type == typeof (Guid))
            {
                return new JsonString(value.ToString());
            }

            if (type == typeof(short))
            {
                return new JsonInteger((short)value);
            }

            if (type == typeof(ushort))
            {
                return new JsonInteger((ushort)value);
            }

            if (type == typeof (int))
            {
                return new JsonInteger((int)value);
            }

            if (type == typeof(long))
            {
                return new JsonInteger((long)value);
            }

            if (type == typeof (decimal))
            {
                return new JsonDecimal((decimal)value);
            }

            if (type == typeof(float))
            {
                return new JsonDecimal((decimal)(float)value);
            }

            if (type == typeof (double))
            {
                return new JsonDecimal((decimal)(double)value);
            }

            if (type == typeof(bool))
            {
                return new JsonBoolean((bool)value);
            }

            if (type == typeof(DateTime))
            {
                return new JsonString(((DateTime)value).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK"));
            }

            if (type == typeof(DateTimeOffset))
            {
                return new JsonString(((DateTimeOffset)value).UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssK"));
            }

            throw new NotSupportedException(type.ToString());
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
            if (type == typeof(string))
            {
                return ((JsonString)jsonValue).Value;
            }

            if (type == typeof(Guid))
            {
                return new Guid(((JsonString)jsonValue).Value);
            }

            if (type == typeof(short))
            {
                return (short)((JsonInteger)jsonValue).Value;
            }

            if (type == typeof(ushort))
            {
                return (ushort)((JsonInteger)jsonValue).Value;
            }

            if (type == typeof(int))
            {
                return (int)((JsonInteger)jsonValue).Value;
            }

            if (type == typeof(long))
            {
                return ((JsonInteger)jsonValue).Value;
            }

            if (type == typeof(decimal))
            {
                return ((JsonDecimal)jsonValue).Value;
            }

            if (type == typeof(float))
            {
                return (float)((JsonDecimal)jsonValue).Value;
            }

            if (type == typeof(double))
            {
                return (double)((JsonDecimal)jsonValue).Value;
            }

            if (type == typeof(DateTime))
            {
                return DateTime.Parse(((JsonString)jsonValue).Value);
            }

            if (type == typeof(DateTimeOffset))
            {
                return DateTimeOffset.Parse(((JsonString)jsonValue).Value);
            }

            if (type == typeof(bool))
            {
                return ((JsonBoolean)jsonValue).Value;
            }

            throw new NotSupportedException(type.ToString());
        }

        /// <summary>
        /// Returns a value indicating whether or not the converter can convert the given type.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        /// <returns>true if the type can be converted by this converter, false if not.</returns>
        public bool CanConvert(Type type)
        {
            return type.GetTypeInfo().IsPrimitive 
                || type == typeof(decimal) 
                || type == typeof(string)
                || type == typeof(Guid)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset);
        }
    }
}
