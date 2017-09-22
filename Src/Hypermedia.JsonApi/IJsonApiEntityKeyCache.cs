using JsonLite.Ast;

namespace Hypermedia.JsonApi
{
    internal interface IJsonApiEntityKeyCache
    {
        /// <summary>
        /// Attempt to return the JSON Entity Key for the given JsonObject.
        /// </summary>
        /// <param name="jsonObject">The JSON object to return the entity key for.</param>
        /// <param name="jsonEntityKey">The JSON entity key that was assigned to the given object.</param>
        /// <returns>true if the entity key could be found for the object, false if not.</returns>
        bool TryGetKey(JsonObject jsonObject, out JsonApiEntityKey jsonEntityKey);

        /// <summary>
        /// Attempt to return the JSON object for the given JSON Entity Key.
        /// </summary>
        /// <param name="jsonEntityKey">The JSON entity key to return the JSON object for.</param>
        /// <param name="jsonObject">The JSON object that was assigned to the given key.</param>
        /// <returns>true if the JSON object was found for the entity key, false if not.</returns>
        bool TryGetObject(JsonApiEntityKey jsonEntityKey, out JsonObject jsonObject);

        /// <summary>
        /// Returns the JSON API entity key for the given object.
        /// </summary>
        /// <param name="jsonObject">The object to return the key for.</param>
        /// <returns>The entity key that is assigned to the given object.</returns>
        JsonApiEntityKey this[JsonObject jsonObject] { get; }
    }
}