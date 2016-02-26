using System;
using System.Web.Http;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using Hypermedia.WebApi;

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
        public IHttpActionResult Execute(CommentResource comment)
        {
            //var post = _database.Posts.GetById(postId).AsResource();

            //if (post == null)
            //{
            //    return NotFound();
            //}

            //var comments = _database.Comments.GetByPostId(new[] { post.Id }).AsResource();

            //foreach (var comment in comments)
            //{
            //    comment.Post = post;
            //}

            //return Ok(comments);

            throw new NotImplementedException();
        }
    }
}