using System;
using Hypermedia.Configuration;
using Hypermedia.Metadata;
using Hypermedia.Sample.Resources;
using Hypermedia.Sample.WebApi;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var resolver = CreateResolver();

            resolver.TryResolve(typeof(PostResource), out IContract contract);
            var relationship = contract.Relationship(nameof(PostResource.OwnerUser));

            Console.WriteLine("{0} => {1}", relationship.Name, relationship.Inverse(resolver).Name);
        }

        /// <summary>
        /// Creates an instance of the resource contract resolver for the configured model.
        /// </summary>
        /// <returns>The resource contract resolver for the configured model.</returns>
        public static IContractResolver CreateResolver()
        {
            var builder = new Builder();

            builder.With<UserResource>("users")
                .Id(nameof(UserResource.Id))
                .HasMany<PostResource>(nameof(UserResource.Posts))
                    .Inverse(nameof(PostResource.OwnerUser))
                    .Template("/v1/users/{id}/posts", "id", resource => resource.Id);

            builder.With<PostResource>("posts")
                .Id(nameof(PostResource.Id))
                .BelongsTo<UserResource>(nameof(PostResource.OwnerUser))
                    .BackingField(nameof(PostResource.OwnerUserId))
                    .Inverse(nameof(UserResource.Posts))
                    .Template("/v1/users/{id}", "id", resource => resource.OwnerUserId)
                //.BelongsTo<UserResource>(nameof(PostResource.ApproverUser), resource => resource.ApproverId.HasValue)
                //    .BackingField(nameof(PostResource.ApproverId))
                //    .Template("/v1/users/{id}", "id", resource => resource.ApproverId)
                //.HasMany<CommentResource>(nameof(PostResource.Comments))
                //    //.Embedded()
                //    .Template("/v1/posts/{id}/comments", "id", resource => resource.Id)
                    ;

            //builder.With<CommentResource>("comments")
            //    .Id(nameof(CommentResource.Id))
            //    .BelongsTo<UserResource>(nameof(CommentResource.User))
            //    .BackingField(nameof(CommentResource.UserId))
            //    .Template("/v1/users/{id}", "id", resource => resource.UserId)
            //    .BelongsTo<PostResource>(nameof(CommentResource.Post))
            //    .BackingField(nameof(CommentResource.PostId))
            //    .Template("/v1/posts/{id}", "id", resource => resource.PostId);

            //TODO: maybe at the point in which it is built is the best place to link the inversions?

            return builder.Build();
        }
    }
}