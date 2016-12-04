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
        // 1. SerializeAsEmbedded relationships
        // 2. BackingField field should only be available on BelongsTo - maybe need a new builder for this
        // 3. BackingField field could look at the parent object if it is not set (ie, default or null)
        // JsonApiSerializer DeserializeBelongsTo - can check the field Clr Type, if it is a related contract then it can serialize it
        // Need to be able to support this ".HasMany<PostResource>("posts")"
        // 4. Can i get rid of ClrType in favor of Accessor.ValueType?
        // 5. Get rid of IMember?

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