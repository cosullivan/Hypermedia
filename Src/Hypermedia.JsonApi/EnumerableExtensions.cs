using System;
using System.Linq;
using System.Collections.Generic;

namespace Hypermedia.JsonApi
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns a default sequence if the source is null.
        /// </summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <param name="source">The source to test for.</param>
        /// <returns>The default sequence.</returns>
        public static IEnumerable<T> DefaultIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? new T[0];
        }

        /// <summary>
        /// Returns a sequence with the additional element added.
        /// </summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <param name="source">The sequence to append the element to.</param>
        /// <param name="element">The element to append to the sequence.</param>
        /// <returns>The sequence with the element appended.</returns>
        public static IEnumerable<T> Union<T>(this IEnumerable<T> source, T element)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.Union(new T[] { element });
        }
    }
}