using System.Collections.Generic;
using System.Linq;
using Hypermedia.Metadata;

namespace Hypermedia.AspNetCore
{
    public sealed class MemberSortPath
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">The path to the resulting member to sort by.</param>
        /// <param name="direction">The direction of the sort.</param>
        public MemberSortPath(IEnumerable<IMember> path, SortDirection direction)
        {
            Path = path.ToList();
            Direction = direction;
        }

        /// <summary>
        /// The list of members that define the full path to the member to sort by.
        /// </summary>
        public IReadOnlyList<IMember> Path { get; }

        /// <summary>
        /// The sort direction.
        /// </summary>
        public SortDirection Direction { get; set; }
    }
}