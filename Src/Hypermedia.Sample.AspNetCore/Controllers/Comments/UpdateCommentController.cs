using System.Threading.Tasks;
using Hypermedia.AspNetCore;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Hypermedia.Sample.AspNetCore.Controllers.Comments
{
    [Route("v1/comments/{id}")]
    public sealed class UpdateCommentController : ResourceController<Comment, CommentResource>
    {
        /// <summary>
        /// Updates a comment.
        /// </summary>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpOptions, HttpPatch, FormatFilter]
        public async Task<IActionResult> ExecuteAsync(int id, [FromBody] IPatch<CommentResource> patch)
        {
            await Task.CompletedTask;

            return NoContent();
        }
    }
}