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
                return new JsonNumber((short)value);
            }

            if (type == typeof(ushort))
            {
                return new JsonNumber((ushort)value);
            }

            if (type == typeof (int))
            {
                return new JsonNumber((int)value);
            }

            if (type == typeof(long))
            {
                return new JsonNumber((long)value);
            }

            if (type == typeof (decimal))
            {
                return new JsonNumber((decimal)value);
            }

            if (type == typeof(float))
            {
                return new JsonNumber((decimal)(float)value);
            }

            if (type == typeof (double))
            {
                return new JsonNumber((decimal)(double)value);
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

            if (type == typeof(TimeSpan))
            {
                return new JsonString(((TimeSpan)value).ToString());
            }

            throw new NotSupportedException(type.ToString());
        }

        /// <summary>
        /// Deserialize a JSON value to a defined CLR type.
        /// </summary>
        /// <param name="deserializer">The deserializer to utilize when deserializing nested objects.</param>
        /// <param name="type">The CLR type to deserialize the JSON value to.</param>
        /// <param name="jsonValue">The JSON value to deserialize.</param>
        /// <returns>The object that represents the CLR version of the given JSON value.</returns>
        public object DeserializeValue(IJsonDeserializer deserializer, Type type, JsonValue jsonValue)
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
                return (short)((JsonNumber)jsonValue).Value;
            }

            if (type == typeof(ushort))
            {
                return (ushort)((JsonNumber)jsonValue).Value;
            }

            if (type == typeof(int))
            {
                return (int)((JsonNumber)jsonValue).Value;
            }

            if (type == typeof(long))
            {
                return (long)((JsonNumber)jsonValue).Value;
            }

            if (type == typeof(decimal))
            {
                return ((JsonNumber)jsonValue).Value;
            }

            if (type == typeof(float))
            {
                return (float)((JsonNumber)jsonValue).Value;
            }

            if (type == typeof(double))
            {
                return (double)((JsonNumber)jsonValue).Value;
            }

            if (type == typeof(DateTime))
            {
                return DateTime.Parse(((JsonString)jsonValue).Value);
            }

            if (type == typeof(DateTimeOffset))
            {
                return DateTimeOffset.Parse(((JsonString)jsonValue).Value);
            }

            if (type == typeof(TimeSpan))
            {
                return TimeSpan.Parse(((JsonString)jsonValue).Value);
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
                || type == typeof(DateTimeOffset)
                || type == typeof(TimeSpan);
        }
    }
}