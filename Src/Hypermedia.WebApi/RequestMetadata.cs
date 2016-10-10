using System.Collections.Generic;

namespace Hypermedia.WebApi
{
    public sealed class RequestMetadata : IRequestMetadata
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="include">The list of members to include in the response.</param>
        /// <param name="sort">The list of members to sort the response by.</param>
        public RequestMetadata(IReadOnlyList<MemberPath> include, IReadOnlyList<MemberSortPath> sort)
        {
            Include = include;
            Sort = sort;
        }

        /// <summary>
        /// The list of properties to include in the request.
        /// </summary>
        public IReadOnlyList<MemberPath> Include { get; }

        /// <summary>
        /// The list of members to sort the response by.
        /// </summary>
        public IReadOnlyList<MemberSortPath> Sort { get; }
    }
}