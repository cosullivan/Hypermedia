using Hypermedia.Json;
using Hypermedia.Metadata;

namespace Hypermedia.JsonApi
{
    public sealed class JsonApiSerializerOptions
    {
        /// <summary>
        /// The delegate to use when a contract could not be resolved during deserialization.
        /// </summary>
        /// <param name="context">The context for the operation.</param>
        /// <returns>The entity to return as a placeholder.</returns>
        public delegate object MissingContractHandlerDelegate(MissingContractContext context);

        /// <summary>
        /// Clone the current instance.
        /// </summary>
        /// <returns>The new instance that was cloned from the current instance.</returns>
        public JsonApiSerializerOptions Clone()
        {
            return new JsonApiSerializerOptions
            {
                ContractResolver = ContractResolver,
                FieldNamingStrategy = FieldNamingStrategy,
                MissingContractHandler = MissingContractHandler
            };
        }

        /// <summary>
        /// The contract resolver to use.
        /// </summary>
        public IContractResolver ContractResolver { get; set; }

        /// <summary>
        /// The field naming strategy.
        /// </summary>
        public IFieldNamingStrategy FieldNamingStrategy { get; set; } = DasherizedFieldNamingStrategy.Instance;

        /// <summary>
        /// The handler to run when a contract is missing.
        /// </summary>
        public MissingContractHandlerDelegate MissingContractHandler { get; set; } = context => throw new JsonApiException("Could not find a type for '{0}'.", context.Type);

        /// <summary>
        /// An optional list of JSON converters.
        /// </summary>
        public IJsonConverter[] JsonConverters { get; set; }
    }
}