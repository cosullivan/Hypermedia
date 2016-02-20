using System.Collections.Generic;
using System.Linq;

namespace Hypermedia.Sample.StackOverflow
{
    public abstract class StackOverflowRepository<TResource> where TResource : Resource
    {
        readonly IDictionary<int, TResource> _dictionary;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="resources">The list of resources available for the repository.</param>
        protected StackOverflowRepository(IEnumerable<TResource> resources)
        {
            _dictionary = resources.ToDictionary();
        }

        /// <summary>
        /// Gets the list of resources with that are assigned the given IDs.
        /// </summary>
        /// <param name="ids">The list of IDs to return the resources for.</param>
        /// <returns>The list of resources that are assigned the given IDs.</returns>
        public IReadOnlyList<TResource> GetById(IReadOnlyList<int> ids)
        {
            return ids.Where(id => Dictionary.ContainsKey(id)).Select(id => Dictionary[id]).ToList();
        }

        /// <summary>
        /// Returns all resources up to the supplied limit.
        /// </summary>
        /// <param name="skip">The number of posts to skip.</param>
        /// <param name="limit">The limit to apply to the posts being returned.</param>
        /// <returns>The list of resources.</returns>
        public IReadOnlyList<TResource> GetAll(int skip = 0, int limit = 100)
        {
            return Dictionary.Values.Skip(skip).Take(limit).ToList();
        }

        /// <summary>
        /// Gets the underlying dictionary the contains the items.
        /// </summary>
        protected IDictionary<int, TResource> Dictionary
        {
            get { return _dictionary; }
        }
    }
}
