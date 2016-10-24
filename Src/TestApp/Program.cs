using System;
using System.Collections.Generic;
using Hypermedia.Metadata;
using Hypermedia.Sample.Resources;
using Hypermedia.WebApi;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var contractResolver = Hypermedia.Sample.WebApi.WebApiConfig.CreateResolver();

            IContract post;
            contractResolver.TryResolve(typeof(PostResource), out post);

            IReadOnlyList<MemberPath> memberPaths;
            MemberPath.TryParse(contractResolver, post, "comments.user,owneruser,comments.post.owneruser", out memberPaths);

            Dump(memberPaths, 0);
        }

        static void Dump(IEnumerable<MemberPath> memberPaths, int depth)
        {
            foreach (var memberPath in memberPaths)
            {
                Dump(memberPath, depth);
            }
        }

        static void Dump(MemberPath memberPath, int depth)
        {
            Console.Write(String.Empty.PadLeft(depth * 2));
            Console.WriteLine(memberPath.Member.Name);

            Dump(memberPath.Children, depth + 1);
        }
    }
}