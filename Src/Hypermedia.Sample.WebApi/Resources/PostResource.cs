using System.Collections.Generic;

namespace Hypermedia.Sample.WebApi.Resources
{
    public sealed class PostResource : Post
    {
        /// <summary>
        /// The user that owns/created the post.
        /// </summary>
        public UserResource OwnerUser { get; set; }

        /// <summary>
        /// Gets or sets the list of comments for the post.
        /// </summary>
        public IReadOnlyList<CommentResource> Comments { get; set; }
    }
}