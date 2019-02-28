using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Hypermedia.AspNetCore.Mvc.Formatters;
using Hypermedia.Json;
using Hypermedia.Json.Converters;
using Hypermedia.Metadata;
using JsonLite.Ast;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Hypermedia.AspNetCore.Json.Formatters
{
    public class JsonInputFormatter : HypermediaInputFormatter
    {
        public const string JsonMediaTypeName = "application/json";

        const string FieldNamingStrategyParameterName = "$fieldnamingstrategy";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contract resolver to use.</param>
        public JsonInputFormatter(IContractResolver contractResolver) : this(JsonMediaTypeName, contractResolver, DefaultFieldNamingStrategy.Instance) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contract resolver to use.</param>
        /// <param name="fieldNamingStrategy">The field naming strategy when serializing and deserializing the JSON.</param>
        public JsonInputFormatter(IContractResolver contractResolver, IFieldNamingStrategy fieldNamingStrategy) : this(JsonMediaTypeName, contractResolver, fieldNamingStrategy) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mediaTypeName">The media type name that the output is handling.</param>
        /// <param name="contractResolver">The contract resolver to use.</param>
        /// <param name="fieldNamingStrategy">The field naming strategy when serializing and deserializing the JSON.</param>
        protected JsonInputFormatter(string mediaTypeName, IContractResolver contractResolver, IFieldNamingStrategy fieldNamingStrategy) : base(contractResolver)
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(mediaTypeName));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
            
            FieldNamingStrategy = fieldNamingStrategy;
        }
        
        /// <summary>
        /// Determines whether this <see cref="T:Microsoft.AspNetCore.Mvc.Formatters.InputFormatter" /> can deserialize an object of the given
        /// <paramref name="type" />.
        /// </summary>
        /// <param name="type">The <see cref="T:System.Type" /> of object that will be read.</param>
        /// <returns><c>true</c> if the <paramref name="type" /> can be read, otherwise <c>false</c>.</returns>
        protected override bool CanReadType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IPatch<>))
            {
                return ContractResolver.CanResolve(type.GetGenericArguments()[0]);
            }

            return base.CanReadType(type);
        }

        /// <summary>Reads an object from the request body.</summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Formatters.InputFormatterContext" />.</param>
        /// <param name="encoding">The <see cref="T:System.Text.Encoding" /> used to read the request body.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that on completion deserializes the request body.</returns>
        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }
            
            var jsonValue = JsonLite.Json.CreateAst(context.HttpContext.Request.Body);

            if (jsonValue == null)
            {
                throw new HypermediaAspNetCoreException("Can not create a JSON instance from the stream.");
            }

            var fieldNamingStrategy = context.HttpContext.Request.GetFieldNamingStrategy(FieldNamingStrategyParameterName) ?? FieldNamingStrategy;

            return Task.FromResult(InputFormatterResult.Success(ReadFromJsonValue(context.ModelType, fieldNamingStrategy, jsonValue)));
        }

        /// <summary>
        /// Read an object from the given JSON value.
        /// </summary>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="fieldNamingStrategy">The field naming strategy that is being used for the scope of the request.</param>
        /// <param name="jsonValue">The JSON value to read.</param>
        /// <returns>The object that was read.</returns>
        protected virtual object ReadFromJsonValue(Type type, IFieldNamingStrategy fieldNamingStrategy, JsonValue jsonValue)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IPatch<>))
            {
                return CreatePatch(type, ContractResolver, fieldNamingStrategy, jsonValue);
            }

            return DeserializeValue(type, fieldNamingStrategy, jsonValue);
        }

        /// <summary>
        /// Creates an instance of the patch object for the media type.
        /// </summary>
        /// <param name="type">The type of the inner instance that is being patched.</param>
        /// <param name="contractResolver">The contract resolver.</param>
        /// <param name="fieldNamingStrategy">The field naming strategy that is being used for the scope of the request.</param>
        /// <param name="jsonValue">The JSON value that represents the patch values.</param>
        /// <returns>The instance of the patch.</returns>
        protected virtual IPatch CreatePatch(Type type, IContractResolver contractResolver, IFieldNamingStrategy fieldNamingStrategy, JsonValue jsonValue)
        {
            var patch = typeof(JsonPatch<>).MakeGenericType(type.GenericTypeArguments[0]);

            var constructor = patch.GetConstructor(new[] { typeof(IContractResolver), typeof(IFieldNamingStrategy), typeof(JsonObject) });
            Debug.Assert(constructor != null);

            return (IPatch)constructor.Invoke(new object[] { ContractResolver, fieldNamingStrategy, jsonValue });
        }

        /// <summary>
        /// Deserialize an object.
        /// </summary>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="fieldNamingStrategy">The field naming strategy that is being used for the scope of the request.</param>
        /// <param name="jsonValue">The JSON value that represents the object to deserialize.</param>
        protected virtual object DeserializeValue(Type type, IFieldNamingStrategy fieldNamingStrategy, JsonValue jsonValue)
        {
            var serializer = new JsonSerializer(
                new JsonConverterFactory(
                    JsonConverterFactory.DefaultConverters.Union(
                        new ContractConverter(ContractResolver, fieldNamingStrategy),
                        new ComplexConverter(fieldNamingStrategy)
                    )));

            return serializer.DeserializeValue(type, jsonValue);
        }

        /// <summary>
        /// The field naming strategy when serializing and deserializing the JSON.
        /// </summary>
        public IFieldNamingStrategy FieldNamingStrategy { get; }
    }
}