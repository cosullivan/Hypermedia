using System.Threading;
using System.Threading.Tasks;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Hypermedia.Sample.AspNetCore.Controllers.Posts
{
    //public sealed class GetPostByIdController  : ResourceController<Post, PostResource>
    //{
    //    /// <summary>
    //    /// Returns the posts with the given id.
    //    /// </summary>
    //    /// <param name="id">the ID of the post to return.</param>
    //    /// <param name="requestMetadata">The request metadata.</param>
    //    /// <param name="cancellationToken">The cancellation token.</param>
    //    /// <returns>The HTTP action result that represents the result of the action.</returns>
    //    [HttpGet, Route("v1/posts/{id}")]
    //    public async Task<IHttpActionResult> ExecuteAsync(
    //        int id,
    //        IRequestMetadata<PostResource> requestMetadata = null,
    //        CancellationToken cancellationToken = default(CancellationToken))
    //    {
    //        var post = await Database.Posts.GetByIdAsync(id, cancellationToken);

    //        if (post == null)
    //        {
    //            return NotFound();
    //        }

    //        return await OkAsync(post, requestMetadata, cancellationToken);
    //    }
    //}

    [Route("v1/posts")]
    public sealed class GetPostByIdController : Controller
    {
        [HttpGet("{id}"), FormatFilter]
        public IActionResult ExecuteAsync(
            int id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Ok(new PostResource());

            //var post = await Database.Posts.GetByIdAsync(id, cancellationToken);

            //if (post == null)
            //{
            //    return NotFound();
            //}

            //return await OkAsync(post, requestMetadata, cancellationToken);
        }
    }
}