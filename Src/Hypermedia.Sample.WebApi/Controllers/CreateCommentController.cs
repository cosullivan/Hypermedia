using System.Web.Http;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;

namespace Hypermedia.Sample.WebApi.Controllers
{
    public class CreateCommentController : ApiController
    {
        readonly IDatabase _database;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="database">The databsae instance.</param>
        public CreateCommentController(IDatabase database)
        {
            _database = database;
        }

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