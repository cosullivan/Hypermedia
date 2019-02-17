using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Hypermedia.Metadata;

namespace Hypermedia.JsonApi.Client
{
    public sealed class JsonApiContent<TEntity> : StringContent
    {
        const string MediaTypeName = "application/vnd.api+json";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entity">The entity to serialize.</param>
        /// <param name="contractResolver">The contract resolver.</param>
        public JsonApiContent(TEntity entity, IContractResolver contractResolver) : this(entity, new JsonApiSerializerOptions { ContractResolver = contractResolver }) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entity">The entity to serialize.</param>
        /// <param name="serializerOptions">The serializer options.</param>
        public JsonApiContent(TEntity entity, JsonApiSerializerOptions serializerOptions) : base(SerializeEntity(entity, serializerOptions), Encoding.UTF8, MediaTypeName) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entities">The entities to serialize.</param>
        /// <param name="contractResolver">The contract resolver.</param>
        public JsonApiContent(IEnumerable<TEntity> entities, IContractResolver contractResolver) : this(entities, new JsonApiSerializerOptions { ContractResolver = contractResolver }) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entities">The entities to serialize.</param>
        /// <param name="serializerOptions">The serializer options.</param>
        public JsonApiContent(IEnumerable<TEntity> entities, JsonApiSerializerOptions serializerOptions) : base(SerializeMany(entities, serializerOptions), Encoding.UTF8, MediaTypeName) { }

        /// <summary>
        /// Serialize the entity to a content string.
        /// </summary>
        /// <param name="entity">The entity to serialize.</param>
        /// <param name="serializerOptions">The serializer options.</param>
        /// <returns>The string that represents the serialized version of the entity.</returns>
        static string SerializeEntity(TEntity entity, JsonApiSerializerOptions serializerOptions)
        {
            var serializer = new JsonApiSerializer(serializerOptions);

            return serializer.SerializeEntity(entity).Stringify();
        }

        /// <summary>
        /// Serialize the entities to a content string.
        /// </summary>
        /// <param name="entities">The entities to serialize.</param>
        /// <param name="serializerOptions">The serializer options.</param>
        /// <returns>The string that represents the serialized version of the entity.</returns>
        static string SerializeMany(IEnumerable<TEntity> entities, JsonApiSerializerOptions serializerOptions)
        {
            var serializer = new JsonApiSerializer(serializerOptions);

            return serializer.SerializeMany(entities).Stringify();
        }
    }
}