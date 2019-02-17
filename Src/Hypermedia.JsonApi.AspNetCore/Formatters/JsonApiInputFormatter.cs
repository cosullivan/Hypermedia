using System;
using System.Diagnostics;
using Hypermedia.AspNetCore;
using Hypermedia.AspNetCore.Json.Formatters;
using Hypermedia.Json;
using Hypermedia.Metadata;
using JsonLite.Ast;

namespace Hypermedia.JsonApi.AspNetCore.Formatters
{
    public sealed class JsonApiInputFormatter : JsonInputFormatter
    {
        const string MediaTypeName = "application/vnd.api+json";

        readonly JsonApiSerializerOptions _serializerOptions;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contract resolver to use.</param>
        public JsonApiInputFormatter(IContractResolver contractResolver) : this(new JsonApiSerializerOptions { ContractResolver = contractResolver }) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serializerOptions">The serializer options to use.</param>
        public JsonApiInputFormatter(JsonApiSerializerOptions serializerOptions) : base(MediaTypeName, serializerOptions.ContractResolver, serializerOptions.FieldNamingStrategy)
        {
            _serializerOptions = serializerOptions;
        }

        /// <summary>
        /// Creates an instance of the patch object for the media type.
        /// </summary>
        /// <param name="type">The type of the inner instance that is being patched.</param>
        /// <param name="contractResolver">The contract resolver.</param>
        /// <param name="fieldNamingStrategy">The field naming strategy that is being used for the scope of the request.</param>
        /// <param name="jsonValue">The JSON value that represents the patch values.</param>
        /// <returns>The instance of the patch.</returns>
        protected override IPatch CreatePatch(Type type, IContractResolver contractResolver, IFieldNamingStrategy fieldNamingStrategy, JsonValue jsonValue)
        {
            var patch = typeof(JsonApiPatch<>).MakeGenericType(type.GenericTypeArguments[0]);

            var constructor = patch.GetConstructor(new[] { typeof(IContractResolver), typeof(IFieldNamingStrategy), typeof(JsonObject) });
            Debug.Assert(constructor != null);

            return (IPatch)constructor.Invoke(new object[] { ContractResolver, fieldNamingStrategy, jsonValue });
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
        /// Deserialize an object.
        /// </summary>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="fieldNamingStrategy">The field naming strategy that is being used for the scope of the request.</param>
        /// <param name="jsonValue">The JSON value that represents the object to deserialize.</param>
        protected override object DeserializeValue(Type type, IFieldNamingStrategy fieldNamingStrategy, JsonValue jsonValue)
        {
            var jsonObject = jsonValue as JsonObject;

            if (jsonObject == null)
            {
                throw new HypermediaAspNetCoreException("The top level JSON value must be an Object.");
            }

            var serializer = CreateJsonApiSerializer(fieldNamingStrategy);

            if (TypeHelper.IsEnumerable(type))
            {
                var collection = TypeHelper.CreateListInstance(type);

                foreach (var item in serializer.DeserializeMany(jsonObject))
                {
                    collection.Add(item);
                }

                return collection;
            }

            return serializer.Deserialize(jsonObject);
        }
    }
}