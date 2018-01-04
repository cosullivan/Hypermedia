using System;
using System.Diagnostics;
using System.Linq;
using Hypermedia.Configuration;
using Hypermedia.Json;
using Hypermedia.JsonApi;
using Hypermedia.JsonApi.WebApi;
using Hypermedia.Metadata;
using Hypermedia.Sample.Client;
using Hypermedia.Sample.Resources;
using Hypermedia.Sample.WebApi;
using JsonLite.Ast;

namespace TestApp
{
    class Program
    {
    	static void Main(string[] args)
        {
            //var timespan = TimeSpan.FromDays(1);

            //var x = TimeSpan.Parse(timespan.ToString());
            //Console.WriteLine(x);

            using (var client = new HypermediaSampleClient("http://hypermediasamplewebapi.azurewebsites.net/", ""))
            //using (var client = new HypermediaSampleClient("http://localhost:59074/", ""))
            {
                client.BatchUpdateAsync(new [] { new CommentResource() }).Wait();
            }
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