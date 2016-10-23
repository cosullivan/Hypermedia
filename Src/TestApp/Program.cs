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
using Hypermedia.WebApi;
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
            var contractResolver = Hypermedia.Sample.WebApi.WebApiConfig.CreateResolver();

            IContract post;
            contractResolver.TryResolve(typeof(PostResource), out post);

            IContract comment;
            contractResolver.TryResolve(typeof(CommentResource), out comment);
            
            var includeMember = new MemberPath(
                post.Relationship(nameof(PostResource.Comments)),
                    new MemberPath(comment.Relationship(nameof(CommentResource.User))));

            MemberPath found;
            //MemberPathResolution options = new MemberPathResolution(resolver, post);

            //Console.WriteLine(MemberPath.TryParse("comments.user", options, out found));
            var resolver = new MemberPathResolver(contractResolver, post, "comments.user");
            Console.WriteLine(resolver.TryResolve(out found));
        }

        internal sealed class MemberPathResolver
        {
            readonly IContractResolver _contractResolver;
            readonly IContract _root;
            readonly string _path;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="contractResolver">The contract resolver to resolve with.</param>
            /// <param name="path">The path to parse from the root level.</param>
            internal MemberPathResolver(IContractResolver contractResolver, string path) : this(contractResolver, null, path) { }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="contractResolver">The contract resolver to resolve with.</param>
            /// <param name="root">The root level contact to parse from, or null if parsing a full path.</param>
            /// <param name="path">The path to parse from the root level.</param>
            internal MemberPathResolver(IContractResolver contractResolver, IContract root, string path)
            {
                _contractResolver = contractResolver;
                _root = root;
                _path = path;
            }

            /// <summary>
            /// Attempt to resolve the member path.
            /// </summary>
            /// <param name="memberPath">The member path to resolve.</param>
            /// <returns>true if the member path could be resolved, false if not.</returns>
            internal bool TryResolve(out MemberPath memberPath)
            {
                memberPath = null;

                return TryResolve(_root, _path, ref memberPath);

                //var root = _root;
                //var parts = _path.Split('.');

                //for (var i = 0; i < parts.Length; i++)
                //{
                //    var relationship = root.Relationship(parts[i]);

                //    if (relationship == null || _contractResolver.TryResolve(relationship.RelatedTo, out root) == false)
                //    {
                //        return false;
                //    }

                //    Console.WriteLine(parts[i]);
                //}

                //return false;
            }

            bool TryResolve(IContract root, string path, ref MemberPath memberPath)
            {
                var index = path.IndexOf(".", StringComparison.Ordinal);

                if (index < 0)
                {
                    return TryResolveRelationship(root, path, null, ref memberPath);
                }

                return TryResolveRelationship(root, path.Substring(0, index), path.Substring(index + 1), ref memberPath);

                //// can only match the field if there are not more parts remaining
                //if (index < 0 && TryResolveField(root, path, ref memberPath))
                //{
                //    return true;
                //}

                //return TryResolveRelationship(root, path.Substring(0, index), path.Substring(index + 1), ref memberPath);
            }

            bool TryResolveField(IContract root, string name, ref MemberPath memberPath)
            {
                var field = root.Field(name);

                if (field == null)
                {
                    //memberPath = null;
                    return false;
                }

                memberPath = new MemberPath(field);
                return true;
            }

            bool TryResolveRelationship(IContract root, string name, string path, ref MemberPath memberPath)
            {
                var relationship = root.Relationship(name);
                if (relationship == null)
                {
                    memberPath = null;
                    return false;
                }

                if (String.IsNullOrEmpty(path))
                {
                    memberPath = new MemberPath(relationship);
                    return true;
                }

                if (_contractResolver.TryResolve(relationship.RelatedTo, out root) == false)
                {
                    memberPath = null;
                    return false;
                }

                memberPath = new MemberPath(relationship);
                return TryResolve(root, path, ref memberPath);
            }
        }
    }
}