using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hypermedia.Sample.WebApi.Services.Enrichment
{
    public sealed class DelegatingRepositoryAccessor<TEntity> : IRepositoryAccessor<TEntity>
    {
        readonly Func<IReadOnlyList<int>, CancellationToken, Task<IReadOnlyList<TEntity>>> _accessor;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="accessor">The accessor function.</param>
        public DelegatingRepositoryAccessor(Func<IReadOnlyList<int>, CancellationToken, Task<IReadOnlyList<TEntity>>> accessor)
        {
            _accessor = accessor;
        }

        /// <summary>
        /// Returns a list of entities that are assigned the given keys.
        /// </summary>
        /// <param name="keys">The list of keys to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of entities that are assigned the keys.</returns>
        public Task<IReadOnlyList<TEntity>> GetAsync(IReadOnlyList<int> keys, CancellationToken cancellationToken)
        {
            return _accessor(keys, cancellationToken);
        }
    }
}