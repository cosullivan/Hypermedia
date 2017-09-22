using System;
using System.Collections.Generic;

namespace Hypermedia.JsonApi
{
    internal sealed class JsonApiEntityKeyEqualityComparer : IEqualityComparer<JsonApiEntityKey>
    {
        internal static readonly IEqualityComparer<JsonApiEntityKey> Instance = new JsonApiEntityKeyEqualityComparer();

        /// <summary>
        /// Constructor.
        /// </summary>
        JsonApiEntityKeyEqualityComparer() { }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(JsonApiEntityKey x, JsonApiEntityKey y)
        {
            return String.Equals(x.Type, y.Type, StringComparison.OrdinalIgnoreCase) && String.Equals(x.Id, y.Id, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is null.</exception>
        public int GetHashCode(JsonApiEntityKey obj)
        {
            return String.Concat(obj.Type, "/", obj.Id).GetHashCode();
        }
    }
}