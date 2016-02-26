using System.Linq;
using Hypermedia.Metadata;
using Hypermedia.WebApi;
using JsonLite.Ast;

namespace Hypermedia.JsonApi.WebApi
{
    public sealed class JsonApiPatch<T> : IPatch<T>
    {
        readonly IResourceContractResolver _resourceContractResolver;
        readonly JsonObject _jsonValue;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="resourceContractResolver">The resource contractor resolver.</param>
        /// <param name="jsonValue">The root document node.</param>
        public JsonApiPatch(IResourceContractResolver resourceContractResolver, JsonObject jsonValue)
        {
            _resourceContractResolver = resourceContractResolver;
            _jsonValue = jsonValue;
        }

        /// <summary>
        /// Attempt to patch the given entity.
        /// </summary>
        /// <param name="entity">The entity to apply the patch to.</param>
        /// <param name="resourceContractResolver">The resource contract resolver to use.</param>
        /// <returns>true if the entity could be patched, false if not.</returns>
        public bool TryPatch(T entity, IResourceContractResolver resourceContractResolver)
        {
            try
            {
                var jsonObject = _jsonValue["data"] as JsonObject;

                if (jsonObject == null)
                {
                    return false;
                }

                var typeAttribute = jsonObject.Members.Single(member => member.Name.Value == "type");

                IResourceContract resourceContract;
                if (_resourceContractResolver.TryResolve(((JsonString)typeAttribute.Value).Value, out resourceContract) == false)
                {
                    return false;
                }

                var serializer = new JsonApiSerializer(ResourceContractResolver);
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
        public IResourceContractResolver ResourceContractResolver
        {
            get { return _resourceContractResolver; }
        }
    }
}
