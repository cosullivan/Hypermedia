using System.Web.Http;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.WebApi.Controllers
{
    public class GetUsersController : ApiController
    {
        readonly IDatabase _database;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="database">The databsae instance.</param>
        public GetUsersController(IDatabase database)
        {
            _database = database;
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
            return Ok(_database.Users.GetAll(skip, limit));
        }
    }
}
