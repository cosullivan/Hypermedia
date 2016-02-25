using System;
using System.Collections.Generic;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;

namespace Hypermedia.Sample.WebApi.Resources
{
    public static class CommentResourceExtensions
    {
        /// <summary>
        /// Populate the related resource properties.
        /// </summary>
        /// <param name="resources">The list of resources to populate.</param>
        /// <param name="database">The database instance to use when populating.</param>
        internal static IReadOnlyList<CommentResource> Populate(this IReadOnlyList<CommentResource> resources, IDatabase database)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            PopulateUser(resources, database);

            return resources;
        }

        /// <summary>
        /// Populate the User property.
        /// </summary>
        /// <param name="resources">The list of resources to populate.</param>
        /// <param name="database">The database instance to use when populating.</param>
        public static void PopulateUser(IReadOnlyList<CommentResource> resources, IDatabase database)
        {
            var dictionary = database
                .Users
                    .GetById(resources.SelectDistinctList(post => post.UserId))
                .AsResource()
                .ToDictionary();

            foreach (var resource in resources)
            {
                UserResource user;
                if (dictionary.TryGetValue(resource.UserId, out user))
                {
                    resource.User = user;
                }
            }
        }
    }
}