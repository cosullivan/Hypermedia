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
        /// <param name="resourceContractResolver">The resource contract resolver.</param>
        public JsonApiContent(TEntity entity, IResourceContractResolver resourceContractResolver) : base(SerializeEntity(entity, resourceContractResolver), Encoding.UTF8, MediaTypeName) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entity">The entity to serialize.</param>
        /// <param name="resourceContractResolver">The resource contract resolver.</param>
        public JsonApiContent(IEnumerable<TEntity> entity, IResourceContractResolver resourceContractResolver) : base(SerializeMany(entity, resourceContractResolver), Encoding.UTF8, MediaTypeName) { }

        /// <summary>
        /// Serialize the entity to a content string.
        /// </summary>
        /// <param name="entity">The entity to serialize.</param>
        /// <param name="resourceContractResolver">The resource contract resolver.</param>
        /// <returns>The string that represents the serialized version of the entity.</returns>
        static string SerializeEntity(TEntity entity, IResourceContractResolver resourceContractResolver)
        {
            var serializer = new JsonApiSerializer(resourceContractResolver);

            return serializer.SerializeEntity(entity).Stringify();
        }

        /// <summary>
        /// Serialize the entities to a content string.
        /// </summary>
        /// <param name="entities">The entities to serialize.</param>
        /// <param name="resourceContractResolver">The resource contract resolver.</param>
        /// <returns>The string that represents the serialized version of the entity.</returns>
        static string SerializeMany(IEnumerable<TEntity> entities, IResourceContractResolver resourceContractResolver)
        {
            var serializer = new JsonApiSerializer(resourceContractResolver);

            return serializer.SerializeMany(entities).Stringify();
        }
    }
}
