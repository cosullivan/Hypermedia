using Hypermedia.Metadata;

namespace Hypermedia.Sample.AspNetCore.Services.Enrichment
{
    public interface IResourceAccessorFactory
    {
        /// <summary>
        /// Get or create an instance of a belongs to resource accessor.
        /// </summary>
        /// <typeparam name="TSource">The type of the source resource to enrich.</typeparam>
        /// <typeparam name="TDestination">The type of the destination resource that is enriched on the source.</typeparam>
        /// <param name="relationship">The relationship to create the accessor for.</param>
        /// <returns>The belongs to resource accessor.</returns>
        IBelongsToResourceAccessor<TSource, TDestination> GetOrCreateBelongsToAccessor<TSource, TDestination>(IBelongsToRelationship relationship);

        /// <summary>
        /// Get or create an instance of a has many resource accessor.
        /// </summary>
        /// <typeparam name="TSource">The type of the source resource to enrich.</typeparam>
        /// <typeparam name="TDestination">The type of the destination resource that is enriched on the source.</typeparam>
        /// <param name="relationship">The relationship to create the accessor for.</param>
        /// <returns>The has many resource accessor.</returns>
        IHasManyResourceAccessor<TSource, TDestination> GetOrCreateHasManyAccessor<TSource, TDestination>(IHasManyRelationship relationship);
    }
}