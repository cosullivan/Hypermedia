using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using Hypermedia.Metadata;
using Hypermedia.WebApi;

namespace Hypermedia.JsonApi.WebApi
{
    internal sealed class JsonApiRequestMetadata<TResource> : IRequestMetadata<TResource>
    {
        readonly IContractResolver _contractResolver;
        readonly IContract _root;
        readonly NameValueCollection _parameters;
        readonly Lazy<IReadOnlyList<MemberPath>> _include;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contract resolver instance to use for resolving the paths.</param>
        /// <param name="root">The root level contract that the request is referring to.</param>
        /// <param name="request">The HTTP request message.</param>
        public JsonApiRequestMetadata(IContractResolver contractResolver, IContract root, HttpRequestMessage request)
        {
            _contractResolver = contractResolver;
            _root = root;
            _parameters = request.RequestUri.ParseQueryString();

            _include = new Lazy<IReadOnlyList<MemberPath>>(CreateInclusionPaths);
        }

        /// <summary>
        /// Initialize the included member path.
        /// </summary>
        /// <returns>The list of member paths that define the related items to include.</returns>
        IReadOnlyList<MemberPath> CreateInclusionPaths()
        {
            if (String.IsNullOrWhiteSpace(_parameters["include"]))
            {
                return new MemberPath[0];
            }

            if (TryResolveInclude(_parameters["include"], out List<MemberPath> memberPaths) == false)
            {
                return new MemberPath[0];
            }

            return Combine(memberPaths).ToList();
        }

        /// <summary>
        /// Attempt to resolve all paths from the include option.
        /// </summary>
        /// <param name="path">The fully qualified inclusion path.</param>
        /// <param name="memberPaths">The list of member paths that were resolved.</param>
        /// <returns>true if the path could be resolved, fasle if not.</returns>
        bool TryResolveInclude(string path, out List<MemberPath> memberPaths)
        {
            memberPaths = new List<MemberPath>();

            var resolver = new MemberPathResolver(_contractResolver, _root);

            foreach (var part in path.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (resolver.TryResolve(part.Trim(), out MemberPath memberPath) == false)
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
        /// The list of properties to include in the request.
        /// </summary>
        public IReadOnlyList<MemberPath> Include => _include.Value;

        /// <summary>
        /// The list of members to sort the response by.
        /// </summary>
        public IReadOnlyList<MemberSortPath> Sort { get; }
    }
}