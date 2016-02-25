using System;
using System.Collections.Generic;
using System.Linq;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;

namespace Hypermedia.Sample.WebApi.Resources
{
    public static class PostResourceExtensions
    {
        /// <summary>
        /// Populate the related resource properties.
        /// </summary>
        /// <param name="resource">The list of resources to populate.</param>
        /// <param name="database">The database instance to use when populating.</param>
        internal static PostResource Populate(this PostResource resource, IDatabase database)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            return Populate(new[] { resource }, database).Single();
        }

        /// <summary>
        /// Populate the related resource properties.
        /// </summary>
        /// <param name="resources">The list of resources to populate.</param>
        /// <param name="database">The database instance to use when populating.</param>
        internal static IReadOnlyList<PostResource> Populate(this IReadOnlyList<PostResource> resources, IDatabase database)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            PopulateOwnerUser(resources, database);
            PopulateComments(resources, database);

            return resources;
        }

        /// <summary>
        /// Populate the OwnerUser property.
        /// </summary>
        /// <param name="resources">The list of resources to populate.</param>
        /// <param name="database">The database instance to use when populating.</param>
        public static void PopulateOwnerUser(IReadOnlyList<PostResource> resources, IDatabase database)
        {
            var dictionary = database
                .Users
                    .GetById(resources.SelectDistinctList(post => post.OwnerUserId))
                .AsResource()
                .ToDictionary();

            foreach (var resource in resources)
            {
                UserResource user;
                if (dictionary.TryGetValue(resource.OwnerUserId, out user))
                {
                    resource.OwnerUser = user;
                }
            }
        }

        /// <summary>
        /// Populate the OwnerUser property.
        /// </summary>
        /// <param name="resources">The list of resources to populate.</param>
        /// <param name="database">The database instance to use when populating.</param>
        public static void PopulateComments(IReadOnlyList<PostResource> resources, IDatabase database)
        {
            var comments = database.Comments.GetByPostId(resources.SelectDistinctList(post => post.Id)).AsResource();
            CommentResourceExtensions.PopulateUser(comments, database);

            var lookup = comments.ToLookup(k => k.PostId);

            foreach (var resource in resources)
            {
                resource.Comments = lookup[resource.Id].AsResource().ToList();

                foreach (var comment in resource.Comments)
                {
                    comment.Post = resource;
                }
            }
        }
    }
}