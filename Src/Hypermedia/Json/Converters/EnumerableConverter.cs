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
            return DeserializeArray(serializer, type, (JsonArray)jsonValue);
        }

        ///// <summary>
        ///// Deserialize a JSON array.
        ///// </summary>
        ///// <param name="serializer">The serializer to utilize when deserializing nested objects.</param>
        ///// <param name="type">The type of the collection to deserialize to.</param>
        ///// <param name="jsonArray">The JSON array to deserialize from.</param>
        ///// <returns>The collection that represents the JSON array.</returns>
        //static ICollection DeserializeArray(IJsonSerializer serializer, Type type, JsonArray jsonArray)
        //{
        //    Type collectionType;
        //    if (TypeHelper.TryGetCollectionType(type, out collectionType) == false)
        //    {
        //        if (TypeHelper.IsEnumerable(type) == false)
        //        {
        //            throw new JsonException("Can not deserialize a JSON array to a type that doesnt support ICollection<T>.");
        //        }

        //        type = typeof(List<>).MakeGenericType(type.GenericTypeArguments[0]);
        //        collectionType = type;
        //    }

        //    var method = collectionType
        //        .GetTypeInfo()
        //            .DeclaredMethods
        //                .FirstOrDefault(m => m.DeclaringType == collectionType && m.Name == "Add");

        //    var elementType = collectionType.GenericTypeArguments[0];

        //    var collection = Activator.CreateInstance(type) as ICollection;

        //    foreach (var jsonValue in jsonArray)
        //    {
        //        var value = serializer.DeserializeValue(elementType, jsonValue);

        //        method.Invoke(collection, new[] { value });
        //    }

        //    return collection;
        //}

        /// <summary>
        /// Deserialize a JSON array.
        /// </summary>
        /// <param name="serializer">The serializer to utilize when deserializing nested objects.</param>
        /// <param name="type">The type of the collection to deserialize to.</param>
        /// <param name="jsonArray">The JSON array to deserialize from.</param>
        /// <returns>The collection that represents the JSON array.</returns>
        static ICollection DeserializeArray(IJsonSerializer serializer, Type type, JsonArray jsonArray)
        {
            // TODO: the collection access should be converted to a dynamically compiled delegate
            Type elementType;
            MethodInfo method;
            if (TryGetCollectionType(type, out type, out elementType, out method) == false)
            {
                throw new JsonException("Can not deserialize a JSON array to a type that doesnt support ICollection<T>.");
            }

            var collection = Activator.CreateInstance(type) as ICollection;

            foreach (var jsonValue in jsonArray)
            {
                var value = serializer.DeserializeValue(elementType, jsonValue);

                method.Invoke(collection, new[] { value });
            }

            return collection;
        }

        /// <summary>
        /// Extract the relevant information such that the collection type can be used dynamically.
        /// </summary>
        /// <param name="type">The property type to deserialize into.</param>
        /// <param name="collectionType">The collection type to create.</param>
        /// <param name="elementType">The element type of the collection.</param>
        /// <param name="method">The add method that can be used to dynamically add the items to the collection.</param>
        /// <returns>true if the collection information could be found for the type, false if not.</returns>
        static bool TryGetCollectionType(Type type, out Type collectionType, out Type elementType, out MethodInfo method)
        {
            method = null;
            elementType = null;

            if (TypeHelper.TryGetCollectionType(type, out collectionType))
            {
                var t = collectionType;

                method = collectionType
                    .GetTypeInfo()
                        .DeclaredMethods
                            .FirstOrDefault(m => m.DeclaringType == t && m.Name == "Add");

                elementType = collectionType.GenericTypeArguments[0];

                return elementType != null && method != null;
            }

            if (type.GetTypeInfo().IsGenericType && type.GetTypeInfo().GetGenericTypeDefinition() == typeof(IReadOnlyList<>))
            {
                elementType = type.GenericTypeArguments[0];
                collectionType = typeof(List<>).MakeGenericType(elementType);
                method = collectionType.GetRuntimeMethod("Add", new [] { elementType });

                return elementType != null && collectionType != null && method != null;
            }

            return false;
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
