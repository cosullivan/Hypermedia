using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Hypermedia.Json;
using Hypermedia.JsonApi.WebApi;
using Hypermedia.Metadata;
using Hypermedia.WebApi.Json;
using JsonLite.Ast;

namespace Hypermedia.Sample.WebApi.Services
{
    public sealed class JsonApiMetadataMediaTypeFormatter : JsonApiMediaTypeFormatter
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The resource contract resolver used to resolve the contracts at runtime.</param>
        public JsonApiMetadataMediaTypeFormatter(IContractResolver contractResolver) : base(contractResolver) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The resource contract resolver used to resolve the contracts at runtime.</param>
        /// <param name="fieldNamingStratgey">The field naming strategy to use.</param>
        /// <param name="outputFormatter">The output formatter to apply when writing the output.</param>
        public JsonApiMetadataMediaTypeFormatter(
            IContractResolver contractResolver,
            IFieldNamingStrategy fieldNamingStratgey,
            IJsonOutputFormatter outputFormatter) : base(contractResolver, fieldNamingStratgey, outputFormatter) { }

        /// <summary>
        /// Returns a specialized instance of the <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter"/> that can format a response for the given parameters.
        /// </summary>
        /// <param name="type">The type to format.</param>
        /// <param name="request">The request.</param>
        /// <param name="mediaType">The media type.</param>
        /// <returns>Returns <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter"/>.</returns>
        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            return new JsonApiMetadataMediaTypeFormatter(ContractResolver, GetPerRequestFieldNamingStrategy(request), GetPerRequestOutputFormatter(request));
        }

        /// <summary>
        /// Serialize the value into an JSON AST.
        /// </summary>
        /// <param name="type">The type to serialize from.</param>
        /// <param name="value">The value to serialize.</param>
        /// <returns>The JSON object that represents the serialized value.</returns>
        protected override JsonValue SerializeValue(Type type, object value)
        {
            var jsonValue = base.SerializeValue(type, value);

            return new JsonApiMetadataVisitor().WriteMetadata(jsonValue);
        }

        #region JsonApiMetadataVisitor

        class JsonApiMetadataVisitor : JsonAstVisitor
        {
            /// <summary>
            /// Write the metadata for the JSON API outout.
            /// </summary>
            /// <param name="jsonValue">The JSON value to write the metadata to.</param>
            /// <returns>The JSON value that had the metadata written to it.</returns>
            public JsonValue WriteMetadata(JsonValue jsonValue)
            {
                return base.Visit(jsonValue);
            }

            /// <summary>
            /// Visit a JSON object.
            /// </summary>
            /// <param name="jsonObject">The JSON object to visit.</param>
            /// <returns>The JSON value that was modified from the visitor.</returns>
            protected override JsonValue Visit(JsonObject jsonObject)
            {
                var metadata = new JsonMember("meta", 
                    new JsonObject(
                        new JsonMember("version", new JsonString("1.0.0.0")),
                        new JsonMember("source", new JsonString("https://mythology.stackexchange.com")),
                        new JsonMember("attribution", new JsonString("The content published here comes from the Mythology site on the StackExchange network.")),
                        new JsonMember("time", new JsonString(DateTime.Now.ToString("D")))));

                var members = jsonObject.Members.Union(new[] { metadata });

                return new JsonObject(members.ToList());
            }
        }

        #endregion
    }
}