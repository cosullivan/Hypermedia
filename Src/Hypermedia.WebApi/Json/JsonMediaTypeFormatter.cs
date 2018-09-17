using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Hypermedia.Json;
using Hypermedia.Json.Converters;
using Hypermedia.Metadata;
using JsonLite.Ast;

namespace Hypermedia.WebApi.Json
{
    public class JsonMediaTypeFormatter : HypermediaMediaTypeFormatter
    {
        const string Name = "json";
        const string MediaTypeName = "application/json";
        const string FieldNamingStrategyParameterName = "$fieldnamingstrategy";
        const string PrettifyParameterName = "$prettify";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The resource contract resolver used to resolve the contracts at runtime.</param>
        public JsonMediaTypeFormatter(IContractResolver contractResolver) : this(contractResolver, DefaultFieldNamingStrategy.Instance, DefaultJsonOutputFormatter.Instance) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The resource contract resolver used to resolve the contracts at runtime.</param>
        /// <param name="fieldNamingStratgey">The field naming strategy to use.</param>
        public JsonMediaTypeFormatter(IContractResolver contractResolver, IFieldNamingStrategy fieldNamingStratgey) : this(Name, MediaTypeName, contractResolver, fieldNamingStratgey, DefaultJsonOutputFormatter.Instance) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The resource contract resolver used to resolve the contracts at runtime.</param>
        /// <param name="outputFormatter">The output formatter to apply when writing the output.</param>
        public JsonMediaTypeFormatter(
            IContractResolver contractResolver,
            IJsonOutputFormatter outputFormatter) : this(contractResolver, DefaultFieldNamingStrategy.Instance, outputFormatter) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The resource contract resolver used to resolve the contracts at runtime.</param>
        /// <param name="fieldNamingStratgey">The field naming strategy to use.</param>
        /// <param name="outputFormatter">The output formatter to apply when writing the output.</param>
        public JsonMediaTypeFormatter(
            IContractResolver contractResolver, 
            IFieldNamingStrategy fieldNamingStratgey, 
            IJsonOutputFormatter outputFormatter) : this(Name, MediaTypeName, contractResolver, fieldNamingStratgey, outputFormatter) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The friendly name of the format.</param>
        /// <param name="mediaTypeName">The correct media type name for content negotiation.</param>
        /// <param name="contractResolver">The resource contract resolver used to resolve the contracts at runtime.</param>
        /// <param name="fieldNamingStratgey">The field naming strategy to use.</param>
        /// <param name="outputFormatter">The output formatter to apply when writing the output.</param>
        protected JsonMediaTypeFormatter(
            string name, 
            string mediaTypeName, 
            IContractResolver contractResolver, 
            IFieldNamingStrategy fieldNamingStratgey, 
            IJsonOutputFormatter outputFormatter) : base(name, mediaTypeName, contractResolver)
        {
            FieldNamingStrategy = fieldNamingStratgey;
            OutputFormatter = outputFormatter;
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
            return new JsonMediaTypeFormatter(ContractResolver, GetPerRequestFieldNamingStrategy(request), GetPerRequestOutputFormatter(request));
        }

        /// <summary>
        /// Returns a per-request JSON output formatter.
        /// </summary>
        /// <param name="request">The request information that is to be used to determine the output formatter to use.</param>
        /// <returns>The per-request output formatter to use.</returns>
        protected virtual IJsonOutputFormatter GetPerRequestOutputFormatter(HttpRequestMessage request)
        {
            var parameters = request.RequestUri.ParseQueryString();

            if (parameters[PrettifyParameterName] != null)
            {
                var prettify = new[] { "yes", "1", "true" }.Contains(parameters[PrettifyParameterName], StringComparer.OrdinalIgnoreCase);

                if (prettify)
                {
                    return PrettyJsonOutputFormatter.Instance;
                }
            }

            return OutputFormatter;
        }

        /// <summary>
        /// Returns the field naming strategy, taking into account the possibility that it could be overridden.
        /// </summary>
        /// <param name="request">The request information that is to be used to determine the field naming strategy to use.</param>
        /// <returns>The per-request field naming strategy to use.</returns>
        protected virtual IFieldNamingStrategy GetPerRequestFieldNamingStrategy(HttpRequestMessage request)
        {
            var parameters = request.RequestUri.ParseQueryString();

            if (parameters[FieldNamingStrategyParameterName] != null)
            {
                switch (parameters[FieldNamingStrategyParameterName])
                {
                    case "none":
                        return DefaultFieldNamingStrategy.Instance;

                    case "dash":
                        return DasherizedFieldNamingStrategy.Instance;

                    case "snake":
                        return SnakeCaseNamingStrategy.Instance;
                }
            }

            return FieldNamingStrategy;
        }

