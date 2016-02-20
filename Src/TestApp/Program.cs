using System;
using Hypermedia.Sample.Client;
using JsonLite.Ast;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO: JsonLite Pretify option
            //TODO: Hypermedia.Sample.Client
            //TODO: NuGet packages
            
            using (var client = new HypermediaSampleClient("http://hypermedia.cainosullivan.com", null))
            {
                //foreach (var user in client.GetUsersAsync().Result)
                //{
                //    Console.WriteLine(user.DisplayName);
                //}

                var post = client.GetPostByIdAsync(1).Result;
                Console.WriteLine(post.OwnerUserId);
            }
        }
    }
}
