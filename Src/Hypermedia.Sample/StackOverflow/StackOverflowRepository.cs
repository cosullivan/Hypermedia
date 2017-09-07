using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.StackOverflow
{
    public abstract class StackOverflowRepository<TEntity> where TEntity : Entity
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entities">The list of entities available for the repository.</param>
        protected StackOverflowRepository(IEnumerable<TEntity> entities)
        {
            Dictionary = entities.ToDictionary();
        }

        /// <summary>
        /// Gets the list of entities with that are assigned the given IDs.
        /// </summary>
        /// <param name="ids">The list of IDs to return the entities for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of entities that are assigned the given IDs.</returns>
        public Task<IReadOnlyList<TEntity>> GetByIdAsync(IReadOnlyList<int> ids, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entities = ids.Where(id => Dictionary.ContainsKey(id)).Select(id => Dictionary[id]).ToReadOnlyList();

            return Task.FromResult(entities);
        }

        /// <summary>
        /// Returns all entities up to the supplied limit.
        /// </summary>
        /// <param name="predicate">The predicate to apply to the entities to determined if they should be returned.</param>
        /// <param name="skip">The number of entities to skip.</param>
        /// <param name="take">The limit to apply to the entities being returned.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of entities.</returns>
        public Task<IReadOnlyList<TEntity>> GetAllAsync(Predicate<TEntity> predicate, int skip = 0, int take = 100, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entities = Dictionary.Values.Where(entity => predicate(entity)).Skip(skip).Take(take).ToReadOnlyList();

            return Task.FromResult(entities);
        }

        /// <summary>
        /// Gets the underlying dictionary the contains the items.
        /// </summary>
        protected IDictionary<int, TEntity> Dictionary { get; }
    }
}