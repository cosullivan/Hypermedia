using System;
using Hypermedia.Metadata;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.AspNetCore.Services.Enrichment
{
    public sealed class CompositeResourceEnricherFactory : IResourceEnricherFactory
    {
        readonly IResourceEnricherFactory[] _factories;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="factories">The list of factories to delegate to.</param>
        public CompositeResourceEnricherFactory(params IResourceEnricherFactory[] factories)
        {
            _factories = factories;
        }

        /// <summary>
        /// Create the enricher for the given resource type.
        /// </summary>
        /// <typeparam name="TSource">The resource type to create the enricher for.</typeparam>
        /// <typeparam name="TDestination">The resource type on the other end of the relationship that is being enriched from.</typeparam>
        /// <param name="relationship">The relationship to create the enricher from.</param>
        /// <param name="database">The database to perform the enrichment within.</param>
        /// <returns>The enricher for the given resource and relationship, or null if no enricher could be provided.</returns>
        public IResourceEnricher<TSource, TDestination> CreateEnricher<TSource, TDestination>(IRelationship relationship, IDatabase database)
        {
            foreach (var factory in _factories)
            {
                var enricher = factory.CreateEnricher<TSource, TDestination>(relationship, database);

                if (enricher != null)
                {
                    return enricher;
                }
            }

            throw new ArgumentException("Could not find a compatible enricher for the given arguments.");
        }
    }
}