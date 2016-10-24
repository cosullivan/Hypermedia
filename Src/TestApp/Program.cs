using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Hypermedia.Json;
using Hypermedia.JsonApi.WebApi;
using Hypermedia.Metadata;
using Hypermedia.Metadata.Runtime;
using Hypermedia.Sample.Client;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using Hypermedia.WebApi;
using JsonLite;
using JsonLite.Ast;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //const string Endpoint = "http://hypermedia.cainosullivan.com";
            //const string Endpoint = "http://localhost:59074/";
            //using (var client = new HttpClient { BaseAddress = new Uri(Endpoint) })
            //{
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.api+json"));

            //    //var json = "{ \"Id\": 1, \"Text\": \"Hello World\", \"Score\": 123 }";
            //    var json = "{ \"data\": { \"type\": \"comments\", \"attributes\": { \"Text\": \"Hello World\", \"Score\": 123 } } }";
            //    Json.CreateAst(json);

            //    var response = client.PostAsync("v1/comments", new StringContent(json, Encoding.UTF8, "application/vnd.api+json")).Result;
            //    Console.WriteLine(response.StatusCode);
            //}

            var include = "owner-user,comments.user";
            var contractResolver = Hypermedia.Sample.WebApi.WebApiConfig.CreateResolver();

            IContract post;
            contractResolver.TryResolve(typeof(PostResource), out post);

            IContract comment;
            contractResolver.TryResolve(typeof(CommentResource), out comment);
            
            var includeMember = new MemberPath(
                post.Relationship(nameof(PostResource.Comments)),
                    new MemberPath(comment.Relationship(nameof(CommentResource.User))));

            MemberPath found;
            //MemberPathResolution options = new MemberPathResolution(resolver, post);

            //Console.WriteLine(MemberPath.TryParse("comments.user", options, out found));
            var resolver = new MemberPathResolver(contractResolver, post, "comments.user");
            Console.WriteLine(resolver.TryResolve(out found));
        }
    }
}