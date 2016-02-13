using System;
using System.Collections.Generic;
using Hypermedia.JsonApi;
using JsonLite.Ast;

namespace TestApp
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Position Position { get; set; }
        public Guid AuthorId { get; set; }
        public List<Comment> Comments { get; set; }
        public int? Rating { get; set; }
    }

    public class Author
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
    }

    public class Comment
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Post Post { get; set; }
        public Author Author { get; set; }
        public string Description { get; set; }
    }

    public class Address
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public Region Region { get; set; }
    }

    public class Region
    {
        public string State;
        public string Country;
    }

    [JsonConverter(typeof(PositionConverter))]
    public struct Position
    {
        readonly double _lat;
        readonly double _lon;

        public Position(double lat, double lon)
        {
            _lat = lat;
            _lon = lon;
        }

        public double Lat { get { return _lat; } }
        public double Lon { get { return _lon; } }
    }

    public class PositionConverter : IJsonConverter
    {
        public JsonValue Serialize(object value)
        {
            var position = (Position)value;

            return new JsonObject(
                new JsonMember(
                    "lat", 
                    new JsonDecimal((decimal)position.Lat)),
                new JsonMember(
                    "lon",
                    new JsonDecimal((decimal)position.Lon)));
        }

        public object Deserialize(Type type, JsonValue jsonValue)
        {
            var jsonObject = (JsonObject)jsonValue;

            var lat = Decimal.ToDouble(((JsonDecimal)jsonObject["lat"]).Value);
            var lon = Decimal.ToDouble(((JsonDecimal)jsonObject["lon"]).Value);

            return new Position(lat, lon);
        }
    }

    public static class Repository
    {
        public static readonly List<Author> Authors = new List<Author>
        {
            new Author { Id = new Guid("f266e388-a4b9-484d-97b1-d80f5bd35e91"), Email = "cain.osullivan@1.com" },
            new Author { Id = new Guid("30f53045-acd6-4d48-b4ea-3f70c8e62f06"), Email = "cain.osullivan@2.com" },
            new Author { Id = new Guid("87b19456-6549-494b-9135-d1861b672809"), Email = "cain.osullivan@3.com" },
            new Author { Id = new Guid("60c9b6ee-1077-4bad-901a-e85a9a00d4a6"), Email = "cain.osullivan@4.com" },
            new Author { Id = new Guid("91cda2d1-b34f-4457-8e5e-7d9c7fe7be44"), Email = "cain.osullivan@5.com" }
        };

        public static readonly List<Post> Posts = new List<Post>
        {
            new Post { Id = new Guid("76138ee2-b506-43a4-8158-aaf73f6dbefd"), Title = "Test 1", AuthorId = Authors[0].Id, Position = new Position(1, 2) },
            new Post { Id = new Guid("c1d6ceca-821e-4028-bf6a-c23b5aefb775"), Title = "Test 2", AuthorId = Authors[1].Id, Position = new Position(2, 3) },
            new Post { Id = new Guid("dc7fc0f8-933d-4541-b89d-276238fa0de2"), Title = "Test 3", AuthorId = Authors[2].Id, Position = new Position(3, 4) }
        };

        public static readonly List<Comment> Comments = new List<Comment>
        {
            new Comment { Id = new Guid("76138ee2-b506-43a4-8158-aaf73f6dbefd"), Description = "Comment 1", PostId = Posts[0].Id, Post = Posts[0], Author = Authors[0] },
            new Comment { Id = new Guid("c1d6ceca-821e-4028-bf6a-c23b5aefb775"), Description = "Comment 2", PostId = Posts[1].Id, Post = Posts[1], Author = Authors[0] },
            new Comment { Id = new Guid("dc7fc0f8-933d-4541-b89d-276238fa0de2"), Description = "Comment 3", PostId = Posts[2].Id, Post = Posts[2], Author = Authors[0] }
        };
    }
}
