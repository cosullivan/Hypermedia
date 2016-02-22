using System;
using System.Collections.Generic;
using System.Linq;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample
{
    public static class EntityExtensions
    {
        /// <summary>
        /// Returns a dictionary of the entities mapped by their ID.
        /// </summary>
        /// <typeparam name="TEntity">The element type of the entity.</typeparam>
        /// <param name="source">The source collection to created the dictionary from.</param>
        /// <returns>The dictionary of entities mapped by their ID.</returns>
        public static IDictionary<int, TEntity> ToDictionary<TEntity>(this IEnumerable<TEntity> source) where TEntity : Entity
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.ToDictionary(k => k.Id, v => v);
        }
    }
}