using System;
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
        /// <param name="path">The path to parse the member paths from.</param>
        /// <param name="memberPaths">The list of member paths that were found.</param>
        /// <returns>true if the input could be parsed, false if not.</returns>
        public static bool TryParse(IContractResolver contractResolver, IContract root, string path, out IReadOnlyList<MemberPath> memberPaths)
        {
            if (TryResolve(contractResolver, root, path, out memberPaths) == false)
            {
                return false;
            }

            memberPaths = Combine(memberPaths).ToList();

            return true;
        }

        /// <summary>
        /// Attempt to resolve all paths.
        /// </summary>
        /// <param name="contractResolver">The contract resolver to use when performing resolution of members.</param>
        /// <param name="root">The root level contract to parse from.</param>
        /// <param name="path">The full path to resolve from.</param>
        /// <param name="memberPaths">The list of member paths that were resolved.</param>
        /// <returns>true if the path could be resolved, fasle if not.</returns>
        static bool TryResolve(IContractResolver contractResolver, IContract root, string path, out IReadOnlyList<MemberPath> memberPaths)
        {
            memberPaths = new List<MemberPath>();

            var resolver = new MemberPathResolver(contractResolver, root);

            foreach (var part in path.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                MemberPath memberPath;
                if (resolver.TryResolve(part.Trim(), out memberPath) == false)
                {
                    return false;
                }

                ((List<MemberPath>)memberPaths).Add(memberPath);
            }

            return true;
        }

        /// <summary>
        /// Combine all of the paths into a hierarchy.
        /// </summary>
        /// <param name="memberPaths">The list of member paths to combine.</param>
        /// <returns>The list of combined member paths.</returns>
        static IEnumerable<MemberPath> Combine(IEnumerable<MemberPath> memberPaths)
        {
            foreach (var group in memberPaths.GroupBy(m => m.Member))
            {
                yield return new MemberPath(group.Key, Combine(group.SelectMany(m => m.Children)).ToArray());
            }
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