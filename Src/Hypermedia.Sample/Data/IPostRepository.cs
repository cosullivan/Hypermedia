using System.Collections.Generic;

namespace Hypermedia.Sample.Data
{
    public interface IPostRepository : IRepository<Post>
    {
        /// <summary>
        /// Gets the list of posts that are assigned to the given user.
        /// </summary>
        /// <param name="userId">The ID of the user to return the posts for.</param>
        /// <returns>The list of posts that are assigned the given IDs.</returns>
        IReadOnlyList<Post> GetByOwnerUserId(int userId);
    }
}
