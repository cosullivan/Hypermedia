using System.Threading;
using System.Threading.Tasks;
using Hypermedia.AspNetCore;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Hypermedia.Sample.AspNetCore.Controllers.Users
{
    [Route("v1/users")]
    public sealed class GetUsersController : ResourceController<User, UserResource>
    {
        /// <summary>
        /// Returns te list of users.
        /// </summary>
        /// <param name="requestMetadata">The request metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpGet, FormatFilter]
        public async Task<IActionResult> ExecuteAsync(
            IRequestMetadata<UserResource> requestMetadata = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var users = await Database.Users.GetAllAsync(user => true, cancellationToken: cancellationToken);

            return await OkAsync(users, requestMetadata, cancellationToken);
        }
    }
}