using Hypermedia.Metadata;

namespace Hypermedia.Json
{
    public class JsonSerializerOptions
    {
        /// <summary>
        /// The contract resolver to use.
        /// </summary>
        public IContractResolver ContractResolver { get; set; }

        /// <summary>
        /// The field naming strategy.
        /// </summary>
        public IFieldNamingStrategy FieldNamingStrategy { get; set; } = DasherizedFieldNamingStrategy.Instance;

        /// <summary>
        /// An optional list of JSON converters.
        /// </summary>
        public IJsonConverter[] JsonConverters { get; set; }
    }
}