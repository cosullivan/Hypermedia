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
            var contractResolver = new ContractResolver(RuntimeContract<TEntity>.CreateRuntimeType());

            return Get<TEntity>(contractResolver);
        }

        /// <summary>
        /// Gets a single entity.
        /// </summary>
        /// <typeparam name="TEntity">The element type.</typeparam>
        /// <param name="contractResolver">The contract resolver.</param>
        /// <returns>The list of JSON API entities.</returns>
        public TEntity Get<TEntity>(IContractResolver contractResolver)
        {
            return Get<TEntity>(contractResolver, new JsonApiEntityCache());
        }

        /// <summary>
        /// Gets a single entity.
        /// </summary>
        /// <typeparam name="TEntity">The element type.</typeparam>
        /// <param name="contractResolver">The contract resolver.</param>
        /// <param name="cache">The entity cache to use for resolving existing instances in the object graph.</param>
        /// <returns>The list of JSON API entities.</returns>
        public TEntity Get<TEntity>(IContractResolver contractResolver, IJsonApiEntityCache cache)
        {
            return Get<TEntity>(new JsonApiSerializerOptions { ContractResolver = contractResolver }, cache);
        }

        /// <summary>
        /// Gets a single entity.
        /// </summary>
        /// <typeparam name="TEntity">The element type.</typeparam>
        /// <param name="serializerOptions">The serializer options.</param>
        /// <param name="cache">The entity cache to use for resolving existing instances in the object graph.</param>
        /// <returns>The list of JSON API entities.</returns>
        public TEntity Get<TEntity>(JsonApiSerializerOptions serializerOptions, IJsonApiEntityCache cache)
        {
            return Get<TEntity>(new JsonApiSerializer(serializerOptions), cache);
        }

        /// <summary>
        /// Gets a single entity.
        /// </summary>
        /// <typeparam name="TEntity">The element type.</typeparam>
        /// <param name="serializer">The JSON API serializer.</param>
        /// <param name="cache">The entity cache to use for resolving existing instances in the object graph.</param>
        /// <returns>The list of JSON API entities.</returns>
        public TEntity Get<TEntity>(JsonApiSerializer serializer, IJsonApiEntityCache cache)
        {
            return (TEntity)serializer.Deserialize(_jsonObject, cache);
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
            return GetMany<TEntity>(contractResolver, new JsonApiEntityCache());
        }

        /// <summary>
        /// Gets a list of entities.
        /// </summary>
        /// <typeparam name="TEntity">The element type.</typeparam>
        /// <param name="contractResolver">The contract resolver.</param>
        /// <param name="cache">The entity cache to use for resolving existing instances in the object graph.</param>
        /// <returns>The list of JSON API entities.</returns>
        public IEnumerable<TEntity> GetMany<TEntity>(IContractResolver contractResolver, IJsonApiEntityCache cache)
        {
            return GetMany<TEntity>(new JsonApiSerializer(new JsonApiSerializerOptions { ContractResolver = contractResolver }), cache);
        }

        /// <summary>
        /// Gets a list of entities.
        /// </summary>
        /// <typeparam name="TEntity">The element type.</typeparam>
        /// <param name="serializerOptions">The serializer options.</param>
        /// <param name="cache">The entity cache to use for resolving existing instances in the object graph.</param>
        /// <returns>The list of JSON API entities.</returns>
        public IEnumerable<TEntity> GetMany<TEntity>(JsonApiSerializerOptions serializerOptions, IJsonApiEntityCache cache)
        {
            return GetMany<TEntity>(new JsonApiSerializer(serializerOptions), cache);
        }

        /// <summary>
        /// Gets a list of entities.
        /// </summary>
        /// <typeparam name="TEntity">The element type.</typeparam>
        /// <param name="serializer">The JSON API serializer.</param>
        /// <param name="cache">The entity cache to use for resolving existing instances in the object graph.</param>
        /// <returns>The list of JSON API entities.</returns>
        public IEnumerable<TEntity> GetMany<TEntity>(JsonApiSerializer serializer, IJsonApiEntityCache cache)
        {
            return serializer.DeserializeMany(_jsonObject, cache).OfType<TEntity>().ToList();
        }
    }
}