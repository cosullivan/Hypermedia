using System;
using System.Collections.Generic;
using System.Linq;
using Hypermedia.Json;
using Hypermedia.Metadata;
using JsonLite.Ast;

namespace Hypermedia.WebApi.Json
{
    internal class ContractConverter : IJsonConverter
    {
        readonly IContractResolver _contractResolver;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contract resolver.</param>
        public ContractConverter(IContractResolver contractResolver)
        {
            _contractResolver = contractResolver;
        }

        /// <summary>
        /// Serialize the value.
        /// </summary>
        /// <param name="serializer">The serializer to utilize when serializing nested objects.</param>
        /// <param name="type">The CLR type of the value to serialize.</param>
        /// <param name="value">The value to serialize.</param>
        /// <returns>The JSON value that represents the given CLR value.</returns>
        public JsonValue SerializeValue(IJsonSerializer serializer, Type type, object value)
        {
            IContract contract;
            if (_contractResolver.TryResolve(type, out contract) == false)
            {
                throw new HypermediaWebApiException($"Could not resolve a contract for {type}.");
            }

            return new JsonObject(SerializeMembers(serializer, contract, value).Where(IsNotNull).ToList());
        }

        /// <summary>
        /// Serialize a list of members from the object.
        /// </summary>
        /// <param name="serializer">The serializer to utilize when serializing the values.</param>
        /// <param name="contract">The contract of the object to serialize the members from.</param>
        /// <param name="value">The value to serialize the members from.</param>
        /// <returns>The list of members that make up the object.</returns>
        static IEnumerable<JsonMember> SerializeMembers(IJsonSerializer serializer, IContract contract, object value)
        {
            foreach (var field in contract.Fields.Where(ShouldSerializeField))
            {
                yield return new JsonMember(field.Name, serializer.SerializeValue(field.GetValue(value)));
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
            return DeserializeObject(serializer, type, jsonValue);
        }

        /// <summary>
        /// Deserialize a JSON object.
        /// </summary>
        /// <param name="serializer">The serializer to utilize when deserializing nested objects.</param>
        /// <param name="type">The type of the object to deserialize to.</param>
        /// <param name="jsonValue">The JSON value to deserialize from.</param>
        /// <returns>The CLR object that represents the JSON object.</returns>
        object DeserializeObject(IJsonSerializer serializer, Type type, JsonValue jsonValue)
        {
            IContract contract;
            if (_contractResolver.TryResolve(type, out contract) == false)
            {
                throw new HypermediaWebApiException($"Could not resolve a contract for {type}.");
            }

            var instance = Activator.CreateInstance(type);

            DeserializeObject(serializer, (JsonObject)jsonValue, contract, instance);
            
            return instance;
        }

        /// <summary>
        /// Deserialize into the given instance.
        /// </summary>
        /// <param name="serializer">The serializer to utilize when deserializing nested objects.</param>
        /// <param name="jsonObject">The JSON object to deserialize from.</param>
        /// <param name="contract">The contract for the type that is being deserialized.</param>
        /// <param name="instance">The instance to deserialize into.</param>
        internal void DeserializeObject(IJsonSerializer serializer, JsonObject jsonObject, IContract contract, object instance)
        {
            DeserializeFields(serializer, jsonObject, contract.Fields.Where(ShouldDeserializeField).ToList(), instance);
        }

        /// <summary>
        /// Deserialize the fields for the given instance.
        /// </summary>
        /// <param name="serializer">The serializer to utilize when deserializing nested objects.</param>
        /// <param name="jsonObject">The JSON object to deserialize from.</param>
        /// <param name="fields">The list of fields to deserialize.</param>
        /// <param name="instance">The instance to deserialize into.</param>
        void DeserializeFields(IJsonSerializer serializer, JsonObject jsonObject, IReadOnlyList<IField> fields, object instance)
        {
            foreach (var member in jsonObject.Members)
            {
                var field = fields.SingleOrDefault(f => String.Equals(f.Name, member.Name, StringComparison.OrdinalIgnoreCase));

                field?.SetValue(instance, serializer.DeserializeValue(field.ClrType, member.Value));
            }
        }

        /// <summary>
        /// Returns a value indicating whether or not the converter can convert the given type.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        /// <returns>true if the type can be converted by this converter, false if not.</returns>
        public bool CanConvert(Type type)
        {
            return _contractResolver.CanResolve(type);
        }

        /// <summary>
        /// Returns a value indicating whether or not the given field should be included when serializing.
        /// </summary>
        /// <param name="field">The field to determine whether or not it should be included.</param>
        /// <returns>true if the field should be included, false if not.</returns>
        static bool ShouldSerializeField(IField field)
        {
            return field.Is(FieldOptions.CanSerialize);
        }

        /// <summary>
        /// Returns a value indicating whether the JSON Member has a non-null value.
        /// </summary>
        /// <param name="jsonMember">The JSON member to test.</param>
        /// <returns>true if the JSON member has a non-null value, false if not.</returns>
        static bool IsNotNull(JsonMember jsonMember)
        {
            return jsonMember.Value.GetType() != typeof(JsonNull);
        }

        /// <summary>
        /// Returns a value indicating whether or not the given field should be included when deserializing.
        /// </summary>
        /// <param name="field">The field to determine whether or not it should be included.</param>
        /// <returns>true if the field should be included, false if not.</returns>
        static bool ShouldDeserializeField(IField field)
        {
            return field.Is(FieldOptions.CanDeserialize);
        }
    }
}