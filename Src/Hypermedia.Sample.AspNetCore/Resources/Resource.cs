using System;
using System.Collections.Generic;
using AutoMapper;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;

namespace Hypermedia.Sample.AspNetCore.Resources
{
    public static class Resource
    {
        static readonly IMapper Mapper;

        /// <summary>
        /// Constructor.
        /// </summary>
        static Resource()
        {
            var configuration = new MapperConfiguration(
                configure =>
                {
                    configure.CreateMap<User, UserResource>();
                    configure.CreateMap<Post, PostResource>();
                    configure.CreateMap<Comment, CommentResource>();
                });

            Mapper = configuration.CreateMapper();
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
    }
}