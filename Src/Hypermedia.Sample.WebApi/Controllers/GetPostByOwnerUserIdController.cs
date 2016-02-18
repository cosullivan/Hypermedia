using System.Web.Http;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.WebApi.Controllers
{
    public class GetPostByOwnerUserIdController : ApiController
    {
        readonly PostRepository _postRepository;
        readonly UserRepository _userRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="postRepository">The post repository.</param>
        /// <param name="userRepository">The user repository.</param>
        public GetPostByOwnerUserIdController(PostRepository postRepository, UserRepository userRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Returns the posts for the given user id.
        /// </summary>
        /// <param name="userId">The ID of the user to return the posts for.</param>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpGet, Route("v1/users/{userId}/posts")]
        public IHttpActionResult Execute(int userId)
        {
            var posts = _postRepository.GetByOwnerUserId(userId);

            var usersDictionary = _userRepository
                .GetById(
                    posts.SelectDistinctList(post => post.OwnerUserId))
                .ToDictionary();

            foreach (var post in posts)
            {
                User user;
                if (usersDictionary.TryGetValue(post.OwnerUserId, out user))
                {
                    post.OwnerUser = user;
                }
            }

            return Ok(posts);
        }
    }
}
