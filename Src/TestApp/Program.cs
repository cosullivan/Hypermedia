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

            /*

            The relationships can be returned under a number of conditions
                1) the following applies to BelongsTo
                    a) via the template using the links node
                    b) by the actual ID key, this can be returned from the Via(...) or if the actual relationship exists as a field
                    c) the relationship can only be included in the case that the relationship is also a field

                2) the following applies to HasMany
                    a) via the template using the links node
                    b) the relationship and data can be included in the link
                    
            */
            
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
