using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using Hypermedia.WebApi;

namespace Hypermedia.Sample.WebApi.Controllers.Posts
{
    public sealed class GetPostByIdController : ResourceController<Post, PostResource>
    {
        /// <summary>
        /// Returns the posts with the given id.
        /// </summary>
        /// <param name="id">the ID of the post to return.</param>
        /// <param name="requestMetadata">The request metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpGet, Route("v1/posts/{id}")]
        public async Task<IHttpActionResult> ExecuteAsync(
            int id,
            IRequestMetadata<PostResource> requestMetadata = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var post = await Database.Posts.GetByIdAsync(id, cancellationToken);

            if (post == null)
            {
                return NotFound();
            }

            return await OkAsync(post, requestMetadata, cancellationToken);
        }
    }
}