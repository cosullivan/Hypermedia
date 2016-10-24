using System;
using Hypermedia.Metadata;

namespace Hypermedia.WebApi
{
    internal sealed class MemberPathResolver
    {
        readonly IContractResolver _contractResolver;
        readonly IContract _root;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contract resolver to resolve with.</param>
        /// <param name="root">The root level contact to parse from, or null if parsing a full path.</param>
        internal MemberPathResolver(IContractResolver contractResolver, IContract root)
        {
            _contractResolver = contractResolver;
            _root = root;
        }

        /// <summary>
        /// Attempt to resolve the member path.
        /// </summary>
        /// <param name="path">The path to resolve.</param>
        /// <param name="memberPath">The member path to resolve.</param>
        /// <returns>true if the member path could be resolved, false if not.</returns>
        internal bool TryResolve(string path, out MemberPath memberPath)
        {
            memberPath = null;

            return TryResolve(_root, path, ref memberPath);
        }

        /// <summary>
        /// Attempt to resolve a realtionship from the given path.
        /// </summary>
        /// <param name="root">The root level contract to resolve the relationship from.</param>
        /// <param name="path">The path for to resolve on the relationship.</param>
        /// <param name="memberPath">The path that was resolved, or undefined if no path could be found.</param>
        /// <returns>true if the path was resolved correctly, false if not.</returns>
        bool TryResolve(IContract root, string path, ref MemberPath memberPath)
        {
            var index = path.IndexOf(".", StringComparison.Ordinal);

            if (index < 0)
            {
                var relationship = root.Relationship(path);

                if (relationship == null)
                {
                    memberPath = null;
                    return false;
                }

                memberPath = new MemberPath(relationship);
                return true;
            }

            return TryResolveRelationship(root, path.Substring(0, index), path.Substring(index + 1), ref memberPath);
        }

        /// <summary>
        /// Attempt to resolve a realtionship from the given name and path.
        /// </summary>
        /// <param name="root">The root level contract to resolve the relationship from.</param>
        /// <param name="name">The name of the relationship to resolve.</param>
        /// <param name="path">The path for to resolve on the relationship.</param>
        /// <param name="memberPath">The path that was resolved, or undefined if no path could be found.</param>
        /// <returns>true if the path was resolved correctly, false if not.</returns>
        bool TryResolveRelationship(IContract root, string name, string path, ref MemberPath memberPath)
        {
            var relationship = root.Relationship(name);

            if (relationship == null)
            {
                memberPath = null;
                return false;
            }

            if (_contractResolver.TryResolve(relationship.RelatedTo, out root) == false)
            {
                memberPath = null;
                return false;
            }

            if (TryResolve(root, path, ref memberPath) == false)
            {
                return false;
            }

            memberPath = new MemberPath(relationship, memberPath);
            return true;
        }
    }
}