using System;
using System.Threading;
using System.Threading.Tasks;
using Hypermedia.AspNetCore;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Hypermedia.Sample.AspNetCore.Controllers.Posts
{
    [Route("v1/posts")]
    public sealed class GetPostsController : ResourceController<Post, PostResource>
    {
        /// <summary>
        /// Returns all of the posts matching the search criteria.
        /// </summary>
        /// <param name="q">The search text to apply to the results.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <param name="take">The limit to the number of items to return.</param>
        /// <param name="requestMetadata">The request metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpGet, FormatFilter]
        public async Task<IActionResult> ExecuteAsync(
            string q = null, 
            int skip = 0, 
            int take = 100,
            IRequestMetadata<PostResource> requestMetadata = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var posts = await Database.Posts.GetAllAsync(Predicate(q), skip, take, cancellationToken);

            return await OkAsync(posts, requestMetadata, cancellationToken);
        }
        
        /// <summary>
        /// Returns a prediate to use for filtering the entities to return.
        /// </summary>
        /// <param name="searchText">The search text to use for the predicate.</param>
        /// <returns>The predicate to use for filtering the list of entities.</returns>
        static Predicate<Post> Predicate(string searchText)
        {
            if (String.IsNullOrWhiteSpace(searchText))
            {
                return post => true;
            }

            return post => String.IsNullOrWhiteSpace(post.Title) == false && post.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) > -1;
        }
    }
}