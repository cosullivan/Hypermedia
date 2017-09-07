using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hypermedia.Sample.Data
{
    public interface IRepository<TEntity> where TEntity : IEntityWithId
    {
        /// <summary>
        /// Gets the list of entities with that are assigned the given IDs.
        /// </summary>
        /// <param name="ids">The list of IDs to return the entities for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of entities that are assigned the given IDs.</returns>
        Task<IReadOnlyList<TEntity>> GetByIdAsync(IReadOnlyList<int> ids, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns all entities up to the supplied limit.
        /// </summary>
        /// <param name="predicate">The predicate to apply to the entities to determined if they should be returned.</param>
        /// <param name="skip">The number of entities to skip.</param>
        /// <param name="take">The limit to apply to the entities being returned.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of entities.</returns>
        Task<IReadOnlyList<TEntity>> GetAllAsync(Predicate<TEntity> predicate, int skip = 0, int take = 100, CancellationToken cancellationToken = default(CancellationToken));
    }

    public static class RepositoryExtensions
    {
        /// <summary>
        /// Returns all entities up to the supplied limit.
        /// </summary>
        /// <param name="repository">The repository to perform the operation on.</param>
        /// <param name="skip">The number of entities to skip.</param>
        /// <param name="take">The limit to apply to the entities being returned.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of entities.</returns>
        public static Task<IReadOnlyList<TEntity>> GetAllAsync<TEntity>(
            this IRepository<TEntity> repository, 
            int skip = 0, 
            int take = 100,
            CancellationToken cancellationToken = default(CancellationToken)) where TEntity : Entity
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            return repository.GetAllAsync(entity => true, skip, take, cancellationToken);
        }

        /// <summary>
        /// Gets an entity by its ID.
        /// </summary>
        /// <param name="repository">The repository to perform the operation on.</param>
        /// <param name="id">The ID of the entity to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The entity with the given ID.</returns>
        public static async Task<TEntity> GetByIdAsync<TEntity>(
            this IRepository<TEntity> repository, 
            int id,
            CancellationToken cancellationToken = default(CancellationToken)) where TEntity : Entity
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var entities = await repository.GetByIdAsync(new [] { id }, cancellationToken);

            return entities.FirstOrDefault();
        }
    }
}