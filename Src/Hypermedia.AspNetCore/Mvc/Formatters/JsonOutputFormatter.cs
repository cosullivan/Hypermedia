using System;
using System.Text;
using System.Threading.Tasks;
using Hypermedia.Json;
using Hypermedia.Json.Converters;
using Hypermedia.Metadata;
using JsonLite.Ast;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Hypermedia.AspNetCore.Mvc.Formatters
{
    public class JsonOutputFormatter : HypermediaOutputFormatter
    {
        public const string JsonMediaTypeName = "application/json";

        const string FieldNamingStrategyParameterName = "$fieldnamingstrategy";
        const string PrettifyParameterName = "$prettify";

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
            using (var writer = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
            {
                var jsonValue = SerializeValue(
                    context.ObjectType, 
                    context.Object,
                    GetPerRequestFieldNamingStrategy(context.HttpContext.Request));

                await writer.WriteAsync(jsonValue.Stringify());
                await writer.FlushAsync();
            }
        }

        /// <summary>
        /// Returns the field naming strategy, taking into account the possibility that it could be overridden.
        /// </summary>
        /// <param name="request">The request information that is to be used to determine the field naming strategy to use.</param>
        /// <returns>The per-request field naming strategy to use.</returns>
        protected IFieldNamingStrategy GetPerRequestFieldNamingStrategy(HttpRequest request)
        {
            if (request.Query.TryGetValue(FieldNamingStrategyParameterName, out var value))
            {
                return FieldNamingStrategy;
            }

            switch (value)
            {
                case "none":
                    return DefaultFieldNamingStrategy.Instance;

                case "dash":
                    return DasherizedFieldNamingStrategy.Instance;

                case "snake":
                    return SnakeCaseNamingStrategy.Instance;
            }

            return FieldNamingStrategy;
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