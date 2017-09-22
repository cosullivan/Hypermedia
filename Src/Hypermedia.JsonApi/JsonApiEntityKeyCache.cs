using System.Collections.Generic;
using System.Linq;
using JsonLite.Ast;

namespace Hypermedia.JsonApi
{
    internal sealed class JsonApiEntityKeyCache : IJsonApiEntityKeyCache
    {
        readonly IDictionary<JsonObject, JsonApiEntityKey> _keyCache = new Dictionary<JsonObject, JsonApiEntityKey>();
        readonly IDictionary<JsonApiEntityKey, JsonObject> _objectCache = new Dictionary<JsonApiEntityKey, JsonObject>(JsonApiEntityKeyEqualityComparer.Instance);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rootObject">The root object to initialize the cache with.</param>
        internal JsonApiEntityKeyCache(JsonObject rootObject)
        {
            Initialize(rootObject);
        }

        /// <summary>
        /// Attempt to return the JSON Entity Key for the given JsonObject.
        /// </summary>
        /// <param name="jsonObject">The JSON object to return the entity key for.</param>
        /// <param name="jsonEntityKey">The JSON entity key that was assigned to the given object.</param>
        /// <returns>true if the entity key could be found for the object, false if not.</returns>
        public bool TryGetKey(JsonObject jsonObject, out JsonApiEntityKey jsonEntityKey)
        {
            return _keyCache.TryGetValue(jsonObject, out jsonEntityKey);
        }

        /// <summary>
        /// Attempt to return the JSON object for the given JSON Entity Key.
        /// </summary>
        /// <param name="jsonEntityKey">The JSON entity key to return the JSON object for.</param>
        /// <param name="jsonObject">The JSON object that was assigned to the given key.</param>
        /// <returns>true if the JSON object was found for the entity key, false if not.</returns>
        public bool TryGetObject(JsonApiEntityKey jsonEntityKey, out JsonObject jsonObject)
        {
            return _objectCache.TryGetValue(jsonEntityKey, out jsonObject);
        }

        /// <summary>
        /// Returns the JSON API entity key for the given object.
        /// </summary>
        /// <param name="jsonObject">The object to return the key for.</param>
        /// <returns>The entity key that is assigned to the given object.</returns>
        public JsonApiEntityKey this[JsonObject jsonObject]
        {
            get
            {
                if (TryGetKey(jsonObject, out JsonApiEntityKey key))
                {
                    return key;
                }

                throw new KeyNotFoundException();
            }
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
                var key = JsonApiEntityKey.CreateKey(jsonObject);

                _keyCache[jsonObject] = key;
                _objectCache[key] = jsonObject;
            }
        }
    }
}