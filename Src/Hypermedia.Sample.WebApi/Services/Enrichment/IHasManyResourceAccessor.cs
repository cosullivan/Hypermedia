using System.Collections.Generic;

namespace Hypermedia.Sample.WebApi.Services.Enrichment
{
    public interface IHasManyResourceAccessor<in TSource, in TDestination>
    {
        /// <summary>
        /// Returns the parent value from the child.
        /// </summary>
        /// <param name="destination">The destination resource to return the foreign key from.</param>
        /// <returns>The value that represents the foreign key.</returns>
        int GetValue(TDestination destination);

        /// <summary>
        /// Sets the destination list on the source.
        /// </summary>
        /// <param name="source">The source to set the value on.</param>
        /// <param name="destination">The destination list of resources to set on the source.</param>
        void SetValue(TSource source, IEnumerable<TDestination> destination);
    }
}