using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Hypermedia.Json;
using Hypermedia.Metadata;
using Hypermedia.WebApi;
using JsonLite.Ast;

namespace Hypermedia.JsonApi.WebApi
{
    public class JsonApiMediaTypeFormatter : Hypermedia.WebApi.Json.JsonMediaTypeFormatter
    {
        readonly IFieldNamingStrategy _fieldNamingStratgey;
        const string Name = "jsonapi";
        const string MediaTypeName = "application/vnd.api+json";
        const string FieldNamingStrategyParameterName = "$fieldnamingstrategy";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The resource contract resolver used to resolve the contracts at runtime.</param>
        public JsonApiMediaTypeFormatter(IContractResolver contractResolver) : this(contractResolver, new DasherizedFieldNamingStrategy(), false) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The resource contract resolver used to resolve the contracts at runtime.</param>
        /// <param name="fieldNamingStratgey">The field naming strategy to use.</param>
        public JsonApiMediaTypeFormatter(IContractResolver contractResolver, IFieldNamingStrategy fieldNamingStratgey) : this(contractResolver, fieldNamingStratgey, false) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The resource contract resolver used to resolve the contracts at runtime.</param>
        /// <param name="fieldNamingStratgey">The field naming strategy to use.</param>
        /// <param name="prettify">A value which indicates whether the output should be prettified.</param>
        JsonApiMediaTypeFormatter(IContractResolver contractResolver, IFieldNamingStrategy fieldNamingStratgey, bool prettify) : base(Name, MediaTypeName, contractResolver, prettify)
        {
            _fieldNamingStratgey = fieldNamingStratgey;
        }

        /// <summary>
        /// Returns a specialized instance of the <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter"/> that can format a response for the given parameters.
        /// </summary>
        /// <param name="type">The type to format.</param>
        /// <param name="request">The request.</param>
        /// <param name="mediaType">The media type.</param>
        /// <returns>Returns <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter"/>.</returns>
        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            var parameters = request.RequestUri.ParseQueryString();

            var prettify = false;
            if (parameters[PrettifyParameterName] != null)
            {
                prettify = new[] { "yes", "1", "true" }.Contains(parameters[PrettifyParameterName], StringComparer.OrdinalIgnoreCase);
            }
            
            var fieldNamingStratgey = _fieldNamingStratgey;
            if (parameters[FieldNamingStrategyParameterName] != null)
            {
                switch (parameters[FieldNamingStrategyParameterName])
                {
                    case "none":
                        fieldNamingStratgey = new DefaultFieldNamingStrategy();
                        break;

                    case "dash":
                        fieldNamingStratgey = new DasherizedFieldNamingStrategy();
                        break;

                    case "snake":
                        fieldNamingStratgey = new SnakeCaseNamingStrategy();
                        break;
                }
            }

            return new JsonApiMediaTypeFormatter(ContractResolver, fieldNamingStratgey, prettify);
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

            var constructor = patch.GetConstructor(new[] { typeof(IContractResolver), typeof(IFieldNamingStrategy), typeof(JsonObject) });
            Debug.Assert(constructor != null);

            return (IPatch)constructor.Invoke(new object[] { ContractResolver, _fieldNamingStratgey, jsonValue });
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

            var serializer = new JsonApiSerializer(ContractResolver, _fieldNamingStratgey);

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
            var serializer = new JsonApiSerializer(ContractResolver, _fieldNamingStratgey);

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