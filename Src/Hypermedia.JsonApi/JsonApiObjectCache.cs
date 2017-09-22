using System.Collections.Generic;
using System.Linq;
using JsonLite.Ast;

namespace Hypermedia.JsonApi
{
    internal sealed class JsonApiObjectCache
    {
        readonly IDictionary<JsonApiEntityKey, JsonObject> _objectCache = new Dictionary<JsonApiEntityKey, JsonObject>(JsonApiEntityKeyEqualityComparer.Instance);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rootObject">The root object to initialize the cache with.</param>
        internal JsonApiObjectCache(JsonObject rootObject)
        {
            Initialize(rootObject);
        }

        /// <summary>
        /// Initialize the entity key cache which allows for a much faster resolution of relationships.
        /// </summary>
        void Initialize(JsonObject rootObject)
        {
            var data = rootObject["data"] as JsonArray;
            if (data != null)
            {
                Initialize(data.OfType<JsonObject>());
            }

            var included = rootObject["included"] as JsonArray;
            if (included != null)
            {
                Initialize(included.OfType<JsonObject>());
            }
        }

        /// <summary>
        /// Initialize the cache with the given list of objects.
        /// </summary>
        /// <param name="jsonObjects">The objects to initialize the cache with.</param>
        void Initialize(IEnumerable<JsonObject> jsonObjects)
        {
            foreach (var jsonObject in jsonObjects)
            {
                _objectCache[new JsonApiEntityKey(jsonObject)] = jsonObject;
            }
        }

        /// <summary>
        /// Attempt to return the JSON object for the given JSON Entity Key.
        /// </summary>
        /// <param name="jsonEntityKey">The JSON entity key to return the JSON object for.</param>
        /// <param name="jsonObject">The JSON object that was assigned to the given key.</param>
        /// <returns>true if the JSON object was found for the entity key, false if not.</returns>
        internal bool TryGetObject(JsonApiEntityKey jsonEntityKey, out JsonObject jsonObject)
        {
            return _objectCache.TryGetValue(jsonEntityKey, out jsonObject);
        }
    }
}