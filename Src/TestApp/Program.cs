using System;
using Hypermedia.Sample.Client;
using JsonLite.Ast;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            const string Endpoint = "http://hypermedia.cainosullivan.com";
            //const string Endpoint = "http://localhost:59074/";
            using (var client = new HypermediaSampleClient(Endpoint, null))
            {
                //foreach (var user in client.GetUsersAsync().Result)
                //{
                //    Console.WriteLine(user.DisplayName);
                //}

                foreach (var post in client.GetPostsAsync(skip:1, take:10).Result)
                {
                    Console.WriteLine(post.PostType);
                    //Console.WriteLine(post.OwnerUser.DisplayName);

                    //foreach (var comment in post.Comments)
                    //{
                    //    Console.WriteLine("[{0}] {1}", comment.User.DisplayName, comment.Text);
                    //}

                    //Console.WriteLine();
                    //Console.WriteLine();
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
