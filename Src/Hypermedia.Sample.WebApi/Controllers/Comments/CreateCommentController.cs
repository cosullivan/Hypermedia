using System.Web.Http;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;

namespace Hypermedia.Sample.WebApi.Controllers.Comments
{
    public sealed class CreateCommentController : ResourceController<Comment, CommentResource>
    {
        /// <summary>
        /// Creates a comment.
        /// </summary>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpOptions, HttpPost, Route("v1/comments")]
        public IHttpActionResult Execute([FromBody] CommentResource comment)
        {
            comment.Id = 123;

            return Created("v1/comments/123", comment);
        }
    }
}