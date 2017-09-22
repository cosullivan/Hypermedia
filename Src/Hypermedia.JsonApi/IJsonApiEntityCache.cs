using System;

namespace Hypermedia.JsonApi
{
    public interface IJsonApiEntityCache
    {
        /// <summary>
        /// Attempts to add an element with the provided key and value to the <see cref="T:Hypermedia.JsonApi.IJsonApiEntityCache" />.
        /// </summary>
        /// <param name="type">The resource type of the entity that is being added.</param>
        /// <param name="id">The ID to assign to the entity that is being added.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <returns>true if the entity could be added, false if it could not be added.</returns>
        bool TryAdd(string type, string id, object value);

        /// <summary>
        /// Attempts to get the entity from the cache.
        /// </summary>
        /// <param name="type">The resource type of the entity to find.</param>
        /// <param name="id">The ID of the entity to find.</param>
        /// <param name="value">The entity that is associated with the given type and ID, otherwise the default value is returned.</param>
        /// <returns>true if the object that implements <see cref="T:Hypermedia.JsonApi.IJsonApiEntityCache" /> contains an element with the specified key; otherwise, false.</returns>
        bool TryGetValue(string type, string id, out object value);
    }

    internal static class JsonApiEntityCacheExtensions
    {
        /// <summary>
        /// Attempts to add an element with the provided key and value to the <see cref="T:Hypermedia.JsonApi.IJsonApiEntityCache" />.
        /// </summary>
        /// <param name="cache">The cache to perform the operation on.</param>
        /// <param name="key">The JSON API entity key that is assign to the value.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <returns>true if the entity could be added, false if it could not be added.</returns>
        internal static bool TryAdd(this IJsonApiEntityCache cache, JsonApiEntityKey key, object value)
        {
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            return cache.TryAdd(key.Type, key.Id, value);
        }

        /// <summary>
        /// Attempts to get the entity from the cache.
        /// </summary>
        /// <param name="cache">The cache to perform the operation on.</param>
        /// <param name="key">The JSON API entity key to return the value for.</param>
        /// <param name="value">The value that was associated with the given key.</param>
        /// <returns>true if the an entity was found for the key, false if not.</returns>
        internal static bool TryGetValue(this IJsonApiEntityCache cache, JsonApiEntityKey key, out object value)
        {
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            return cache.TryGetValue(key.Type, key.Id, out value);
        }
    }
}