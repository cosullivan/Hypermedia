using System.Collections.Generic;
using System.Linq;
using Hypermedia.Metadata;
using Hypermedia.Metadata.Runtime;
using JsonLite.Ast;

namespace Hypermedia.JsonApi.Client
{
    public sealed class JsonApiResponse
    {
        readonly JsonObject _jsonObject;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonAst">The JSON AST the contains the content to parse.</param>
        public JsonApiResponse(JsonValue jsonAst)
        {
            _jsonObject = (JsonObject)jsonAst;
        }

        /// <summary>
        /// Gets a single entity.
        /// </summary>
        /// <typeparam name="TEntity">The element type.</typeparam>
        /// <returns>The list of JSON API entities.</returns>
        public TEntity Get<TEntity>()
        {
            var resourceContractResolver = new ResourceContractResolver(RuntimeResourceContract<TEntity>.CreateRuntimeType());

            return Get<TEntity>(resourceContractResolver);
        }

        /// <summary>
        /// Gets a single entity.
        /// </summary>
        /// <typeparam name="TEntity">The element type.</typeparam>
        /// <param name="resourceContractResolver">The resource contract resolver.</param>
        /// <returns>The list of JSON API entities.</returns>
        public TEntity Get<TEntity>(IResourceContractResolver resourceContractResolver)
        {
            var serializer = new JsonApiSerializer(resourceContractResolver);

            return (TEntity)serializer.DeserializeEntity(_jsonObject);
        }

        /// <summary>
        /// Gets a list of entities.
        /// </summary>
        /// <typeparam name="TEntity">The element type.</typeparam>
        /// <returns>The list of JSON API entities.</returns>
        public IEnumerable<TEntity> GetMany<TEntity>()
        {
            var resourceContractResolver = new ResourceContractResolver(RuntimeResourceContract<TEntity>.CreateRuntimeType());

            return GetMany<TEntity>(resourceContractResolver);
        }

        /// <summary>
        /// Gets a list of entities.
        /// </summary>
        /// <typeparam name="TEntity">The element type.</typeparam>
        /// <param name="resourceContractResolver">The resource contract resolver.</param>
        /// <returns>The list of JSON API entities.</returns>
        public IEnumerable<TEntity> GetMany<TEntity>(IResourceContractResolver resourceContractResolver)
        {
            var serializer = new JsonApiSerializer(resourceContractResolver);

            return serializer.DeserializeMany(_jsonObject).OfType<TEntity>().ToList();
        }
    }
}
