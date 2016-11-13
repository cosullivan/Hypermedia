using System;
using System.Collections.Generic;
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
using Hypermedia.Metadata;
using JsonLite.Ast;

namespace Hypermedia.WebApi.Json
{
    public class JsonMediaTypeFormatter : HypermediaMediaTypeFormatter
    {
        const string Name = "json";
        const string MediaTypeName = "application/json";
        const string PrettifyParameterName = "$prettify";

        readonly bool _prettify;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The resource contract resolver used to resolve the contracts at runtime.</param>
        public JsonMediaTypeFormatter(IContractResolver contractResolver) : this(Name, MediaTypeName, contractResolver, false) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The resource contract resolver used to resolve the contracts at runtime.</param>
        /// <param name="prettify">Indicates whether the output should formatted in a readable way.</param>
        public JsonMediaTypeFormatter(IContractResolver contractResolver, bool prettify) : this(Name, MediaTypeName, contractResolver, prettify) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The friendly name of the format.</param>
        /// <param name="mediaTypeName">The correct media type name for content negotiation.</param>
        /// <param name="contractResolver">The resource contract resolver used to resolve the contracts at runtime.</param>
        /// <param name="prettify">Indicates whether the output should formatted in a readable way.</param>
        protected JsonMediaTypeFormatter(string name, string mediaTypeName, IContractResolver contractResolver, bool prettify) : base(name, mediaTypeName, contractResolver)
        {
            _prettify = prettify;
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

            if (parameters[PrettifyParameterName] != null)
            {
                var prettify = new[] { "yes", "1", "true" }.Contains(parameters[PrettifyParameterName], StringComparer.OrdinalIgnoreCase);

                return CreatePerRequestInstance(ContractResolver, prettify);
            }

            return base.GetPerRequestFormatterInstance(type, request, mediaType);
        }

        /// <summary>
        /// Creates a per request formatter instance.
        /// </summary>
        /// <param name="contractResolver">The contract resolver to create the request with.</param>
        /// <param name="prettify">A value which indicates whether the output should be prettified.</param>
        /// <returns>The formatter instance to use specifically for the scope of a request.</returns>
        protected virtual MediaTypeFormatter CreatePerRequestInstance(IContractResolver contractResolver, bool prettify)
        {
            return new JsonMediaTypeFormatter(ContractResolver, prettify);
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

            var constructor = patch.GetConstructor(new[] { typeof(IContractResolver), typeof(JsonObject) });
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
            var serializer = new JsonSerializer(new JsonConverterFactory(new ContractConverter(ContractResolver)), new DefaultFieldNamingStrategy());

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
                writer.WriteLine(SerializeValue(type, value).Stringify(_prettify));
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
            var serializer = new JsonSerializer(new JsonConverterFactory(new ContractConverter(ContractResolver)), new DefaultFieldNamingStrategy());

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
    }
}