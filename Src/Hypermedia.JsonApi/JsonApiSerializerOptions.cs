using Hypermedia.Json;
using Hypermedia.Metadata;

namespace Hypermedia.JsonApi
{
    public sealed class JsonApiSerializerOptions
    {
        public delegate void MissingContractHandlerDelegate(MissingContractContext context);

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
                FieldNamingStrategy = FieldNamingStrategy,
                MissingContractHandler = MissingContractHandler
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

        /// <summary>
        /// The handler to run when a contract is missing.
        /// </summary>
        public MissingContractHandlerDelegate MissingContractHandler { get; set; } = context => throw new JsonApiException("Could not find a type for '{0}'.", context.Type);
    }
}