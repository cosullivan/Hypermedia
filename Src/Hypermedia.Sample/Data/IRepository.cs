using System.Collections.Generic;

namespace Hypermedia.Sample.Data
{
    public interface IRepository<out TResource> where TResource : Entity
    {
        /// <summary>
        /// Gets the list of resources with that are assigned the given IDs.
        /// </summary>
        /// <param name="ids">The list of IDs to return the resources for.</param>
        /// <returns>The list of resources that are assigned the given IDs.</returns>
        IReadOnlyList<TResource> GetById(IReadOnlyList<int> ids);

        /// <summary>
        /// Returns all resources up to the supplied limit.
        /// </summary>
        /// <param name="skip">The number of posts to skip.</param>
        /// <param name="take">The limit to apply to the posts being returned.</param>
        /// <returns>The list of resources.</returns>
        IReadOnlyList<TResource> GetAll(int skip = 0, int take = 100);
    }
}
