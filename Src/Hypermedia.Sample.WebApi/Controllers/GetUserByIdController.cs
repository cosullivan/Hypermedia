using System.Linq;
using System.Web.Http;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.WebApi.Controllers
{
    public class GetUserByIdController : ApiController
    {
        readonly IDatabase _database;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="database">The databsae instance.</param>
        public GetUserByIdController(IDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Returns the user that is assigned the given id.
        /// </summary>
        /// <param name="id">The ID of the user to return.</param>
        /// <returns>The HTTP action result that represents the user.</returns>
        [HttpGet, Route("v1/users/{id}")]
        public IHttpActionResult Execute(int id)
        {
            var user = _database.Users.GetById(new [] { id }).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
