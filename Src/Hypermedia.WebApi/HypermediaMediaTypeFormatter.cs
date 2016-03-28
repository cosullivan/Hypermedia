using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Hypermedia.Metadata;

namespace Hypermedia.WebApi
{
    public abstract class HypermediaMediaTypeFormatter : MediaTypeFormatter
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The friendly name of the format.</param>
        /// <param name="mediaTypeName">The correct media type name for content negotiation.</param>
        /// <param name="contractResolver">The resource contract resolver used to resolve the contracts at runtime.</param>
        protected HypermediaMediaTypeFormatter(string name, string mediaTypeName, IContractResolver contractResolver)
        {
            ContractResolver = contractResolver;
            
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Insert(0, new MediaTypeHeaderValue(mediaTypeName));

            MediaTypeMappings.Clear();
            MediaTypeMappings.Add(new QueryStringMapping("$format", name, mediaTypeName));
        }

        /// <summary>
        /// Queries whether this <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter"/> can deserializean object of the specified type.
        /// </summary>
        /// <param name="type">The type to deserialize.</param>
        /// <returns>true if the <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter"/> can deserialize the type; otherwise, false.</returns>
        public override bool CanReadType(Type type)
        {
            if (type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IPatch<>))
            {
                return true;
            }

            return CanReadOrWrite(type);
        }

        /// <summary>
        /// Queries whether this <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter"/> can serializean object of the specified type.
        /// </summary>
        /// <param name="type">The type to serialize.</param>
        /// <returns>true if the <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter"/> can serialize the type; otherwise, false.</returns>
        public override bool CanWriteType(Type type)
        {
            return CanReadOrWrite(type);
        }

        /// <summary>
        /// Returns a value indicating whether or not the dictionary has a metadata mapping for the given type.
        /// </summary>
        /// <param name="type">The element type to test for a mapping.</param>
        /// <returns>true if the given type has a mapping, false if not.</returns>
        protected virtual bool CanReadOrWrite(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return ContractResolver.CanResolve(TypeHelper.GetUnderlyingType(type));
        }

        /// <summary>
        /// Gets an instance of the resource contract resolver used to resolve the types at runtime.
        /// </summary>
        public IContractResolver ContractResolver { get; }
    }
}
