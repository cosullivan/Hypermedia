using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using Hypermedia.WebApi;

namespace Hypermedia.Sample.WebApi.Controllers.Users
{
    public sealed class GetUsersController : ResourceController<User, UserResource>
    {
        /// <summary>
        /// Returns te list of users.
        /// </summary>
        /// <param name="requestMetadata">The request metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpGet, Route("v1/users")]
        public async Task<IHttpActionResult> ExecuteAsync(
            IRequestMetadata<UserResource> requestMetadata = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var users = await Database.Users.GetAllAsync(user => true, cancellationToken: cancellationToken);

            return await OkAsync(users, requestMetadata, cancellationToken);
        }
    }
}