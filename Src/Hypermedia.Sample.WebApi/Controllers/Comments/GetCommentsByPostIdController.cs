using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using Hypermedia.WebApi;

namespace Hypermedia.Sample.WebApi.Controllers.Comments
{
    public sealed class GetCommentsByPostIdController : ResourceController<Comment, CommentResource>
    {
        /// <summary>
        /// Returns the comments that are assigned to a post.
        /// </summary>
        /// <param name="id">the ID of the post to return the comments for.</param>
        /// <param name="requestMetadata">The request metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpGet, Route("v1/posts/{postId}/comments")]
        public async Task<IHttpActionResult> ExecuteAsync(
            int id,
            IRequestMetadata<CommentResource> requestMetadata = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var post = await Database.Posts.GetByIdAsync(id, cancellationToken);

            if (post == null)
            {
                return NotFound();
            }

            var comments = await Database.Comments.GetByPostIdAsync(new[] { post.Id }, cancellationToken);
            
            return await OkAsync(comments, requestMetadata, cancellationToken);
        }
    }
}