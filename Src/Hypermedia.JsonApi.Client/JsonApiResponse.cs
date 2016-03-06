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
            var resourceContractResolver = new ContractResolver(RuntimeContract<TEntity>.CreateRuntimeType());

            return Get<TEntity>(resourceContractResolver);
        }

        /// <summary>
        /// Gets a single entity.
        /// </summary>
        /// <typeparam name="TEntity">The element type.</typeparam>
        /// <param name="contractResolver">The contract resolver.</param>
        /// <returns>The list of JSON API entities.</returns>
        public TEntity Get<TEntity>(IContractResolver contractResolver)
        {
            var serializer = new JsonApiSerializer(contractResolver);

            return (TEntity)serializer.DeserializeEntity(_jsonObject);
        }

        /// <summary>
        /// Gets a list of entities.
        /// </summary>
        /// <typeparam name="TEntity">The element type.</typeparam>
        /// <returns>The list of JSON API entities.</returns>
        public IEnumerable<TEntity> GetMany<TEntity>()
        {
            var contractResolver = new ContractResolver(RuntimeContract<TEntity>.CreateRuntimeType());

            return GetMany<TEntity>(contractResolver);
        }

        /// <summary>
        /// Gets a list of entities.
        /// </summary>
        /// <typeparam name="TEntity">The element type.</typeparam>
        /// <param name="contractResolver">The contract resolver.</param>
        /// <returns>The list of JSON API entities.</returns>
        public IEnumerable<TEntity> GetMany<TEntity>(IContractResolver contractResolver)
        {
            var serializer = new JsonApiSerializer(contractResolver);

            return serializer.DeserializeMany(_jsonObject).OfType<TEntity>().ToList();
        }
    }
}
