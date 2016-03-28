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

        ///// <summary>
        ///// Creates a comment.
        ///// </summary>
        ///// <returns>The HTTP action result that represents the result of the action.</returns>
        //[HttpOptions, HttpPost, Route("v1/comments")]
        //public IHttpActionResult Execute(IPatch<Comment> comment)
        //{
        //    var c = new Comment();
        //    var x = comment.TryPatch(c);

        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Creates a comment.
        /// </summary>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpOptions, HttpPost, Route("v1/comments")]
        public IHttpActionResult Execute(Comment2 comment)
        {
            throw new NotImplementedException();
        }
    }

    public class Comment2
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
}