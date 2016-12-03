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
        // TODO:
        // 1. Embedded relationships
        // 2. Via field should only be available on BelongsTo - maybe need a new builder for this
        // 3. Via field could look at the parent object if it is not set (ie, default or null)
        // JsonApiSerializer DeserializeBelongsTo - can check the field Clr Type, if it is a related contract then it can serialize it

        static void Main(string[] args)
        {
            const string Endpoint = "http://hypermedia.cainosullivan.com";
            //const string Endpoint = "http://localhost:59074/";
            using (var client = new HypermediaSampleClient(Endpoint, ""))
            {
                //var posts = client.GetPostsAsync().Result;
                //foreach (var post in posts)
                //{
                //    Console.WriteLine(post.ViewCount);
                //}

                var post = client.GetPostByIdAsync(38).Result;
                Console.WriteLine(post.ViewCount);
            }
        }
    }
}