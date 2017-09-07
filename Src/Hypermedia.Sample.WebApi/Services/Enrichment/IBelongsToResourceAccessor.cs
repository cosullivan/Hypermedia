namespace Hypermedia.Sample.WebApi.Services.Enrichment
{
    public interface IBelongsToResourceAccessor<in TSource, in TDestination>
    {
        /// <summary>
        /// Returns the parent value.
        /// </summary>
        /// <param name="source">The source resource to return the foreign key from.</param>
        /// <returns>The value that represents the foreign key </returns>
        int? GetValue(TSource source);

        /// <summary>
        /// Sets the destination value on the source.
        /// </summary>
        /// <param name="source">The source to set the value on.</param>
        /// <param name="destination">The destination instance to set on the source.</param>
        void SetValue(TSource source, TDestination destination);
    }
}