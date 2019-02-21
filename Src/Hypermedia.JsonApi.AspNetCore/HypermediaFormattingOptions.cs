using Hypermedia.Json;
using Hypermedia.Metadata;

namespace Hypermedia.JsonApi.AspNetCore
{
    public sealed class HypermediaFormattingOptions
    {
        /// <summary>
        /// Create a default set of options.
        /// </summary>
        /// <returns>The default set of JSON API serializer options.</returns>
        internal JsonApiSerializerOptions CoalesceJsonApiSerializeOptions()
        {
            return new JsonApiSerializerOptions
            {
                ContractResolver = JsonApiSerializerOptions?.ContractResolver ?? ContractResolver,
                FieldNamingStrategy = JsonApiSerializerOptions?.FieldNamingStrategy ?? FieldNamingStrategy
            };
        }

        /// <summary>
        /// The contract resolver to use.
        /// </summary>
        public IContractResolver ContractResolver { get; set; }

        /// <summary>
        /// The default field naming strategy.
        /// </summary>
        public IFieldNamingStrategy FieldNamingStrategy { get; set; }

        /// <summary>
        /// The JSON API serializer options.
        /// </summary>
        public JsonApiSerializerOptions JsonApiSerializerOptions { get; set; }
    }
}