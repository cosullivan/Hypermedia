using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hypermedia.Sample.WebApi.Services.Enrichment
{
    public interface IRepositoryAccessor<TEntity>
    {
        /// <summary>
        /// Returns a list of entities that are assigned the given keys.
        /// </summary>
        /// <param name="keys">The list of keys to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of entities that are assigned the keys.</returns>
        Task<IReadOnlyList<TEntity>> GetAsync(IReadOnlyList<int> keys, CancellationToken cancellationToken);
    }
}