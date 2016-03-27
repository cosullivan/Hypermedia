//using System.Collections;
//using System.Collections.Generic;

//namespace Hypermedia.WebApi
//{
//    public interface IContentSerializer<TContent>
//    {
//        /// <summary>
//        /// Serialize a list of entities.
//        /// </summary>
//        /// <param name="entities">The list of entities to serialize.</param>
//        /// <returns>The content that represents the serialized entity.</returns>
//        string SerializeMany(IEnumerable entities);

//        /// <summary>
//        /// Serialize the an entity.
//        /// </summary>
//        /// <param name="entity">The entity to serialize.</param>
//        /// <returns>The content that represents the serialized entity.</returns>
//        string SerializeEntity(object entity);

//        /// <summary>
//        /// Deserialize a collection of items.
//        /// </summary>
//        /// <param name="content">The content to deserialize.</param>
//        /// <returns>The list of items that was deserialized.</returns>
//        IEnumerable<object> DeserializeMany(string content);

//        /// <summary>
//        /// Deserialize content into a CLR type.
//        /// </summary>
//        /// <param name="content">The content to deserialize into a CLR type.</param>
//        /// <returns>The instance that was created.</returns>
//        object DeserializeEntity(string content);
//    }
//}
