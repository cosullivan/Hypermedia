using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Hypermedia.Json;
using Hypermedia.Sample.Client;
using Hypermedia.Sample.Data;
using JsonLite;
using JsonLite.Ast;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //var posts = new[] { new Post() };

            //var serializer = new JsonSerializer();
            //////var json = serializer.SerializeValue(new object[] { 1, 2, 3, 4, 5, new { A = 1, B = new { C = "A", D = 234.45 } } });
            //var json = serializer.SerializeValue(posts);

            //Console.WriteLine(json.Stringify());

            //var array = serializer.DeserializeValue(typeof (List<Post>), Json.CreateAst(json.Stringify()));

            //const string Endpoint = "http://hypermedia.cainosullivan.com";
            const string Endpoint = "http://localhost:59074/";
            //using (var client = new HypermediaSampleClient(Endpoint, null))
            //{
            //    //foreach (var user in client.GetUsersAsync().Result)
            //    //{
            //    //    Console.WriteLine(user.DisplayName);
            //    //}

            //    //foreach (var post in client.GetPostsAsync(skip: 1, take: 10).Result)
            //    //{
            //    //    Console.WriteLine(post.Title);
            //    //    //Console.WriteLine(post.OwnerUser.DisplayName);

            //    //    //foreach (var comment in post.Comments)
            //    //    //{
            //    //    //    Console.WriteLine("[{0}] {1}", comment.User.DisplayName, comment.Text);
            //    //    //}

            //    //    //Console.WriteLine();
            //    //    //Console.WriteLine();
            //    //}
            //}
            using (var client = new HttpClient { BaseAddress = new Uri(Endpoint) })
            {
                var json = "{ \"Id\": 1, \"Text\": \"Hello World\", \"Score\": 123 }";
                Json.CreateAst(json);

                var response = client.PostAsync("v1/comments", new StringContent(json, Encoding.UTF8, "application/json")).Result;
                Console.WriteLine(response.StatusCode);
            }
        }
    }
}
