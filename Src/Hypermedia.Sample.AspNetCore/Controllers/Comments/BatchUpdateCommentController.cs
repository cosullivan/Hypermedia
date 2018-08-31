using System.Collections.Generic;
using System.Threading.Tasks;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Hypermedia.Sample.AspNetCore.Controllers.Comments
{
    [Route("v1/comments")]
    public sealed class BatchUpdateCommentController : ResourceController<Comment, CommentResource>
    {
        /// <summary>
        /// Updates a comment.
        /// </summary>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpOptions, HttpPut, FormatFilter]
        public async Task<IActionResult> ExecuteAsync(IReadOnlyList<CommentResource> comments)
        {
            await Task.CompletedTask;

            return NoContent();
        }
    }
}