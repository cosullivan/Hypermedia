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
            var json = (JsonObject)JsonLite.Json.CreateAst(System.IO.File.ReadAllText(@"C:\Temp\test.json"));
            Console.WriteLine(json.Members.Count);

            var array = (JsonArray) json.Members[0].Value;
            var data = new JsonObject(new JsonMember("data", new JsonArray(array.Take(500000).ToList())));

            var serializer = new JsonApiSerializer(CreateResolver());
            var timer = new Stopwatch();
            timer.Start();

            var entities = serializer.DeserializeMany(data).ToList();

            timer.Stop();

            Console.WriteLine("Count={0} Time Taken={1}ms", entities.Count, timer.ElapsedMilliseconds);


            //json = new JsonObject(json.Members[1]);

            //var resolver = CreateResolver();

            //var patch = new JsonApiPatch<PostResource>(resolver, new DasherizedFieldNamingStrategy(), json);
            //foreach (var member in patch.Members)
            //{
            //    Console.WriteLine(member.Name);
            //}

            //resolver.TryResolve(typeof(PostResource), out IContract contract);
            //var relationship = contract.Relationship(nameof(PostResource.OwnerUser));

            //Console.WriteLine("{0} => {1}", relationship.Name, relationship.Inverse(resolver).Name);

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