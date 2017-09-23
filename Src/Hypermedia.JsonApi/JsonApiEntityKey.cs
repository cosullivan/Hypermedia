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
        JsonApiEntityKey(string type, string id)
        {
            // ReSharper disable once JoinNullCheckWithUsage
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Type = type;
            Id = id;
        }

        /// <summary>
        /// Create an instance of an entity key for the JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object to create the entity key for.</param>
        /// <returns>The entity key for the given JSON object.</returns>
        internal static JsonApiEntityKey Create(JsonObject jsonObject)
        {
            var id = jsonObject["id"];

            return new JsonApiEntityKey(((JsonString)jsonObject["type"]).Value, id?.Stringify());
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