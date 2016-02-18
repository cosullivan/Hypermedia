using System.Web.Http;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.WebApi.Controllers
{
    public class GetUsersController : ApiController
    {
        readonly UserRepository _userRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        public GetUsersController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Returns the users from a given offset to a limit.
        /// </summary>
        /// <param name="skip">The number of items to skip.</param>
        /// <param name="limit">The limit to the number of items to return.</param>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpGet, Route("v1/users")]
        public IHttpActionResult Execute(int skip = 0, int limit = 100)
        {
            return Ok(_userRepository.GetAll(skip, limit));
        }
    }
}
