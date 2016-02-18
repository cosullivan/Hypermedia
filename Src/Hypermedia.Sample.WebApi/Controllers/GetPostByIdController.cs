using System.Linq;
using System.Web.Http;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.WebApi.Controllers
{
    public class GetPostByIdController : ApiController
    {
        readonly PostRepository _postRepository;
        readonly UserRepository _userRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="postRepository">The post repository.</param>
        /// <param name="userRepository">The user repository.</param>
        public GetPostByIdController(PostRepository postRepository, UserRepository userRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Returns the posts with the given id.
        /// </summary>
        /// <param name="id">the ID of the post to return.</param>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpGet, Route("v1/posts/{id}")]
        public IHttpActionResult Execute(int id)
        {
            var post = _postRepository.GetById(new[] { id }).FirstOrDefault();

            if (post == null)
            {
                return NotFound();
            }

            post.OwnerUser = _userRepository.GetById(new[] { post.OwnerUserId }).FirstOrDefault();
            
            return Ok(post);
        }
    }
}
