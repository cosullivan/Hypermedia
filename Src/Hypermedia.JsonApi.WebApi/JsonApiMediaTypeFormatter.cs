using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Hypermedia.Metadata;
using Hypermedia.WebApi;
using JsonLite;
using JsonLite.Ast;

namespace Hypermedia.JsonApi.WebApi
{
    public class JsonApiMediaTypeFormatter : HypermediaMediaTypeFormatter
    {
        const string Name = "jsonapi";
        const string MediaTypeName = "application/vnd.api+json";

        //readonly HttpRequestMessage _request;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="resourceContractResolver">The resource contract resolver used to resolve the contracts at runtime.</param>
        public JsonApiMediaTypeFormatter(IResourceContractResolver resourceContractResolver) : base(Name, MediaTypeName, resourceContractResolver) { }

        ///// <summary>
        ///// Constructor.
        ///// </summary>
        ///// <param name="resourceContractResolver">The resource contract resolver used to resolve the contracts at runtime.</param>
        ///// <param name="request">The HTTP request message that the formatter is working with.</param>
        //internal JsonApiMediaTypeFormatter(IResourceContractResolver resourceContractResolver, HttpRequestMessage request) : this(resourceContractResolver)
        //{
        //    _request = request;
        //}

        ///// <summary>
        ///// Returns a specialized instance of the <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter"/> that can format a response for the given parameters.
        ///// </summary>
        ///// <param name="type">The type to format.</param>
        ///// <param name="request">The request.</param>
        ///// <param name="mediaType">The media type.</param>
        ///// <returns>Returns <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter"/>.</returns>
        //public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        //{
        //    return new JsonApiMediaTypeFormatter(ResourceContractResolver, request);
        //}

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
            var jsonAst = Json.CreateAst(readStream) as JsonObject;

            if (jsonAst == null)
            {
                throw new JsonApiException("Can to serialize the JSON into a JSON object required for the JSONAPI specification.");   
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IPatch<>))
            {
                var patch = typeof(JsonApiPatch<>).MakeGenericType(type.GenericTypeArguments[0]);

                var constructor = patch.GetConstructor(new[] {typeof(IResourceContractResolver), typeof (JsonObject)});
                Debug.Assert(constructor != null);

                return Task.FromResult(constructor.Invoke(new object[] { ResourceContractResolver, jsonAst }));
            }

            var serializer = new JsonApiSerializer(ResourceContractResolver);

            if (TypeHelper.IsEnumerable(type))
            {
                return Task.FromResult((object)serializer.DeserializeMany(jsonAst));
            }

            return Task.FromResult(serializer.DeserializeEntity(jsonAst));
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
                writer.WriteLine(SerializeValue(type, value).Stringify());
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
        JsonObject SerializeValue(Type type, object value)
        {
            var serializer = new JsonApiSerializer(ResourceContractResolver);

            if (TypeHelper.IsEnumerable(type))
            {
                return serializer.SerializeMany((IEnumerable)value);
            }

            return serializer.SerializeEntity(value);
        }
    }
}
