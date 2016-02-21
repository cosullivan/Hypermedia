using System.Web.Http;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.WebApi.Resources;

namespace Hypermedia.Sample.WebApi.Controllers
{
    public class GetPostByOwnerUserIdController : ApiController
    {
        readonly IDatabase _database;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="database">The databsae instance.</param>
        public GetPostByOwnerUserIdController(IDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Returns the posts for the given user id.
        /// </summary>
        /// <param name="userId">The ID of the user to return the posts for.</param>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpGet, Route("v1/users/{userId}/posts")]
        public IHttpActionResult Execute(int userId)
        {
            var posts = _database.Posts.GetByOwnerUserId(userId).AsResource();

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
