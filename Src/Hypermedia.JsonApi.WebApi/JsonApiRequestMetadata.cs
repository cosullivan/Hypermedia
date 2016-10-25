using System;
using System.Collections.Generic;
using System.Linq;
using Hypermedia.Metadata;
using Hypermedia.WebApi;

namespace Hypermedia.JsonApi.WebApi
{
    internal sealed class JsonApiRequestMetadata<TResource> : IRequestMetadata<TResource>
    {
        readonly Lazy<IReadOnlyList<MemberPath>> _include;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contract resolver instance to use for resolving the paths.</param>
        /// <param name="options">The options that the formatter was created with.</param>
        public JsonApiRequestMetadata(IContractResolver contractResolver, JsonApiMediaTypeFormatterOptions options)
        {
            ContractResolver = contractResolver;
            Options = options;

            _include = new Lazy<IReadOnlyList<MemberPath>>(InitializeInclude);
        }

        /// <summary>
        /// Initialize the included member path.
        /// </summary>
        /// <returns>The list of member paths that define the related items to include.</returns>
        IReadOnlyList<MemberPath> InitializeInclude()
        {
            IContract root;
            if (ContractResolver.TryResolve(typeof(TResource), out root) == false)
            {
                return new MemberPath[0];
            }

            List<MemberPath> memberPaths;
            if (TryResolveInclude(root, out memberPaths) == false)
            {
                return new MemberPath[0];
            }

            return Combine(memberPaths).ToList();
        }

        /// <summary>
        /// Attempt to resolve all paths from the include option.
        /// </summary>
        /// <param name="root">The root level contract to parse from.</param>
        /// <param name="memberPaths">The list of member paths that were resolved.</param>
        /// <returns>true if the path could be resolved, fasle if not.</returns>
        bool TryResolveInclude(IContract root, out List<MemberPath> memberPaths)
        {
            memberPaths = new List<MemberPath>();

            var resolver = new MemberPathResolver(ContractResolver, root);

            foreach (var part in Options.IncludePath.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                MemberPath memberPath;
                if (resolver.TryResolve(part.Trim(), out memberPath) == false)
                {
                    return false;
                }

                memberPaths.Add(memberPath);
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
        /// Gets the resource contract resolver that is to be used for resolving the paths.
        /// </summary>
        public IContractResolver ContractResolver { get; }

        /// <summary>
        /// The options that the formatter was created with.
        /// </summary>
        public JsonApiMediaTypeFormatterOptions Options { get; }

        /// <summary>
        /// The list of properties to include in the request.
        /// </summary>
        public IReadOnlyList<MemberPath> Include
        {
            get { return _include.Value; }
        }

        /// <summary>
        /// The list of members to sort the response by.
        /// </summary>
        public IReadOnlyList<MemberSortPath> Sort { get; }
    }
}