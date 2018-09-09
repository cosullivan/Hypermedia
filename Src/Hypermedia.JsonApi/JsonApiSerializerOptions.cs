using Hypermedia.Json;
using Hypermedia.Metadata;

namespace Hypermedia.JsonApi
{
    public sealed class JsonApiSerializerOptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contract resolver to use.</param>
        public JsonApiSerializerOptions(IContractResolver contractResolver)
        {
            ContractResolver = contractResolver;
        }

        /// <summary>
        /// Clone the current instance.
        /// </summary>
        /// <returns>The new instance that was cloned from the current instance.</returns>
        public JsonApiSerializerOptions Clone()
        {
            return new JsonApiSerializerOptions(ContractResolver)
            {
                FieldNamingStrategy = FieldNamingStrategy
            };
        }

        /// <summary>
        /// The contract resolver to use.
        /// </summary>
        public IContractResolver ContractResolver { get; }

        /// <summary>
        /// The field naming strategy.
        /// </summary>
        public IFieldNamingStrategy FieldNamingStrategy { get; set; } = DasherizedFieldNamingStrategy.Instance;
    }
}