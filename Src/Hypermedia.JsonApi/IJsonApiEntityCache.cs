using System;
using JsonLite.Ast;

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

    public static class JsonApiEntityCacheExtensions
    {
        /// <summary>
        /// Attempts to adds an entity to the <see cref="T:Hypermedia.JsonApi.IJsonApiEntityCache" />.
        /// </summary>
        /// <param name="cache">The cache to add the entity to.</param>
        /// <param name="key">The item that defines the key for the entity to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <returns>true if the entity could be added, false if it could not be added.</returns>
        public static bool TryAdd(this IJsonApiEntityCache cache, JsonObject key, object value)
        {
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            string type;
            string id;
            if (TryCreateKey(key, out type, out id) == false)
            {
                return false;
            }

            return cache.TryAdd(type, id, value);
        }

        /// <summary>
        /// Attempts to get the entity from the cache.
        /// </summary>
        /// <param name="cache">The cache to add the entity to.</param>
        /// <param name="key">The item that defines the key for the entity to locate.</param>
        /// <param name="value">The entity that is associated with the given type and ID, otherwise the default value is returned.</param>
        /// <returns>true if the object that implements <see cref="T:Hypermedia.JsonApi.IJsonApiEntityCache" /> contains an element with the specified key; otherwise, false.</returns>
        public static bool TryGetValue(this IJsonApiEntityCache cache, JsonObject key, out object value)
        {
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            string type;
            string id;
            if (TryCreateKey(key, out type, out id) == false)
            {
                value = null;
                return false;
            }

            return cache.TryGetValue(type, id, out value);
        }

        /// <summary>
        /// Attempts to create the key components of an entity.
        /// </summary>
        /// <param name="jsonObject">The JSON object to create the key for.</param>
        /// <param name="type">The type of the entity.</param>
        /// <param name="id">The ID of the entity.</param>
        /// <returns>true if the required components could be created, false if not.</returns>
        static bool TryCreateKey(JsonObject jsonObject, out string type, out string id)
        {
            type = null;
            id = null;

            if (jsonObject["id"] == null)
            {
                return false;
            }

            type = jsonObject["type"].Stringify().ToLower();
            id = jsonObject["id"].Stringify().ToLower();

            return true;
        }
    }
}