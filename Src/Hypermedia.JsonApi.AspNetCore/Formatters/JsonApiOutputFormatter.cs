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

        readonly JsonApiSerializerOptions _serializerOptions;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contract resolver to use.</param>
        public JsonApiOutputFormatter(IContractResolver contractResolver) : this(new JsonApiSerializerOptions { ContractResolver = contractResolver }) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serializerOptions">The serializer options.</param>
        public JsonApiOutputFormatter(JsonApiSerializerOptions serializerOptions) 
            : base(JsonApiMediaTypeName, serializerOptions.ContractResolver, serializerOptions.FieldNamingStrategy)
        {
            _serializerOptions = serializerOptions;
        }

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
        /// Create an instance of a serializer with the specified field naming strategy as an override.
        /// </summary>
        /// <param name="fieldNamingStrategy">The field naming strategy to override with.</param>
        /// <returns>The serializer instance to use.</returns>
        JsonApiSerializer CreateJsonApiSerializer(IFieldNamingStrategy fieldNamingStrategy)
        {
            if (_serializerOptions.FieldNamingStrategy == fieldNamingStrategy)
            {
                return new JsonApiSerializer(_serializerOptions);
            }

            var serializerOptions = _serializerOptions.Clone();
            
            serializerOptions.FieldNamingStrategy = fieldNamingStrategy;

            return new JsonApiSerializer(serializerOptions);
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
            var serializer = CreateJsonApiSerializer(fieldNamingStrategy);

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