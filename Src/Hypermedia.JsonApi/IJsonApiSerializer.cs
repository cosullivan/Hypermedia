using System.Collections;
using JsonLite.Ast;

namespace Hypermedia.JsonApi
{
    public interface IJsonApiSerializer
    {
        /// <summary>
        /// Serialize a list of entities.
        /// </summary>
        /// <param name="entities">The list of entities to serialize.</param>
        /// <returns>The JSON object that represents the serialized entity.</returns>
        JsonObject SerializeMany(IEnumerable entities);

        /// <summary>
        /// Serialize the an entity.
        /// </summary>
        /// <param name="entity">The entity to serialize.</param>
        /// <returns>The JSON object that represents the serialized entity.</returns>
        JsonObject Serialize(object entity);
    }
}