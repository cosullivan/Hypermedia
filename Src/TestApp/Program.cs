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

            const string Endpoint = "http://hypermedia.cainosullivan.com";
            //const string Endpoint = "http://localhost:59074/";
            using (var client = new HypermediaSampleClient(Endpoint, null))
            {
                //foreach (var user in client.GetUsersAsync().Result)
                //{
                //    Console.WriteLine(user.DisplayName);
                //}

                foreach (var post in client.GetPostsAsync().Result)
                {
                    Console.WriteLine(post);
                    Console.WriteLine(post.OwnerUser.DisplayName);
                }

                //var post = client.GetPostByIdAsync(38).Result;
                //Console.WriteLine(post.OwnerUserId);
                //Console.WriteLine(post.OwnerUser);
                //Console.WriteLine(post.Comments);

                //foreach (var comment in post.Comments)
                //{
                //    Console.WriteLine(comment.Text);
                //}

                //foreach (var comment in client.GetCommentsByPostIdAsync(38).Result)
                //{
                //    Console.WriteLine();
                //    Console.WriteLine(comment.UserId);
                //    Console.WriteLine(comment.Text);
                //}
            }
        }
    }
}
