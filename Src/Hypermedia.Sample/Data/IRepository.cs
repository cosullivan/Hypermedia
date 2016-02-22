using System;
using System.Collections.Generic;
using System.Linq;

namespace Hypermedia.Sample.Data
{
    public interface IRepository<out TEntity> where TEntity : Entity
    {
        /// <summary>
        /// Gets the list of entities with that are assigned the given IDs.
        /// </summary>
        /// <param name="ids">The list of IDs to return the entities for.</param>
        /// <returns>The list of entities that are assigned the given IDs.</returns>
        IReadOnlyList<TEntity> GetById(IReadOnlyList<int> ids);

        /// <summary>
        /// Returns all entities up to the supplied limit.
        /// </summary>
        /// <param name="skip">The number of entities to skip.</param>
        /// <param name="take">The limit to apply to the entities being returned.</param>
        /// <returns>The list of entities.</returns>
        IReadOnlyList<TEntity> GetAll(int skip = 0, int take = 100);
    }

    public static class RepositoryExtensions
    {
        /// <summary>
        /// Gets an entity by its ID.
        /// </summary>
        /// <param name="repository">The repository to perform the operation on.</param>
        /// <param name="id">The ID of the entity to return.</param>
        /// <returns>The entity with the given ID.</returns>
        public static TEntity GetById<TEntity>(this IRepository<TEntity> repository, int id) where TEntity : Entity
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            return repository.GetById(new [] { id }).FirstOrDefault();
        }
    }
}
