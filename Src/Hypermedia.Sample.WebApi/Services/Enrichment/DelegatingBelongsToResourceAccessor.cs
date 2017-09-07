using System;

namespace Hypermedia.Sample.WebApi.Services.Enrichment
{
    public sealed class DelegatingBelongsToResourceAccessor<TSource, TDestination> : IBelongsToResourceAccessor<TSource, TDestination>
    {
        readonly Func<TSource, int?> _foreignKeyGetter;
        readonly Action<TSource, TDestination> _foreignKeySetter;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="foreignKeyGetter">The foreign key accessor to return the parent id from the child.</param>
        /// <param name="foreignKeySetter">The foreign key setter to assign the parent to the child.</param>
        public DelegatingBelongsToResourceAccessor(Func<TSource, int?> foreignKeyGetter, Action<TSource, TDestination> foreignKeySetter)
        {
            _foreignKeyGetter = foreignKeyGetter;
            _foreignKeySetter = foreignKeySetter;
        }

        /// <summary>
        /// Returns the parent value.
        /// </summary>
        /// <param name="source">The source resource to return the foreign key from.</param>
        /// <returns>The value that represents the foreign key </returns>
        public int? GetValue(TSource source)
        {
            return _foreignKeyGetter(source);
        }

        /// <summary>
        /// Sets the destination value on the source.
        /// </summary>
        /// <param name="source">The source to set the value on.</param>
        /// <param name="destination">The destination instance to set on the source.</param>
        public void SetValue(TSource source, TDestination destination)
        {
            _foreignKeySetter(source, destination);
        }
    }
}