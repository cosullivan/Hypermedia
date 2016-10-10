using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Hypermedia.Json;
using Hypermedia.JsonApi.WebApi;
using Hypermedia.Metadata;
using Hypermedia.Metadata.Runtime;
using Hypermedia.Sample.Client;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using JsonLite;
using JsonLite.Ast;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //const string Endpoint = "http://hypermedia.cainosullivan.com";
            //const string Endpoint = "http://localhost:59074/";
            //using (var client = new HttpClient { BaseAddress = new Uri(Endpoint) })
            //{
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.api+json"));

            //    //var json = "{ \"Id\": 1, \"Text\": \"Hello World\", \"Score\": 123 }";
            //    var json = "{ \"data\": { \"type\": \"comments\", \"attributes\": { \"Text\": \"Hello World\", \"Score\": 123 } } }";
            //    Json.CreateAst(json);

            //    var response = client.PostAsync("v1/comments", new StringContent(json, Encoding.UTF8, "application/vnd.api+json")).Result;
            //    Console.WriteLine(response.StatusCode);
            //}

            var include = "owner-user,comments.user";
            var resolver = Hypermedia.Sample.WebApi.WebApiConfig.CreateResolver();

            
            //request.Contract = new DynamicContract<Post>();

            
        }

        class IncludePath
        {
            public IRelationship Relationship { get; set; }
        }
    }
}