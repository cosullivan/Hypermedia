using System;
using System.Collections.Generic;
using System.Linq;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.StackOverflow
{
    public abstract class StackOverflowRepository<TEntity> where TEntity : Entity
    {
        readonly IDictionary<int, TEntity> _dictionary;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entities">The list of entities available for the repository.</param>
        protected StackOverflowRepository(IEnumerable<TEntity> entities)
        {
            _dictionary = entities.ToDictionary();
        }

        /// <summary>
        /// Gets the list of entities with that are assigned the given IDs.
        /// </summary>
        /// <param name="ids">The list of IDs to return the entities for.</param>
        /// <returns>The list of entities that are assigned the given IDs.</returns>
        public IReadOnlyList<TEntity> GetById(IReadOnlyList<int> ids)
        {
            return ids.Where(id => Dictionary.ContainsKey(id)).Select(id => Dictionary[id]).ToList();
        }

        /// <summary>
        /// Returns all entities up to the supplied limit.
        /// </summary>
        /// <param name="predicate">The predicate to apply to the entities to determined if they should be returned.</param>
        /// <param name="skip">The number of entities to skip.</param>
        /// <param name="take">The limit to apply to the entities being returned.</param>
        /// <returns>The list of entities.</returns>
        public IReadOnlyList<TEntity> GetAll(Predicate<TEntity> predicate, int skip = 0, int take = 100)
        {
            return Dictionary.Values.Where(entity => predicate(entity)).Skip(skip).Take(take).ToList();
        }

        /// <summary>
        /// Gets the underlying dictionary the contains the items.
        /// </summary>
        protected IDictionary<int, TEntity> Dictionary
        {
            get { return _dictionary; }
        }
    }
}
