using System.Web.Http;
using Hypermedia.Sample.Data;
using Hypermedia.Sample;

namespace Hypermedia.Sample.WebApi.Controllers
{
    public class GetPostsController : ApiController
    {
        readonly PostRepository _postRepository;
        readonly UserRepository _userRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="postRepository">The post repository.</param>
        /// <param name="userRepository">The user repository.</param>
        public GetPostsController(PostRepository postRepository, UserRepository userRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Returns the posts from a given offset to a limit.
        /// </summary>
        /// <param name="skip">The number of items to skip.</param>
        /// <param name="limit">The limit to the number of items to return.</param>
        /// <returns>The HTTP action result that represents the posts.</returns>
        [HttpGet, Route("v1/posts")]
        public IHttpActionResult Execute(int skip = 0, int limit = 100)
        {
            var posts = _postRepository.GetAll(skip, limit);

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
