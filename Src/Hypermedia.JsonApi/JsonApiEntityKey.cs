using System;
using JsonLite.Ast;

namespace Hypermedia.JsonApi
{
    internal struct JsonApiEntityKey
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The type of the entity.</param>
        /// <param name="id">The ID of the entity.</param>
        internal JsonApiEntityKey(string type, string id)
        {
            // ReSharper disable once JoinNullCheckWithUsage
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // ReSharper disable once JoinNullCheckWithUsage
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Type = type.ToLower();
            Id = id.ToLower();
        }

        /// <summary>
        /// Creates a key representation that is used for comparissons.
        /// </summary>
        /// <param name="jsonObject">The JSON object to create the key for.</param>
        /// <returns>The key that represents the given JSON object.</returns>
        internal static JsonApiEntityKey CreateKey(JsonObject jsonObject)
        {
            if (jsonObject["id"] != null)
            {
                return new JsonApiEntityKey(jsonObject["type"].Stringify(), jsonObject["id"].Stringify());
            }

            return new JsonApiEntityKey(jsonObject["type"].Stringify(), String.Empty);
        }

        /// <summary>
        /// The type of the entity.
        /// </summary>
        internal string Type { get; }

        /// <summary>
        /// The ID of the entity.
        /// </summary>
        internal string Id { get; }
    }
}