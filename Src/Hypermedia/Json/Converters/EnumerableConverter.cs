using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JsonLite.Ast;

namespace Hypermedia.Json.Converters
{
    internal sealed class EnumerableConverter : IJsonConverter
    {
        internal static readonly IJsonConverter Instance = new EnumerableConverter();

        /// <summary>
        /// Serialize the value.
        /// </summary>
        /// <param name="serializer">The serializer to utilize when serializing nested objects.</param>
        /// <param name="type">The CLR type of the value to serialize.</param>
        /// <param name="value">The value to serialize.</param>
        /// <returns>The JSON value that represents the given CLR value.</returns>
        public JsonValue SerializeValue(IJsonSerializer serializer, Type type, object value)
        {
            return new JsonArray(SerializeValue(serializer, (IEnumerable)value).ToList());
        }

        /// <summary>
        /// Serialize a list of values.
        /// </summary>
        /// <param name="serializer">The serializer to utilize when serializing the values.</param>
        /// <param name="values">The list of values to serialize to the array.</param>
        /// <returns>The list of values that make up the array.</returns>
        static IEnumerable<JsonValue> SerializeValue(IJsonSerializer serializer, IEnumerable values)
        {
            foreach (var value in values)
            {
                yield return serializer.SerializeValue(value);
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
            var jsonArray = (JsonArray) jsonValue;

            if (type.IsArray)
            {
                return DeserializeArray(serializer, type, jsonArray);
            }

            if (TypeHelper.IsCollection(type))
            {
                return DeserializeCollection(serializer, type, jsonArray);
            }

            if (type.GetTypeInfo().IsGenericType && type.GetTypeInfo().GetGenericTypeDefinition() == typeof(IReadOnlyList<>))
            {
                type = typeof(List<>).MakeGenericType(type.GenericTypeArguments[0]);

                return DeserializeValue(serializer, type, jsonArray);
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Deserialize a JSON array into an array.
        /// </summary>
        /// <param name="serializer">The serializer to utilize when deserializing nested objects.</param>
        /// <param name="type">The type of the array.</param>
        /// <param name="jsonArray">The JSON array to deserialize from.</param>
        /// <returns>The collection that represents the JSON array.</returns>
        static Array DeserializeArray(IJsonSerializer serializer, Type type, JsonArray jsonArray)
        {
            var array = Array.CreateInstance(type.GetElementType(), jsonArray.Count);

            for (var i = 0; i < jsonArray.Count; i++)
            {
                var value = serializer.DeserializeValue(type.GetElementType(), jsonArray[i]);

                array.SetValue(value, i);
            }

            return array;
        }

        /// <summary>
        /// Deserialize a JSON array into a collection.
        /// </summary>
        /// <param name="serializer">The serializer to utilize when deserializing nested objects.</param>
        /// <param name="type">The type of the collection to create.</param>
        /// <param name="jsonArray">The JSON array to deserialize from.</param>
        /// <returns>The collection that represents the JSON array.</returns>
        static ICollection DeserializeCollection(IJsonSerializer serializer, Type type, JsonArray jsonArray)
        {
            Type collectionType;
            if (TypeHelper.TryGetCollectionType(type, out collectionType) == false)
            {
                throw new JsonException("Can not deserialize a JSON array to a type that doesnt support ICollection<T>.");
            }

            var method = collectionType
                .GetTypeInfo()
                    .DeclaredMethods
                        .FirstOrDefault(m => m.DeclaringType == collectionType && m.Name == "Add");

            var elementType = collectionType.GenericTypeArguments[0];

            var collection = (ICollection)Activator.CreateInstance(type);

            foreach (var jsonValue in jsonArray)
            {
                var value = serializer.DeserializeValue(elementType, jsonValue);

                method.Invoke(collection, new[] { value });
            }

            return collection;
        }

        /// <summary>
        /// Returns a value indicating whether or not the converter can convert the given type.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        /// <returns>true if the type can be converted by this converter, false if not.</returns>
        public bool CanConvert(Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces.Any(t => t == typeof(IEnumerable));
        }
    }
}