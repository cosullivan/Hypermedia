using System;
using System.Collections.Generic;

namespace Hypermedia.Sample.AspNetCore.Services.Enrichment
{
    public sealed class DelegatingHasManyResourceAccessor<TSource, TDestination> : IHasManyResourceAccessor<TSource, TDestination>
    {
        readonly Func<TDestination, int> _foreignKeyGetter;
        readonly Action<TSource, IEnumerable<TDestination>> _foreignKeySetter;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="foreignKeyGetter">The foreign key accessor to return the parent id from the child.</param>
        /// <param name="foreignKeySetter">The foreign key setter to assign the parent to the child.</param>
        public DelegatingHasManyResourceAccessor(Func<TDestination, int> foreignKeyGetter, Action<TSource, IEnumerable<TDestination>> foreignKeySetter)
        {
            _foreignKeyGetter = foreignKeyGetter;
            _foreignKeySetter = foreignKeySetter;
        }

        /// <summary>
        /// Returns the parent value from the child.
        /// </summary>
        /// <param name="destination">The destination resource to return the foreign key from.</param>
        /// <returns>The value that represents the foreign key.</returns>
        public int GetValue(TDestination destination)
        {
            return _foreignKeyGetter(destination);
        }

        /// <summary>
        /// Sets the destination list on the source.
        /// </summary>
        /// <param name="source">The source to set the value on.</param>
        /// <param name="destination">The destination list of resources to set on the source.</param>
        public void SetValue(TSource source, IEnumerable<TDestination> destination)
        {
            _foreignKeySetter(source, destination);
        }
    }
}