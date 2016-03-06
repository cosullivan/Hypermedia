using Hypermedia.Metadata;
using Hypermedia.WebApi;
using JsonLite.Ast;

namespace Hypermedia.JsonApi.WebApi
{
    public sealed class JsonApiPatch<T> : IPatch<T>
    {
        readonly IContractResolver _contractResolver;
        readonly JsonObject _jsonValue;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The resource contractor resolver.</param>
        /// <param name="jsonValue">The root document node.</param>
        public JsonApiPatch(IContractResolver contractResolver, JsonObject jsonValue)
        {
            _contractResolver = contractResolver;
            _jsonValue = jsonValue;
        }

        /// <summary>
        /// Attempt to patch the given entity.
        /// </summary>
        /// <param name="entity">The entity to apply the patch to.</param>
        /// <param name="resourceContractResolver">The resource contract resolver to use.</param>
        /// <returns>true if the entity could be patched, false if not.</returns>
        public bool TryPatch(T entity, IContractResolver resourceContractResolver)
        {
            try
            {
                var jsonObject = _jsonValue["data"] as JsonObject;

                var typeAttribute = jsonObject?["type"];
                if (typeAttribute == null)
                {
                    return false;
                }

                IResourceContract resourceContract;
                if (_contractResolver.TryResolve(((JsonString)typeAttribute).Value, out resourceContract) == false)
                {
                    return false;
                }

                var serializer = new JsonApiSerializer(ContractResolver);
                serializer.DeserializeEntity(resourceContract, jsonObject, entity);

                return true;
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { }

            return false;
        }

        /// <summary>
        /// Gets the resource contract resolver that is to be used for the patching.
        /// </summary>
        public IContractResolver ContractResolver
        {
            get { return _contractResolver; }
        }
    }
}