        /// <summary>
        /// Asynchronously deserializes an object of the specified type.
        /// </summary>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="readStream">The <see cref="T:System.IO.Stream"/> to read.</param>
        /// <param name="content">The <see cref="T:System.Net.Http.HttpContent"/>, if available. It may be null.</param>
        /// <param name="formatterLogger">The <see cref="T:System.Net.Http.Formatting.IFormatterLogger"/> to log events to.</param>
        /// <exception cref="T:System.NotSupportedException">Derived types need to support reading.</exception>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task"/> whose result will be an object of the given type.</returns>
        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var jsonValue = JsonLite.Json.CreateAst(readStream);

            if (jsonValue == null)
            {
                throw new HypermediaWebApiException("Can not create a JSON instance from the stream.");
            }

            return Task.FromResult(ReadFromJsonValue(type, jsonValue));
        }

        /// <summary>
        /// Read an object from the given JSON value.
        /// </summary>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="jsonValue">The JSON value to read.</param>
        /// <returns>The object that was read.</returns>
        protected virtual object ReadFromJsonValue(Type type, JsonValue jsonValue)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IPatch<>))
            {
                return CreatePatch(type, ContractResolver, jsonValue);
            }

            return DeserializeValue(type, jsonValue);
        }

        /// <summary>
        /// Creates an instance of the patch object for the media type.
        /// </summary>
        /// <param name="type">The type of the inner instance that is being patched.</param>
        /// <param name="contractResolver">The contract resolver.</param>
        /// <param name="jsonValue">The JSON value that represents the patch values.</param>
        /// <returns>The instance of the patch.</returns>
        protected virtual IPatch CreatePatch(Type type, IContractResolver contractResolver, JsonValue jsonValue)
        {
            var patch = typeof(JsonPatch<>).MakeGenericType(type.GenericTypeArguments[0]);

            var constructor = patch.GetConstructor(new[] { typeof(IContractResolver), typeof(IFieldNamingStrategy), typeof(JsonObject) });
            Debug.Assert(constructor != null);

            return (IPatch)constructor.Invoke(new object[] { ContractResolver, jsonValue });
        }

        /// <summary>
        /// Deserialize an object.
        /// </summary>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="jsonValue">The JSON value that represents the object to deserialize.</param>
        protected virtual object DeserializeValue(Type type, JsonValue jsonValue)
        {
            var serializer = new JsonSerializer(
                new JsonConverterFactory(
                    JsonConverterFactory.Default,
                    new ContractConverter(ContractResolver, FieldNamingStrategy),
                    new ComplexConverter(FieldNamingStrategy)));

            return serializer.DeserializeValue(type, jsonValue);
        }

        /// <summary>
        /// Asynchronously writes an object of the specified type.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task"/> that will perform the write.</returns>
        /// <param name="type">The type of the object to write.</param>
        /// <param name="value">The object value to write. It may be null.</param>
        /// <param name="writeStream">The <see cref="T:System.IO.Stream"/> to which to write.</param>
        /// <param name="content">The <see cref="T:System.Net.Http.HttpContent"/> if available. It may be null.</param>
        /// <param name="transportContext">The <see cref="T:System.Net.TransportContext"/> if available. It may be null.</param>
        /// <exception cref="T:System.NotSupportedException">Derived types need to support writing.</exception>
        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            using (var writer = new StreamWriter(writeStream, Encoding.ASCII, 1024, leaveOpen: true))
            {
                OutputFormatter.Write(writer, SerializeValue(type, value));
            }

            writeStream.Flush();

            return Task.FromResult(0);
        }

        /// <summary>
        /// Serialize the value into an JSON AST.
        /// </summary>
        /// <param name="type">The type to serialize from.</param>
        /// <param name="value">The value to serialize.</param>
        /// <returns>The JSON object that represents the serialized value.</returns>
        protected virtual JsonValue SerializeValue(Type type, object value)
        {
            var serializer = new JsonSerializer(
                new JsonConverterFactory(
                    JsonConverterFactory.Default,
                    new ContractConverter(ContractResolver, FieldNamingStrategy),
                    new ComplexConverter(FieldNamingStrategy)));

            return serializer.SerializeValue(value);
        }

        /// <summary>
        /// Returns a value indicating whether or not the dictionary has a metadata mapping for the given type.
        /// </summary>
        /// <param name="type">The element type to test for a mapping.</param>
        /// <returns>true if the given type has a mapping, false if not.</returns>
        protected override bool CanReadOrWrite(Type type)
        {
            return true;
        }

        /// <summary>
        /// The field naming strategy to use.
        /// </summary>
        public IFieldNamingStrategy FieldNamingStrategy { get; }

        /// <summary>
        /// The output formatter to use.
        /// </summary>
        public IJsonOutputFormatter OutputFormatter { get; }
    }
}