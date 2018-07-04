using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hypermedia.Sample.AspNetCore.Resources;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.AspNetCore.Services.Enrichment
{
    public sealed class DelegatingBelongsToResourceEnricher<TSource, TDestinationEntity, TDestination> : IResourceEnricher<TSource, TDestination> 
        where TDestinationEntity : IEntityWithId
        where TDestination : IEntityWithId
    {
        readonly IBelongsToResourceAccessor<TSource, TDestination> _foreignKeyAccessor;
        readonly IRepositoryAccessor<TDestinationEntity> _destinationRepositoryAccessor;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="foreignKeyAccessor">The foreign key accessor responsible accessing the parent information from the child.</param>
        /// <param name="destinationRepositoryAccessor">The destination entity repository accessor.</param>
        public DelegatingBelongsToResourceEnricher(
            IBelongsToResourceAccessor<TSource, TDestination> foreignKeyAccessor,
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
            // ReSharper disable once PossibleInvalidOperationException
            var keys = resources.Where(r => _foreignKeyAccessor.GetValue(r).HasValue).SelectDistinctList(r => _foreignKeyAccessor.GetValue(r).Value);

            var entities = await _destinationRepositoryAccessor.GetAsync(keys, cancellationToken);

            var destination = entities.SelectList(Resource.Map<TDestinationEntity, TDestination>);
            var destinationLookup = destination.ToDictionary(entity => entity.Id);

            foreach (var resource in resources.Where(r => _foreignKeyAccessor.GetValue(r).HasValue))
            {
                // ReSharper disable once PossibleInvalidOperationException
                if (destinationLookup.TryGetValue(_foreignKeyAccessor.GetValue(resource).Value, out var destinationResource))
                {
                    _foreignKeyAccessor.SetValue(resource, destinationResource);
                }
            }

            return destination;
        }
    }
}