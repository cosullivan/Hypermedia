using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
            //const string Endpoint = "http://hypermedia.cainosullivan.com";
            const string Endpoint = "http://localhost:59074/";
            using (var client = new HypermediaSampleClient(Endpoint, null))
            {
                //foreach (var user in client.GetUsersAsync().Result)
                //{
                //    Console.WriteLine(user.DisplayName);
                //}

                var posts1 = client.GetPostsAsync(skip: 1, take: 10).Result;
                var posts2 = client.GetPostsAsync(skip: 1, take: 10).Result;
                var owners = posts1.Union(posts2).Select(p => p.OwnerUser).Distinct();
                Console.WriteLine(owners.Count());
                //Console.WriteLine(posts.Select(p => p.OwnerUser).Distinct().Count());
                foreach (var post in posts2)
                {
                    Console.WriteLine(post.OwnerUser.Id);
                }
                //foreach (var post in client.GetPostsAsync(skip: 1, take: 10).Result)
                //{
                //    Console.WriteLine(post.Title);
                //    //Console.WriteLine(post.OwnerUser.DisplayName);

                //    //foreach (var comment in post.Comments)
                //    //{
                //    //    Console.WriteLine("[{0}] {1}", comment.User.DisplayName, comment.Text);
                //    //}

                //    //Console.WriteLine();
                //    //Console.WriteLine();
                //}
            }
        }
    }
}
