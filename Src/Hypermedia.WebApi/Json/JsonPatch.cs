using Hypermedia.Json;
using Hypermedia.Metadata;
using JsonLite.Ast;

namespace Hypermedia.WebApi.Json
{
    public sealed class JsonPatch<T> : IPatch<T>
    {
        readonly JsonValue _jsonValue;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contractor resolver.</param>
        /// <param name="jsonValue">The root document node.</param>
        public JsonPatch(IContractResolver contractResolver, JsonValue jsonValue)
        {
            _jsonValue = jsonValue;
            ContractResolver = contractResolver;
        }

        /// <summary>
        /// Attempt to patch the given entity.
        /// </summary>
        /// <param name="entity">The entity to apply the patch to.</param>
        /// <param name="contractResolver">The contract resolver to use.</param>
        /// <returns>true if the entity could be patched, false if not.</returns>
        public bool TryPatch(T entity, IContractResolver contractResolver)
        {
            try
            {
                IContract contract;
                if (contractResolver.TryResolve(typeof(T), out contract) == false)
                {
                    return false;
                }

                var serializer = new JsonSerializer(new JsonConverterFactory(new ContractConverter(ContractResolver)));

                var converter = new ContractConverter(contractResolver);
                converter.DeserializeObject(serializer, (JsonObject)_jsonValue, contract, entity);

                return true;
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { }

            return false;
        }

        /// <summary>
        /// Gets the resource contract resolver that is to be used for the patching.
        /// </summary>
        public IContractResolver ContractResolver { get; }
    }
}
