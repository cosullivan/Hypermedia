using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.WebApi.Resources;

namespace Hypermedia.Sample.WebApi.Services.Enrichment
{
    public sealed class DelegatingHasManyResourceEnricher<TSource, TDestinationEntity, TDestination> : IResourceEnricher<TSource, TDestination>
        where TSource : IEntityWithId
        where TDestinationEntity : IEntityWithId
        where TDestination : IEntityWithId
    {
        readonly IHasManyResourceAccessor<TSource, TDestination> _foreignKeyAccessor;
        readonly IRepositoryAccessor<TDestinationEntity> _destinationRepositoryAccessor;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="foreignKeyAccessor">The foreign key accessor responsible accessing the child information for the parent.</param>
        /// <param name="destinationRepositoryAccessor">The destination entity repository accessor.</param>
        public DelegatingHasManyResourceEnricher(
            IHasManyResourceAccessor<TSource, TDestination> foreignKeyAccessor,
            IRepositoryAccessor<TDestinationEntity> destinationRepositoryAccessor)
        {
            _foreignKeyAccessor = foreignKeyAccessor;
            _destinationRepositoryAccessor = destinationRepositoryAccessor;
        }

        /// <summary>
        /// Enrich the list of resources.
        /// </summary>
        /// <param name="resources">The list of resources to enrich.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which asynchronously performs the operation.</returns>
        public async Task<IReadOnlyList<TDestination>> EnrichAsync(IReadOnlyList<TSource> resources, CancellationToken cancellationToken = default(CancellationToken))
        {
            var keys = resources.SelectDistinctList(resource => resource.Id);

            var entities = await _destinationRepositoryAccessor.GetAsync(keys, cancellationToken);

            var destination = entities.SelectList(Resource.Map<TDestinationEntity, TDestination>);
            var destinationLookup = destination.ToLookup(_foreignKeyAccessor.GetValue);

            foreach (var resource in resources)
            {
                _foreignKeyAccessor.SetValue(resource, destinationLookup[resource.Id]);
            }

            return destination;
        }
    }
}