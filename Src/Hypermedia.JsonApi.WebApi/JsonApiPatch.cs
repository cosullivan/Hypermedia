using Hypermedia.Json;
using Hypermedia.Metadata;
using Hypermedia.WebApi;
using JsonLite.Ast;

namespace Hypermedia.JsonApi.WebApi
{
    public sealed class JsonApiPatch<T> : IPatch<T>
    {
        readonly IFieldNamingStrategy _fieldNamingStratgey;
        readonly JsonObject _jsonValue;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The resource contractor resolver.</param>
        /// <param name="fieldNamingStratgey">The field naming strategy to use.</param>
        /// <param name="jsonValue">The root document node.</param>
        public JsonApiPatch(IContractResolver contractResolver, IFieldNamingStrategy fieldNamingStratgey, JsonObject jsonValue)
        {
            _fieldNamingStratgey = fieldNamingStratgey;
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
                var jsonObject = _jsonValue["data"] as JsonObject;

                var typeAttribute = jsonObject?["type"];
                if (typeAttribute == null)
                {
                    return false;
                }

                IContract contract;
                if (contractResolver.TryResolve(((JsonString)typeAttribute).Value, out contract) == false)
                {
                    return false;
                }

                var serializer = new JsonApiSerializer(contractResolver, _fieldNamingStratgey);
                serializer.DeserializeEntity(contract, jsonObject, entity);

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