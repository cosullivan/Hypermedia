using System;
using System.Collections;
using System.Linq;
using Hypermedia.AspNetCore.Json.Formatters;
using Hypermedia.Json;
using Hypermedia.Metadata;
using JsonLite.Ast;

namespace Hypermedia.JsonApi.AspNetCore.Formatters
{
    public sealed class JsonApiOutputFormatter : JsonOutputFormatter
    {
        public const string JsonApiMediaTypeName = "application/vnd.api+json";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contract resolver to use.</param>
        public JsonApiOutputFormatter(IContractResolver contractResolver) : this(contractResolver, DasherizedFieldNamingStrategy.Instance) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contract resolver to use.</param>
        /// <param name="fieldNamingStrategy">The field naming strategy when serializing and deserializing the JSON.</param>
        public JsonApiOutputFormatter(IContractResolver contractResolver, IFieldNamingStrategy fieldNamingStrategy) 
            : base(JsonApiMediaTypeName, contractResolver, fieldNamingStrategy) { }

        /// <summary>
        /// Serialize the value into an JSON AST.
        /// </summary>
        /// <param name="type">The type to serialize from.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="fieldNamingStrategy">The field naming strategy when serializing and deserializing the JSON.</param>
        /// <returns>The JSON object that represents the serialized value.</returns>
        protected override JsonValue SerializeValue(Type type, object value, IFieldNamingStrategy fieldNamingStrategy)
        {
            if (ContractResolver.CanResolve(TypeHelper.GetUnderlyingType(type)))
            {
                return SerializeContract(type, value, fieldNamingStrategy);
            }

            if (TypeHelper.GetUnderlyingType(type) == typeof(JsonApiError))
            {
                return SerializeJsonApiError(type, value);
            }

            throw new HypermediaException("Not supported.");
        }

        /// <summary>
        /// Serialize the value into an JSON AST.
        /// </summary>
        /// <param name="type">The type to serialize from.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="fieldNamingStrategy">The field naming strategy when serializing and deserializing the JSON.</param>
        /// <returns>The JSON object that represents the serialized value.</returns>
        JsonValue SerializeContract(Type type, object value, IFieldNamingStrategy fieldNamingStrategy)
        {
            var serializer = new JsonApiSerializer(ContractResolver, fieldNamingStrategy);

            if (TypeHelper.IsEnumerable(type))
            {
                return serializer.SerializeMany((IEnumerable)value);
            }

            return serializer.SerializeEntity(value);
        }

        /// <summary>
        /// Serialize the value into an JSON AST.
        /// </summary>
        /// <param name="type">The type to serialize from.</param>
        /// <param name="value">The value to serialize.</param>
        /// <returns>The JSON object that represents the serialized value.</returns>
        JsonValue SerializeJsonApiError(Type type, object value)
        {
            if (TypeHelper.IsEnumerable(type))
            {
                return JsonApiErrorSerializer.Instance.SerializeMany(((IEnumerable)value).OfType<JsonApiError>());
            }

            return JsonApiErrorSerializer.Instance.Serialize((JsonApiError)value);
        }

        /// <summary>
        /// Returns a value indicating whether or not the given type can be written by this serializer.
        /// </summary>
        /// <param name="type">The object type.</param>
        /// <returns><c>true</c> if the type can be written, otherwise <c>false</c>.</returns>
        protected override bool CanWriteType(Type type)
        {
            return base.CanWriteType(type) || type == typeof(JsonApiError);
        }
    }
}