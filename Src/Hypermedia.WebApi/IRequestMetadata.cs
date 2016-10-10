using System.Collections.Generic;

namespace Hypermedia.WebApi
{
    public interface IRequestMetadata
    {
        /// <summary>
        /// The list of properties to include in the request.
        /// </summary>
        IReadOnlyList<MemberPath> Include { get; }

        /// <summary>
        /// The list of members to sort the response by.
        /// </summary>
        IReadOnlyList<MemberSortPath> Sort { get; }
    }
}