using Hypermedia.Metadata;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.WebApi.Services.Enrichment
{
    public interface IResourceEnricherFactory
    {
        /// <summary>
        /// Create the enricher for the given resource type.
        /// </summary>
        /// <typeparam name="TSource">The resource type to create the enricher for.</typeparam>
        /// <typeparam name="TDestination">The resource type on the other end of the relationship that is being enriched from.</typeparam>
        /// <param name="relationship">The relationship to create the enricher from.</param>
        /// <param name="database">The database to perform the enrichment within.</param>
        /// <returns>The enricher for the given resource and relationship, or null if no enricher could be provided.</returns>
        IResourceEnricher<TSource, TDestination> CreateEnricher<TSource, TDestination>(IRelationship relationship, IDatabase database);
    }
}