using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JsonLite.Ast;

namespace Hypermedia.Json.Converters
{
    internal sealed class ComplexConverter : IJsonConverter
    {
        internal static readonly IJsonConverter Instance = new ComplexConverter();

        /// <summary>
        /// Serialize the value.
        /// </summary>
        /// <param name="serializer">The serializer to utilize when serializing nested objects.</param>
        /// <param name="type">The CLR type of the value to serialize.</param>
        /// <param name="value">The value to serialize.</param>
        /// <returns>The JSON value that represents the given CLR value.</returns>
        public JsonValue SerializeValue(IJsonSerializer serializer, Type type, object value)
        {
            return new JsonObject(SerializeMembers(serializer, type, value).ToList());
        }

        /// <summary>
        /// Serialize a list of members from the object.
        /// </summary>
        /// <param name="serializer">The serializer to utilize when serializing the values.</param>
        /// <param name="type">The type of the object to serialize the members from.</param>
        /// <param name="value">The value to serialize the members from.</param>
        /// <returns>The list of members that make up the object.</returns>
        static IEnumerable<JsonMember> SerializeMembers(IJsonSerializer serializer, Type type, object value)
        {
            foreach (var property in type.GetRuntimeProperties().Where(p => p.CanRead))
            {
                yield return new JsonMember(serializer.FieldNamingStrategy.GetName(property.Name), serializer.SerializeValue(property.GetValue(value)));
            }
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
            return DeserializeObject(serializer, type, (JsonObject)jsonValue);
        }

        /// <summary>
        /// Deserialize a JSON object.
        /// </summary>
        /// <param name="serializer">The serializer to utilize when deserializing nested objects.</param>
        /// <param name="type">The type of the object to deserialize to.</param>
        /// <param name="jsonObject">The JSON object to deserialize from.</param>
        /// <returns>The CLR object that represents the JSON object.</returns>
        static object DeserializeObject(IJsonSerializer serializer, Type type, JsonObject jsonObject)
        {
            var entity = Activator.CreateInstance(type);

            foreach (var member in jsonObject.Members)
            {
                var property = type.GetRuntimeProperty(serializer.FieldNamingStrategy.ResolveName(member.Name));

                if (property != null)
                {
                    property.SetValue(entity, serializer.DeserializeValue(property.PropertyType, member.Value));
                    continue;
                }

                if (entity is IJsonExtension jsonExtension)
                {
                    jsonExtension.Data = jsonExtension.Data ?? new List<JsonMember>();
                    jsonExtension.Data.Add(member);
                }
            }

            return entity;
        }

        /// <summary>
        /// Returns a value indicating whether or not the converter can convert the given type.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        /// <returns>true if the type can be converted by this converter, false if not.</returns>
        public bool CanConvert(Type type)
        {
            return true;
        }
    }
}