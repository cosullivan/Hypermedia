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
        /// <param name="predicate">The predicate to apply to the entities to determined if they should be returned.</param>
        /// <param name="skip">The number of entities to skip.</param>
        /// <param name="take">The limit to apply to the entities being returned.</param>
        /// <returns>The list of entities.</returns>
        IReadOnlyList<TEntity> GetAll(Predicate<TEntity> predicate, int skip = 0, int take = 100);
    }

    public static class RepositoryExtensions
    {
        /// <summary>
        /// Returns all entities up to the supplied limit.
        /// </summary>
        /// <param name="repository">The repository to perform the operation on.</param>
        /// <param name="skip">The number of entities to skip.</param>
        /// <param name="take">The limit to apply to the entities being returned.</param>
        /// <returns>The list of entities.</returns>
        public static IReadOnlyList<TEntity> GetAll<TEntity>(this IRepository<TEntity> repository, int skip = 0, int take = 100) where TEntity : Entity
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            return repository.GetAll(entity => true, skip, take);
        }

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
