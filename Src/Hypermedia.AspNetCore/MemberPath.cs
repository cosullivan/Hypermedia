using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Hypermedia.Metadata;

namespace Hypermedia.AspNetCore
{
    [DebuggerDisplay("Member={Member.Name} Children={Children.Count}")]
    public sealed class MemberPath
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="member">The member of the property.</param>
        /// <param name="children">The children properties for the nested path.</param>
        public MemberPath(IMember member, params MemberPath[] children)
        {
            Member = member;
            Children = children.ToList();
        }

        /// <summary>
        /// The member to return.
        /// </summary>
        public IMember Member { get; }

        /// <summary>
        /// The list of children members in the case that this is a nested relationship.
        /// </summary>
        public IReadOnlyList<MemberPath> Children { get; }
    }
}