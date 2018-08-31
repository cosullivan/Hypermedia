using System.Threading;
using System.Threading.Tasks;
using Hypermedia.AspNetCore;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Hypermedia.Sample.AspNetCore.Controllers.Posts
{
    [Route("v1/users/{userId}/posts")]
    public sealed class GetPostByOwnerUserIdController : ResourceController<Post, PostResource>
    {
        /// <summary>
        /// Returns the posts for the given user id.
        /// </summary>
        /// <param name="userId">The ID of the user to return the posts for.</param>
        /// <param name="requestMetadata">The request metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpGet, FormatFilter]
        public async Task<IActionResult> ExecuteAsync(
            int userId, 
            IRequestMetadata<PostResource> requestMetadata = null, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var posts = await Database.Posts.GetByOwnerUserIdAsync(userId, cancellationToken);

            return await OkAsync(posts, requestMetadata, cancellationToken);
        }
    }
}