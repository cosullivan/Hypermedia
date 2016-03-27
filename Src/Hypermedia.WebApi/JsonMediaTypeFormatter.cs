using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hypermedia.Json;
using Hypermedia.Metadata;
using JsonLite.Ast;

namespace Hypermedia.WebApi
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
        public JsonMediaTypeFormatter(IContractResolver contractResolver) : this(contractResolver, false) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The resource contract resolver used to resolve the contracts at runtime.</param>
        /// <param name="prettify">Indicates whether the output should formatted in a readable way.</param>
        public JsonMediaTypeFormatter(IContractResolver contractResolver, bool prettify) : base(Name, MediaTypeName, contractResolver)
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

                return new JsonMediaTypeFormatter(ContractResolver, prettify);
            }

            return base.GetPerRequestFormatterInstance(type, request, mediaType);
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
            HERE: implement this one

            //var jsonAst = JsonLite.Json.CreateAst(readStream) as JsonObject;

            //if (jsonAst == null)
            //{
            //    throw new JsonApiException("Can to serialize the JSON into a JSON object required for the JSONAPI specification.");
            //}

            //if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IPatch<>))
            //{
            //    var patch = typeof(JsonApiPatch<>).MakeGenericType(type.GenericTypeArguments[0]);

            //    var constructor = patch.GetConstructor(new[] { typeof(IContractResolver), typeof(JsonObject) });
            //    Debug.Assert(constructor != null);

            //    return Task.FromResult(constructor.Invoke(new object[] { ContractResolver, jsonAst }));
            //}

            //var serializer = new JsonApiSerializer(ContractResolver);

            //if (TypeHelper.IsEnumerable(type))
            //{
            //    return Task.FromResult((object)serializer.DeserializeMany(jsonAst));
            //}

            //return Task.FromResult(serializer.DeserializeEntity(jsonAst));

            return null;
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
        JsonValue SerializeValue(Type type, object value)
        {
            var serializer = new JsonSerializer(new JsonConverterFactory(new ContractConverter(ContractResolver)));

            return serializer.SerializeValue(value);
        }

        #region ContractConverter

        class ContractConverter : IJsonConverter
        {
            readonly IContractResolver _contractResolver;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="contractResolver">The contract resolver.</param>
            public ContractConverter(IContractResolver contractResolver)
            {
                _contractResolver = contractResolver;
            }

            /// <summary>
            /// Serialize the value.
            /// </summary>
            /// <param name="serializer">The serializer to utilize when serializing nested objects.</param>
            /// <param name="type">The CLR type of the value to serialize.</param>
            /// <param name="value">The value to serialize.</param>
            /// <returns>The JSON value that represents the given CLR value.</returns>
            public JsonValue SerializeValue(IJsonSerializer serializer, Type type, object value)
            {
                IContract contract;
                if (_contractResolver.TryResolve(type, out contract) == false)
                {
                    throw new HypermediaWebApiException($"Could not resolve a contract for {type}.");
                }

                return new JsonObject(SerializeMembers(serializer, contract, value).ToList());
            }

            /// <summary>
            /// Serialize a list of members from the object.
            /// </summary>
            /// <param name="serializer">The serializer to utilize when serializing the values.</param>
            /// <param name="contract">The contract of the object to serialize the members from.</param>
            /// <param name="value">The value to serialize the members from.</param>
            /// <returns>The list of members that make up the object.</returns>
            static IEnumerable<JsonMember> SerializeMembers(IJsonSerializer serializer, IContract contract, object value)
            {
                foreach (var field in contract.Fields.Where(ShouldSerializeField))
                {
                    yield return new JsonMember(field.Name, serializer.SerializeValue(field.GetValue(value)));
                }

                yield break;
            }

            /// <summary>
            /// Deserialize a JSON value to a defined CLR type.
            /// </summary>
            /// <param name="serializer">The serializer to utilize when deserializing nested objects.</param>
            /// <param name="type">The CLR type to deserialize the JSON value to.</param>
            /// <param name="jsonValue">The JSON value to deserialize.</param>
            /// <returns>The object that represents the CLR version of the given JSON value.</returns>
            public object DeserializeValue(IJsonSerializer serializer, Type type, JsonValue jsonValue)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Returns a value indicating whether or not the converter can convert the given type.
            /// </summary>
            /// <param name="type">The type to convert.</param>
            /// <returns>true if the type can be converted by this converter, false if not.</returns>
            public bool CanConvert(Type type)
            {
                return _contractResolver.CanResolve(type);
            }

            /// <summary>
            /// Returns a value indicating whether or not the given field should be included when serializing.
            /// </summary>
            /// <param name="field">The field to determine whether or not it should be included.</param>
            /// <returns>true if the field should be included, false if not.</returns>
            static bool ShouldSerializeField(IField field)
            {
                return field.Is(FieldOptions.CanSerialize);
            }
        }

        #endregion
    }
}
