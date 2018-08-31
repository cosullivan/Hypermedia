using System.Threading;
using System.Threading.Tasks;
using Hypermedia.AspNetCore;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Hypermedia.Sample.AspNetCore.Controllers.Comments
{
    [Route("v1/posts/{postId}/comments")]
    public sealed class GetCommentsByPostIdController : ResourceController<Comment, CommentResource>
    {
        /// <summary>
        /// Returns the comments that are assigned to a post.
        /// </summary>
        /// <param name="postId">the ID of the post to return the comments for.</param>
        /// <param name="requestMetadata">The request metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpGet, FormatFilter]
        public async Task<IActionResult> ExecuteAsync(
            int postId,
            IRequestMetadata<CommentResource> requestMetadata = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var post = await Database.Posts.GetByIdAsync(postId, cancellationToken);

            if (post == null)
            {
                return NotFound();
            }

            var comments = await Database.Comments.GetByPostIdAsync(new[] { post.Id }, cancellationToken);
            
            return await OkAsync(comments, requestMetadata, cancellationToken);
        }
    }
}