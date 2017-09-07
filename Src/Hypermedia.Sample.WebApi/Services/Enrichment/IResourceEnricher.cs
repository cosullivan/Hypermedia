using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hypermedia.Sample.WebApi.Services.Enrichment
{
    public interface IResourceEnricher<in TSourceResource, TDestinationResource>
    {
        /// <summary>
        /// Enrich the list of resources.
        /// </summary>
        /// <param name="resources">The list of resources to enrich.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which asynchronously performs the operation.</returns>
        Task<IReadOnlyList<TDestinationResource>> EnrichAsync(IReadOnlyList<TSourceResource> resources, CancellationToken cancellationToken = default(CancellationToken));
    }
}