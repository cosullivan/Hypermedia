using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Hypermedia.Json;
using Hypermedia.Metadata;
using Hypermedia.Sample.Client;
using Hypermedia.Sample.Resources;
using Hypermedia.WebApi;
using JsonLite;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            const string Endpoint = "http://hypermedia.cainosullivan.com";
            //const string Endpoint = "http://localhost:59074/";
            using (var client = new HypermediaSampleClient(Endpoint, ""))
            {
                var posts = client.GetPostsAsync().Result;
                foreach (var post in posts)
                {
                    Console.WriteLine(post.ViewCount);
                }
            }
        }
    }
}