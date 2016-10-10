using System;
using System.Web.Http;
using Hypermedia.JsonApi.WebApi;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using Hypermedia.Sample.WebApi.Resources;
using Hypermedia.WebApi;

namespace Hypermedia.Sample.WebApi.Controllers
{
    public class GetPostsController : ApiController
    {
        readonly IDatabase _database;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="database">The databsae instance.</param>
        public GetPostsController(IDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Returns the posts from a given offset to a limit.
        /// </summary>
        /// <param name="q">The search text to apply to the results.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <param name="take">The limit to the number of items to return.</param>
        /// <param name="metadata">The request metadtaa.</param>
        /// <returns>The HTTP action result that represents the posts.</returns>
        [HttpGet, Route("v1/posts")]
        public IHttpActionResult Execute(string q = null, int skip = 0, int take = 100, IRequestMetadata metadata = null)
        {
            var posts = _database.Posts.GetAll(Predicate(q), skip, take).AsResource();

            return Ok(posts);
        }

        /// <summary>
        /// Returns a prediate to use for filtering the entities to return.
        /// </summary>
        /// <param name="searchText">The search text to use for the predicate.</param>
        /// <returns>The predicate to use for filtering the list of entities.</returns>
        static Predicate<Post> Predicate(string searchText)
        {
            if (String.IsNullOrWhiteSpace(searchText))
            {
                return post => true;
            }

            return post => String.IsNullOrWhiteSpace(post.Title) == false && post.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) > -1;
        }
    }
}
