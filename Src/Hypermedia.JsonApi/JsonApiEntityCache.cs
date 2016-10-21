using System;
using System.Collections.Concurrent;

namespace Hypermedia.JsonApi
{
    public sealed class JsonApiEntityCache : IJsonApiEntityCache
    {
        readonly ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Attempts to add an element with the provided key and value to the <see cref="T:Hypermedia.JsonApi.IJsonApiEntityCache" />.
        /// </summary>
        /// <param name="type">The resource type of the entity that is being added.</param>
        /// <param name="id">The ID to assign to the entity that is being added.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <returns>true if the entity could be added, false if it could not be added.</returns>
        public bool TryAdd(string type, string id, object value)
        {
            return _cache.TryAdd($"{type}:{id}".ToLower(), value);
        }

        /// <summary>
        /// Attempts to get the entity from the cache.
        /// </summary>
        /// <param name="type">The resource type of the entity to find.</param>
        /// <param name="id">The ID of the entity to find.</param>
        /// <param name="value">The entity that is associated with the given type and ID, otherwise the default value is returned.</param>
        /// <returns>true if the object that implements <see cref="T:Hypermedia.JsonApi.IJsonApiEntityCache" /> contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue(string type, string id, out object value)
        {
            return _cache.TryGetValue($"{type}:{id}".ToLower(), out value);
        }
    }
}