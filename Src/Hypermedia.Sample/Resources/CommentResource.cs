using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.Resources
{
    public sealed class CommentResource : Comment
    {
        /// <summary>
        /// Gets or sets the user that created the comment.
        /// </summary>
        public UserResource User { get; set; }

        /// <summary>
        /// Gets or sets the post that the comment belongs to.
        /// </summary>
        public PostResource Post { get; set; }
    }
}