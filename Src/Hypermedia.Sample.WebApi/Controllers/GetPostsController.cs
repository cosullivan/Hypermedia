using System.Web.Http;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.WebApi.Resources;

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
        /// <param name="skip">The number of items to skip.</param>
        /// <param name="take">The limit to the number of items to return.</param>
        /// <returns>The HTTP action result that represents the posts.</returns>
        [HttpGet, Route("v1/posts")]
        public IHttpActionResult Execute(int skip = 0, int take = 100)
        {
            var posts = _database.Posts.GetAll(skip, take).AsResource();

            var usersDictionary = _database.Users
                .GetById(
                    posts.SelectDistinctList(post => post.OwnerUserId))
                .AsResource()
                .ToDictionary();

            foreach (var post in posts)
            {
                UserResource user;
                if (usersDictionary.TryGetValue(post.OwnerUserId, out user))
                {
                    post.OwnerUser = user;
                }
            }

            return Ok(posts);
        }
    }
}
