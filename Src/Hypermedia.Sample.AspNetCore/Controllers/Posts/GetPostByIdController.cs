using System.Threading;
using System.Threading.Tasks;
using Hypermedia.AspNetCore;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Hypermedia.Sample.AspNetCore.Controllers.Posts
{
    [Route("v1/posts")]
    public sealed class GetPostByIdController : ResourceController<Post, PostResource>
    {
        /// <summary>
        /// Returns the posts with the given id.
        /// </summary>
        /// <param name="id">the ID of the post to return.</param>
        /// <param name="requestMetadata">The request metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("{id}"), FormatFilter]
        public async Task<IActionResult> ExecuteAsync(
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