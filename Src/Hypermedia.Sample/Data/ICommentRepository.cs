using System.Collections.Generic;

namespace Hypermedia.Sample.Data
{
    public interface ICommentRepository : IRepository<Comment>
    {
        /// <summary>
        /// Gets the list of comments that are assigned to the given post id.
        /// </summary>
        /// <param name="postIds">The IDs of the post to return the comments for.</param>
        /// <returns>The list of comments that are assigned to the given list of post ids.</returns>
        IReadOnlyList<Comment> GetByPostId(IReadOnlyList<int> postIds);
    }
}
