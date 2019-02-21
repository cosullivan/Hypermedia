using Hypermedia.Configuration;
using Hypermedia.JsonApi.Client;
using Hypermedia.Metadata;
using Hypermedia.Sample.Client;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Hypermedia.Json;
using Hypermedia.JsonApi;
using Hypermedia.Metadata.Runtime;
using JsonLite.Ast;
using PostResource = Hypermedia.Sample.Resources.PostResource;

namespace ConsoleApp
{
    class Program
    {
        class CustomContractResolver : IContractResolver
        {
            readonly IContractResolver _defaultResolver;
            
            readonly IContractResolver _unknownContractResolver = 
                new Builder()
                    .With<UnknownResource>(new ReflectionTypeDiscovery(FieldDiscovery.Deep))
                        .Id(nameof(UnknownResource.Id))
                .Build();

            public CustomContractResolver(IContractResolver defaultResolver)
            {
                _defaultResolver = defaultResolver;
            }

            public bool TryResolve(Type type, out IContract contract)
            {
                return _defaultResolver.TryResolve(type, out contract);
            }

            public bool TryResolve(string name, out IContract contract)
            {
                if (_defaultResolver.TryResolve(name, out contract))
                {
                    return true;
                }

                return _unknownContractResolver.TryResolve(typeof(UnknownResource), out contract);
            }
        }

        public class UnknownResource : Entity, IJsonExtension 
        {
            /// <summary>
            /// The extensible data for the instance.
            /// </summary>
            /// <remarks>When serializing, this information is added to the output. When deserializing, this information
            /// is added from members that don't deserialize to a property of the instance.</remarks>
            public ICollection<JsonMember> Data { get; set; }
        }

        static async Task Main(string[] args)
        {
            //using (var client = new HypermediaSampleClient("http://hypermediasamplewebapi.azurewebsites.net/", ""))
            using (var client = new HypermediaSampleClient("http://localhost:50419/", ""))
            {
                //client.BatchUpdateAsync(new [] { new CommentResource() }).Wait();
                //client.UpdateAsync(new CommentResource { Text = "Hello World!" }).Wait();
                client.CreateAsync(new CommentResource { Text = "Hello World!" }).Wait();

                //var posts = client.GetPostsAsync().Result;

                //foreach (var post in posts)
                //{
                //    Console.WriteLine(post.Title);
                //}
            }

            //var contractResolver = new Builder()
            //    //.With<PostResource>("posts")
            //    //    .Id(nameof(PostResource.Id))
            //    .Build();

            //using (var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:50419") })
            //{
            //    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.api+json"));

            //    var response = await httpClient.GetAsync($"v1/posts?skip=0&take=1");
            //    response.EnsureSuccessStatusCode();

            //    var serializerOptions = new JsonApiSerializerOptions(contractResolver)
            //    {
            //        MissingContractHandler = context =>
            //        {
            //            Console.WriteLine("The resource '{0}' is missing", context.Type);
            //            return null;
            //        }
            //    };

            //    //var entities = await response.Content.ReadAsJsonApiManyAsync<Entity>(new CustomContractResolver(contractResolver));
            //    var entities = await response.Content.ReadAsJsonApiManyAsync<Entity>(serializerOptions);

            //    foreach (var entity in entities)
            //    {
            //        Console.WriteLine(entity.Id);
            //        Console.WriteLine(entity.CreationDate);

            //        foreach (var jsonMember in ((IJsonExtension) entity).Data)
            //        {
            //            Console.WriteLine("{0} = {1}", jsonMember.Name, jsonMember.Value.Stringify());
            //        }
            //    }
            //}
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

            builder.With<CommentResource>("comments")
                .Id(nameof(CommentResource.Id))
                .BelongsTo<UserResource>(nameof(CommentResource.User))
                    .BackingField(nameof(CommentResource.UserId))
                    .Template("/v1/users/{id}", "id", resource => resource.UserId)
                .BelongsTo<PostResource>(nameof(CommentResource.Post))
                    .BackingField(nameof(CommentResource.PostId))
                    .Template("/v1/posts/{id}", "id", resource => resource.PostId);

            return builder.Build();
        }
    }
}
