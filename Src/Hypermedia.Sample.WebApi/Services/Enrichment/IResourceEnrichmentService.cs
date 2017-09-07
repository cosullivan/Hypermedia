using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hypermedia.Sample.Data;
using Hypermedia.WebApi;

namespace Hypermedia.Sample.WebApi.Services.Enrichment
{
    public interface IResourceEnrichmentService
    {
        /// <summary>
        /// Enrich the list of resources.
        /// </summary>
        /// <typeparam name="TResource">The resource type to enrich.</typeparam>
        /// <param name="database">The database to perform the enrichment within.</param>
        /// <param name="resources">The list of resources to enrich.</param>
        /// <param name="memberPaths">The list of member access paths that define what to enrich.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which asynchronously performs the operation.</returns>
        Task EnrichAsync<TResource>(
            IDatabase database,
            IReadOnlyList<TResource> resources, 
            IReadOnlyList<MemberPath> memberPaths, 
            CancellationToken cancellationToken = default(CancellationToken));
    }
}