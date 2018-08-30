using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Hypermedia.AspNetCore.Mvc.Formatters;
using Hypermedia.Json;
using Hypermedia.Json.Converters;
using Hypermedia.Metadata;
using JsonLite.Ast;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Hypermedia.AspNetCore.Json.Formatters
{
    public class JsonOutputFormatter : HypermediaOutputFormatter
    {
        public const string JsonMediaTypeName = "application/json";

        const string FieldNamingStrategyParameterName = "$fieldnamingstrategy";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contract resolver to use.</param>
        public JsonOutputFormatter(IContractResolver contractResolver) : this(JsonMediaTypeName, contractResolver, DefaultFieldNamingStrategy.Instance) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mediaTypeName">The media type name that the output is handling.</param>
        /// <param name="contractResolver">The contract resolver to use.</param>
        /// <param name="fieldNamingStrategy">The field naming strategy when serializing and deserializing the JSON.</param>
        protected JsonOutputFormatter(string mediaTypeName, IContractResolver contractResolver, IFieldNamingStrategy fieldNamingStrategy) : base(contractResolver)
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(mediaTypeName));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
            
            FieldNamingStrategy = fieldNamingStrategy;
        }

        /// <inheritdoc />
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var jsonValue = SerializeValue(
                context.ObjectType, 
                context.Object,
                GetPerRequestFieldNamingStrategy(context.HttpContext.Request));

            using (var writer = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
            {
                await WriteResponseBodyAsync(writer, jsonValue);
                await writer.FlushAsync();
            }
        }

        /// <summary>
        /// Write the response body.
        /// </summary>
        /// <param name="writer">The text writer to write the contents of the response to.</param>
        /// <param name="jsonValue">The JSON value to write to the output.</param>
        /// <returns>A task which asychronously performs the operation.</returns>
        protected virtual async Task WriteResponseBodyAsync(TextWriter writer, JsonValue jsonValue)
        {
            await writer.WriteAsync(jsonValue.Stringify());
        }

        /// <summary>
        /// Returns the field naming strategy, taking into account the possibility that it could be overridden.
        /// </summary>
        /// <param name="request">The request information that is to be used to determine the field naming strategy to use.</param>
        /// <returns>The per-request field naming strategy to use.</returns>
        protected IFieldNamingStrategy GetPerRequestFieldNamingStrategy(HttpRequest request)
        {
            return request.GetFieldNamingStrategy(FieldNamingStrategyParameterName) ?? FieldNamingStrategy;
        }

        /// <summary>
        /// Serialize the value into an JSON AST.
        /// </summary>
        /// <param name="type">The type to serialize from.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="fieldNamingStrategy">The field naming strategy when serializing and deserializing the JSON.</param>
        /// <returns>The JSON object that represents the serialized value.</returns>
        protected virtual JsonValue SerializeValue(Type type, object value, IFieldNamingStrategy fieldNamingStrategy)
        {
            var serializer = new JsonSerializer(new JsonConverterFactory(new ContractConverter(ContractResolver)), fieldNamingStrategy);

            return serializer.SerializeValue(value);
        }

        /// <summary>
        /// The field naming strategy when serializing and deserializing the JSON.
        /// </summary>
        public IFieldNamingStrategy FieldNamingStrategy { get; }
    }
}