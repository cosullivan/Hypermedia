# Hypermedia
A .NET Client and Server foundational Hypermedia library for .NET. Currently, there is only support 
for the [**JSON API**](http://jsonapi.org) format, but the foundation has been designed with extensibility 
in mind.

# Quick Start

Start with some domain/POCO objects. The Hypermedia model sits externally to the domain objects.

```cs
public class User
{
    public int Id { get; set; }
    public string DisplayName { get; set; }
    public int Reputation { get; set; }
    public string ProfileImageUrl { get; set; }
    public DateTimeOffset CreationDate { get; set; }
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public int Score { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public int OwnerUserId { get; set; }
    public User OwnerUser { get; set; }
    public IReadOnlyList<Comment> Comments { get; set; }
}

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
}

```

Define your model using the Fluent builder interface.
```cs
static IResourceContractResolver CreateResolver()
{
    return new Builder()
        .With<User>("users")
            .Id(nameof(User.Id))
            .Ignore(nameof(User.PasswordHash)
            .HasMany<Post>("posts")
                .Template("/v1/users/{id}/posts", "id", resource => resource.Id)
        .With<Post>("posts")
            .Id(nameof(Post.Id))
            .BelongsTo<User>(nameof(Post.OwnerUser))
                .Via(nameof(Post.OwnerUserId))
                .Template("/v1/users/{id}", "id", resource => resource.OwnerUserId)
            .HasMany<Comment>(nameof(Post.Comments))
                .Template("/v1/posts/{id}/comments", "id", resource => resource.Id)
        .With<Comment>("comments")
            .Id(nameof(Comment.Id))
            .BelongsTo<User>(nameof(Comment.User))
                .Via(nameof(Comment.UserId))
                .Template("/v1/users/{id}", "id", resource => resource.UserId)
            .BelongsTo<Post>(nameof(Comment.Post))
                .Via(nameof(Comment.PostId))
                .Template("/v1/posts/{id}", "id", resource => resource.PostId)
        .Build();
}
```

Register the JSON API Media Type formatter in your Web API project.

```cs
static void ConfigureFormatters(HttpConfiguration configuration)
{
    configuration.Formatters.Remove(configuration.Formatters.XmlFormatter);
    configuration.Formatters.Remove(configuration.Formatters.JsonFormatter);

    configuration.Formatters.Add(new JsonApiMediaTypeFormatter(CreateResolver()));
}
```

## Client Extensions
The Hypermedia.JsonApi.Client package provides some extension methods to GET and POST/PUT/PATCH to and from 
JSON API compliant backends. 

### Returning a Single Resource
```cs
var response = await _httpClient.GetAsync($"v1/users/{id}", cancellationToken);
response.EnsureSuccessStatusCode();

return await response.Content.ReadAsJsonApiAsync<UserResource>(CreateResolver());
```

### Returning a List of Resources
```cs
var response = await _httpClient.GetAsync($"v1/posts?skip={skip}&take={take}", cancellationToken);
response.EnsureSuccessStatusCode();

return await response.Content.ReadAsJsonApiManyAsync<PostResource>(_resourceContractResolver);
```
# Samples
View the Hypermedia.Sample.WebApi project for an example of how to use the Hypermedia library. The sample API is running
live at http://hypermedia.cainosullivan.com/ and has the following endpoints;

* http://hypermedia.cainosullivan.com/v1/users
* http://hypermedia.cainosullivan.com/v1/posts
* http://hypermedia.cainosullivan.com/v1/comments

Try it out now.

http://hypermedia.cainosullivan.com/v1/posts/1?$prettify=true

```json
{
  "data": {
    "type": "posts", 
    "id": 1, 
    "attributes": {
      "post-type-id": 1, 
      "title": "Where can I find the earliest literary compendium for the Greek Pantheon?", 
      "body": "<p>I know that much of Greek mythology was passed on by word of mouth but someone must have been the first to collect this in one source.  What was that Compendium and is there a version available in modern English?</p>\n", 
      "score": 13, 
      "view-count": 79, 
      "answer-count": 0, 
      "comment-count": 0, 
      "favorite-count": 0, 
      "creation-date": "2015-04-28T15:51:47Z"
    }, 
    "relationships": {
      "owner-user": {
        "links": {
          "related": "/v1/users/9"
        }, 
        "data": {
          "type": "users", 
          "id": 9
        }
      }, 
      "comments": {
        "links": {
          "related": "/v1/posts/1/comments"
        }, 
        "data": []
      }
    }
  }, 
  "included": [{
    "type": "users", 
    "id": 9, 
    "attributes": {
      "display-name": "Chad", 
      "reputation": 246, 
      "up-votes": 8, 
      "down-votes": 6, 
      "profile-image-url": "https://www.gravatar.com/avatar/c7f4863aa0893199727418907a27fa52?s=128&d=identicon&r=PG&f=1", 
      "creation-date": "2015-04-28T15:46:16Z"
    }, 
    "relationships": {
      "posts": {
        "links": {
          "related": "/v1/users/9/posts"
        }
      }
    }
  }]
}
```