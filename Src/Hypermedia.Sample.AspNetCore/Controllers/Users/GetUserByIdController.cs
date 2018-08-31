using System.Threading;
using System.Threading.Tasks;
using Hypermedia.AspNetCore;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Hypermedia.Sample.AspNetCore.Controllers.Users
{
    [Route("v1/users/{id}")]
    public sealed class GetUserByIdController : ResourceController<User, UserResource>
    {
        /// <summary>
        /// Returns the user that is assigned the given id.
        /// </summary>
        /// <param name="id">The ID of the user to return.</param>
        /// <param name="requestMetadata">The request metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpGet, FormatFilter]
        public async Task<IActionResult> ExecuteAsync(
            int id,
            IRequestMetadata<UserResource> requestMetadata = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var user = await Database.Users.GetByIdAsync(id, cancellationToken);

            if (user == null)
            {
                return NotFound();
            }

            return await OkAsync(user, requestMetadata, cancellationToken);
        }
    }
}