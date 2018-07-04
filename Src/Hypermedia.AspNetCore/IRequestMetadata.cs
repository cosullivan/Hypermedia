using System.Collections.Generic;

namespace Hypermedia.AspNetCore
{
    public interface IRequestMetadata<TResource>
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