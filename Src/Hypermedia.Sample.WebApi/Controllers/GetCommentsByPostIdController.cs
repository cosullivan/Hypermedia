using System.Web.Http;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.WebApi.Resources;

namespace Hypermedia.Sample.WebApi.Controllers
{
    public class GetCommentsByPostIdController : ApiController
    {
        readonly IDatabase _database;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="database">The databsae instance.</param>
        public GetCommentsByPostIdController(IDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Returns the comments for the given post id.
        /// </summary>
        /// <param name="postId">The ID of the post to return the comments for.</param>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpGet, Route("v1/posts/{postId}/comments")]
        public IHttpActionResult Execute(int postId)
        {
            var post = _database.Posts.GetById(postId).AsResource();

            if (post == null)
            {
                return NotFound();
            }

            var comments = _database.Comments.GetByPostId(new[] { post.Id }).AsResource();

            foreach (var comment in comments)
            {
                comment.Post = post;
            }

            return Ok(comments);
        }
    }
}
