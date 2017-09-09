using System.Threading.Tasks;
using System.Web.Http;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using Hypermedia.WebApi;

namespace Hypermedia.Sample.WebApi.Controllers.Comments
{
    public sealed class UpdateCommentController : ResourceController<Comment, CommentResource>
    {
        /// <summary>
        /// Updates a comment.
        /// </summary>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpOptions, HttpPatch, Route("v1/comments/{id}")]
        public async Task<IHttpActionResult> ExecuteAsync(int id, IPatch<CommentResource> patch)
        {
            await Task.CompletedTask;

            return NoContent();
        }
    }
}