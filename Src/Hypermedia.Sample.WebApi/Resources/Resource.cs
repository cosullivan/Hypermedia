using System;
using System.Collections.Generic;
using AutoMapper;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;

namespace Hypermedia.Sample.WebApi.Resources
{
    public static class Resource
    {
        static readonly IMapper Mapper;

        /// <summary>
        /// Constructor.
        /// </summary>
        static Resource()
        {
            Mapper = new MapperConfiguration(InitializeMapping).CreateMapper();
        }

        /// <summary>
        /// Initialize the mappings.
        /// </summary>
        /// <param name="configuration">The configuration to use for initializing the mappings.</param>
        static void InitializeMapping(IMapperConfiguration configuration)
        {
            configuration.CreateMap<User, UserResource>();
            configuration.CreateMap<Post, PostResource>();
            configuration.CreateMap<Comment, CommentResource>();
        }

        /// <summary>
        /// Map an entity to a resource.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to map to.</typeparam>
        /// <typeparam name="TResource">The resource type to map to.</typeparam>
        /// <param name="entity">The entity to map to its resource.</param>
        /// <returns>The resource that was mapped from the entity.</returns>
        public static TResource Map<TEntity, TResource>(this TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return Mapper.Map<TEntity, TResource>(entity);
        }

        /// <summary>
        /// Map a list of entities to a resource.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to map to.</typeparam>
        /// <typeparam name="TResource">The resource type to map to.</typeparam>
        /// <param name="entities">The list if entities to map to their resource.</param>
        /// <returns>The list of resources that were mapped from the entities.</returns>
        public static IReadOnlyList<TResource> Map<TEntity, TResource>(this IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            return entities.SelectList(entity => Mapper.Map < TEntity, TResource>(entity));
        }

        /// <summary>
        /// Returns the user as its defined resource.
        /// </summary>
        /// <param name="user">The user to return as a resource.</param>
        /// <returns>The user resouce that was mapped from the user.</returns>
        public static UserResource AsResource(this User user)
        {
            if (user == null)
            {
                return null;
            }

            return Map<User, UserResource>(user);
        }

        /// <summary>
        /// Returns the list of users as resources.
        /// </summary>
        /// <param name="users">The list of users to return as resources.</param>
        /// <returns>The users resouces that were mapped from the users.</returns>
        public static IReadOnlyList<UserResource> AsResource(this IEnumerable<User> users)
        {
            if (users == null)
            {
                return null;
            }

            return Map<User, UserResource>(users);
        }

        /// <summary>
        /// Returns the post as its defined resource.
        /// </summary>
        /// <param name="post">The post to return as a resource.</param>
        /// <returns>The post resouce that was mapped from the post.</returns>
        public static PostResource AsResource(this Post post)
        {
            if (post == null)
            {
                return null;
            }

            return Map<Post, PostResource>(post);
        }

        /// <summary>
        /// Returns the list of post as resources.
        /// </summary>
        /// <param name="posts">The list of posts to return as resources.</param>
        /// <returns>The post resouces that were mapped from the posts.</returns>
        public static IReadOnlyList<PostResource> AsResource(this IEnumerable<Post> posts)
        {
            if (posts == null)
            {
                return null;
            }

            return Map<Post, PostResource>(posts);
        }

        /// <summary>
        /// Returns the comment as its defined resource.
        /// </summary>
        /// <param name="comment">The comment to return as a resource.</param>
        /// <returns>The comment resouce that was mapped from the comment.</returns>
        public static CommentResource AsResource(this Comment comment)
        {
            if (comment == null)
            {
                return null;
            }

            return Map<Comment, CommentResource>(comment);
        }

        /// <summary>
        /// Returns the list of comments as resources.
        /// </summary>
        /// <param name="comments">The list of comments to return as resources.</param>
        /// <returns>The comments resouces that were mapped from the comments.</returns>
        public static IReadOnlyList<CommentResource> AsResource(this IEnumerable<Comment> comments)
        {
            if (comments == null)
            {
                return null;
            }

            return Map<Comment, CommentResource>(comments);
        }
    }
}