using System;
using System.Collections.Generic;
using System.Linq;

namespace Hypermedia.Sample
{
    public static class ResourceExtensions
    {
        /// <summary>
        /// Returns a dictionary of the resouces mapped by their ID.
        /// </summary>
        /// <typeparam name="TResource">The element type of the resource.</typeparam>
        /// <param name="source">The source collection to created the dictionary from.</param>
        /// <returns>The dictionary of resources mapped by their ID.</returns>
        public static IDictionary<int, TResource> ToDictionary<TResource>(this IEnumerable<TResource> source) where TResource : Entity
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.ToDictionary(k => k.Id, v => v);
        }
    }
}