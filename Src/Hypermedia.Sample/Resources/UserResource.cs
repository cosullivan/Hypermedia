using System.Collections.Generic;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.Resources
{
    public sealed class UserResource : User
    {
        /// <summary>
        /// The list of posts that are assigned to the user.
        /// </summary>
        public IReadOnlyList<PostResource> Posts { get; set; }
    }
}