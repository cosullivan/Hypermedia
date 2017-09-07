using System;
using System.Collections.Generic;
using System.Linq;

namespace Hypermedia.Sample
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Projects each element of a sequence into a new list form.
        /// </summary>
        /// <param name="source">A sequence of values to invoke a transform function on.</param><param name="selector">A transform function to apply to each element.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements are the result of invoking the transform function on each element of <paramref name="source"/>.</returns>
        public static IReadOnlyList<TResult> SelectList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.Select(selector).ToList();
        }

        /// <summary>
        /// Projects each element of a sequence into a new distinct list form.
        /// </summary>
        /// <param name="source">A sequence of values to invoke a transform function on.</param><param name="selector">A transform function to apply to each element.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements are the result of invoking the transform function on each element of <paramref name="source"/>.</returns>
        public static IReadOnlyList<TResult> SelectDistinctList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.Select(selector).Distinct().ToList();
        }

        /// <summary>
        /// Returns the given input as a read only list.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence of elements to return as a read only list.</param>
        /// <returns>The read only list of elements.</returns>
        public static IReadOnlyList<TSource> ToReadOnlyList<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.ToList();
        }
    }
}
