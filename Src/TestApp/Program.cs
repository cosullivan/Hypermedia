using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Hypermedia.Metadata;
using Hypermedia.Sample.Resources;
using Hypermedia.WebApi;
using JsonLite;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //const string Endpoint = "http://hypermedia.cainosullivan.com";
            const string Endpoint = "http://localhost:59074/";
            using (var client = new HttpClient { BaseAddress = new Uri(Endpoint) })
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.api+json"));

                var response = client.GetAsync("v1/posts").Result;
                Console.WriteLine(response.StatusCode);
            }
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