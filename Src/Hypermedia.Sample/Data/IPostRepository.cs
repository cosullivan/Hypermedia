using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hypermedia.Sample.Data
{
    public interface IPostRepository : IRepository<Post>
    {
        /// <summary>
        /// Gets the list of posts that are assigned to the given user.
        /// </summary>
        /// <param name="userId">The ID of the user to return the posts for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of posts that are assigned the given IDs.</returns>
        Task<IReadOnlyList<Post>> GetByOwnerUserIdAsync(int userId, CancellationToken cancellationToken = default(CancellationToken));
    }
}