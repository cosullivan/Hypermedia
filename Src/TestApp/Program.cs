using System;
using System.Collections.Generic;
using System.Linq;
using Hypermedia.Configuration;
using Hypermedia.JsonApi;
using System.IO;
using System.Net.Http;
using Hypermedia.JsonApi.Client;
using Hypermedia.Sample.Data;
using JsonLite;
using JsonLite.Ast;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(JsonNull.Instance == JsonNull.Instance);

            //var userRepository = UserRepository.FromXml(@"C:\Temp\mythology.stackexchange.com\users.xml");

            //var resolver = new Builder()
            //    .With<Post>("posts")
            //        .Id(nameof(Post.Id))
            //        .HasMany<Comment>("comments")
            //            .Via(nameof(Post.Comments))
            //            .Template("/v1/api/posts/{id}/comments")
            //                .Parameter("id", post => post.Id)
            //        .BelongsTo<Author>("author")
            //            .Via(nameof(Post.AuthorId))
            //            .Template("/v1/api/authors/{authorId}", "authorId", post => post.AuthorId)
            //    .With<Comment>("comments")
            //        .Id(nameof(Comment.Id))
            //        .Field(nameof(Comment.Description))
            //        .BelongsTo<Author>("author")
            //            .Via(nameof(Comment.Author))
            //            .Template("/v1/api/authors/{authorId}", "authorId", comment => comment.Author.Id)
            //        .BelongsTo<Post>("post")
            //            .Via(nameof(Comment.Post))
            //            .Template("/v1/api/posts/{postId}", "postId", comment => comment.Post.Id)
            //    .With<Author>("authors")
            //        .Id(nameof(Author.Id))
            //        .Field(nameof(Author.Email))
            //    .Build();

            //var p = Repository.Posts[0];
            //p.Comments = new List<Comment>(Repository.Comments);

            //var serializer = new JsonApiSerializer(resolver);
            ////var json = serializer.SerializeEntity(p
            //Repository.Authors[0].Address = new Address { Region = new Region() };
            //var json = serializer.SerializeMany(Repository.Authors);
            //Console.WriteLine(json.Stringify());
            //File.WriteAllText(@"c:\temp\output.json", json.Stringify());

            //////var items = serializer.DeserializeMany(json);
            //////foreach (var item in items)
            //////{
            //////    Console.WriteLine(item);
            //////}

            ////var json = File.ReadAllText(@"C:\Dev\Hypermedia\Src\TestApp\sample1.json");
            ////var serializer = new JsonApiSerializer(types);

            ////var entity = (Post)serializer.DeserializeEntity((JsonObject)Json.CreateAst(json));
            ////Console.WriteLine(entity.Title);
            ////Console.WriteLine(entity.AuthorId);
        }
    }
}
