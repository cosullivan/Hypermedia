using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hypermedia.Sample.Data
{
    public interface ICommentRepository : IRepository<Comment>
    {
        /// <summary>
        /// Gets the list of comments that are assigned to the given post id.
        /// </summary>
        /// <param name="postIds">The IDs of the post to return the comments for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of comments that are assigned to the given list of post ids.</returns>
        Task<IReadOnlyList<Comment>> GetByPostIdAsync(IReadOnlyList<int> postIds, CancellationToken cancellationToken = default(CancellationToken));
    }
}