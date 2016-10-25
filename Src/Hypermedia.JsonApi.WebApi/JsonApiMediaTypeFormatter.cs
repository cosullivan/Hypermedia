using System;
using System.Collections;
using System.Diagnostics;
using System.Net.Http.Formatting;
using Hypermedia.Metadata;
using Hypermedia.WebApi;
using JsonLite.Ast;

namespace Hypermedia.JsonApi.WebApi
{
    public class JsonApiMediaTypeFormatter : Hypermedia.WebApi.Json.JsonMediaTypeFormatter
    {
        const string Name = "jsonapi";
        const string MediaTypeName = "application/vnd.api+json";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The resource contract resolver used to resolve the contracts at runtime.</param>
        public JsonApiMediaTypeFormatter(IContractResolver contractResolver) : base(Name, MediaTypeName, contractResolver, false) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The resource contract resolver used to resolve the contracts at runtime.</param>
        /// <param name="prettify">A value which indicates whether the output should be prettified.</param>
        JsonApiMediaTypeFormatter(IContractResolver contractResolver, bool prettify) : base(Name, MediaTypeName, contractResolver, prettify) { }

        /// <summary>
        /// Creates a per request formatter instance.
        /// </summary>
        /// <param name="contractResolver">The contract resolver to create the request with.</param>
        /// <param name="prettify">A value which indicates whether the output should be prettified.</param>
        /// <returns>The formatter instance to use specifically for the scope of a request.</returns>
        protected override MediaTypeFormatter CreatePerRequestInstance(IContractResolver contractResolver, bool prettify)
        {
            return new JsonApiMediaTypeFormatter(ContractResolver, prettify);
        }

        /// <summary>
        /// Creates an instance of the patch object for the media type.
        /// </summary>
        /// <param name="type">The type of the inner instance that is being patched.</param>
        /// <param name="contractResolver">The contract resolver.</param>
        /// <param name="jsonValue">The JSON value that represents the patch values.</param>
        /// <returns>The instance of the patch.</returns>
        protected override IPatch CreatePatch(Type type, IContractResolver contractResolver, JsonValue jsonValue)
        {
            var patch = typeof(JsonApiPatch<>).MakeGenericType(type.GenericTypeArguments[0]);

            var constructor = patch.GetConstructor(new[] { typeof(IContractResolver), typeof(JsonObject) });
            Debug.Assert(constructor != null);

            return (IPatch)constructor.Invoke(new object[] { ContractResolver, jsonValue });
        }

        /// <summary>
        /// Deserialize an object.
        /// </summary>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="jsonValue">The JSON value that represents the object to deserialize.</param>
        protected override object DeserializeValue(Type type, JsonValue jsonValue)
        {
            var jsonObject = jsonValue as JsonObject;

            if (jsonObject == null)
            {
                throw new HypermediaWebApiException("The top level JSON value must be an Object.");
            }

            var serializer = new JsonApiSerializer(ContractResolver);

            if (TypeHelper.IsEnumerable(type))
            {
                return serializer.DeserializeMany(jsonObject);
            }

            return serializer.DeserializeEntity(jsonObject);
        }

        /// <summary>
        /// Serialize the value into an JSON AST.
        /// </summary>
        /// <param name="type">The type to serialize from.</param>
        /// <param name="value">The value to serialize.</param>
        /// <returns>The JSON object that represents the serialized value.</returns>
        protected override JsonValue SerializeValue(Type type, object value)
        {
            var serializer = new JsonApiSerializer(ContractResolver);

            if (TypeHelper.IsEnumerable(type))
            {
                return serializer.SerializeMany((IEnumerable)value);
            }

            return serializer.SerializeEntity(value);
        }

        /// <summary>
        /// Returns a value indicating whether or not the dictionary has a metadata mapping for the given type.
        /// </summary>
        /// <param name="type">The element type to test for a mapping.</param>
        /// <returns>true if the given type has a mapping, false if not.</returns>
        protected override bool CanReadOrWrite(Type type)
        {
            return ContractResolver.CanResolve(TypeHelper.GetUnderlyingType(type));
        }
    }
}