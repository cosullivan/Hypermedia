using System.Linq;
using System.Web.Http;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.WebApi.Controllers
{
    public class GetUserByIdController : ApiController
    {
        readonly UserRepository _userRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        public GetUserByIdController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Returns the user that is assigned the given id.
        /// </summary>
        /// <param name="id">The ID of the user to return.</param>
        /// <returns>The HTTP action result that represents the user.</returns>
        [HttpGet, Route("v1/users/{id}")]
        public IHttpActionResult Execute(int id)
        {
            var user = _userRepository.GetById(new [] { id }).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
