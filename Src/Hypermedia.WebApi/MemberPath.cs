using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Hypermedia.Metadata;

namespace Hypermedia.WebApi
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
        /// Attempt to parse the member path.
        /// </summary>
        /// <param name="contractResolver">The contract resolver to use when performing resolution of members.</param>
        /// <param name="root">The root level contract to parse from.</param>
        /// <param name="path">The input string to parse the member path from.</param>
        /// <param name="memberPath">The member path that was found.</param>
        /// <returns>true if the input could be parsed, false if not.</returns>
        public static bool TryParse(IContractResolver contractResolver, IContract root, string path, out MemberPath memberPath)
        {
            var resolver = new MemberPathResolver(contractResolver, root, path);

            return resolver.TryResolve(out memberPath);
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