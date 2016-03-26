using System;
using System.Collections.Generic;
using System.Linq;
using JsonLite.Ast;

namespace Hypermedia.JsonApi
{
    internal class JsonApiEntityKeyEqualityComparer : IEqualityComparer<JsonObject>
    {
        internal static readonly IEqualityComparer<JsonObject> Instance = new JsonApiEntityKeyEqualityComparer();

        /// <summary>
        /// Constructor.
        /// </summary>
        JsonApiEntityKeyEqualityComparer() { }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T"/> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T"/> to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(JsonObject x, JsonObject y)
        {
            return String.Equals(CreateKey(x), CreateKey(y));
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param>
        /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(JsonObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return CreateKey(obj).GetHashCode();
        }

        /// <summary>
        /// Creates a string key representation that is used for comparissons.
        /// </summary>
        /// <param name="jsonObject">The JSON object to create the key for.</param>
        /// <returns>The string key that represents the given JSON object.</returns>
        static string CreateKey(JsonObject jsonObject)
        {
            if (jsonObject["id"] != null)
            {
                return $"{jsonObject["type"].Stringify()}:{jsonObject["id"].Stringify()}".ToLower();
            }

            return jsonObject["type"].Stringify();
        }
    }
}
