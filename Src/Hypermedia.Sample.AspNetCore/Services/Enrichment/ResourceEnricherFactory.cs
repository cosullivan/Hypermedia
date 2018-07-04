using System;
using System.Linq.Expressions;
using Hypermedia.Metadata;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.AspNetCore.Services.Enrichment
{
    public sealed class ResourceEnricherFactory : IResourceEnricherFactory
    {
        readonly IResourceAccessorFactory _resourceAccessorFactory;
        readonly IRepositoryAccessorFactory _repositoryAccessorFactory;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="resourceAccessorFactory">The resource accessor factory.</param>
        /// <param name="repositoryAccessorFactory">The repository accessor factory.</param>
        public ResourceEnricherFactory(
            IResourceAccessorFactory resourceAccessorFactory, 
            IRepositoryAccessorFactory repositoryAccessorFactory)
        {
            _resourceAccessorFactory = resourceAccessorFactory;
            _repositoryAccessorFactory = repositoryAccessorFactory;
        }

        /// <summary>
        /// Create the enricher for the given resource type.
        /// </summary>
        /// <typeparam name="TSource">The resource type to create the enricher for.</typeparam>
        /// <typeparam name="TDestination">The resource type on the other end of the relationship that is being enriched from.</typeparam>
        /// <param name="relationship">The relationship to create the enricher from.</param>
        /// <param name="database">The database to perform the enrichment within.</param>
        /// <returns>The enricher for the given resource and relationship, or null if no enricher could be provided.</returns>
        public IResourceEnricher<TSource, TDestination> CreateEnricher<TSource, TDestination>(IRelationship relationship, IDatabase database)
        {
            switch (relationship.Type)
            {
                case RelationshipType.BelongsTo:
                    return CreateBelongsToEnricher<TSource, TDestination>((IBelongsToRelationship)relationship, database);

                case RelationshipType.HasMany:
                    return CreateHasManyEnricher<TSource, TDestination>((IHasManyRelationship)relationship, database);
            }

            return null;
        }

        /// <summary>
        /// Create a belongs to enricher for the given relationship.
        /// </summary>
        /// <typeparam name="TSource">The resource type to create the enricher for.</typeparam>
        /// <typeparam name="TDestination">The resource type on the other end of the relationship that is being enriched from.</typeparam>
        /// <param name="relationship">The relationship to create the enricher from.</param>
        /// <param name="database">The database to perform the enrichment within.</param>
        /// <returns>The enricher for the given resource and relationship.</returns>
        IResourceEnricher<TSource, TDestination> CreateBelongsToEnricher<TSource, TDestination>(IBelongsToRelationship relationship, IDatabase database)
        {
            var parameters = new[]
            {
                Expression.Parameter(typeof(IBelongsToRelationship)),
                Expression.Parameter(typeof(IDatabase))
            };

            var methodCallExpression = Expression.Call(
                Expression.Constant(this), 
                nameof(CreateBelongsToEnricher), 
                new[]
                {
                    typeof(TSource),
                    relationship.RelatedTo.BaseType,
                    relationship.RelatedTo
                },
                // ReSharper disable once CoVariantArrayConversion
                parameters);

            var @delegate = Expression
                .Lambda<Func<IBelongsToRelationship, IDatabase, IResourceEnricher<TSource, TDestination>>>(
                    methodCallExpression,
                    parameters)
                .Compile();

            return @delegate(relationship, database);
        }

        /// <summary>
        /// Create a belongs to enricher for the given relationship.
        /// </summary>
        /// <param name="relationship">The relationship to create the enricher from.</param>
        /// <param name="database">The database to perform the enrichment within.</param>
        /// <returns>The enricher for the given resource and relationship.</returns>
        IResourceEnricher<TSource, TDestination> CreateBelongsToEnricher<TSource, TDestinationEntity, TDestination>(IBelongsToRelationship relationship, IDatabase database)
            where TDestinationEntity : IEntityWithId
            where TDestination : IEntityWithId
        {
            var foreignKeyAccessor = _resourceAccessorFactory.GetOrCreateBelongsToAccessor<TSource, TDestination>(relationship);
            var repositoryAccessorFactory = _repositoryAccessorFactory.GetOrCreateBelongsToAccessor<TDestinationEntity>(relationship);

            return new DelegatingBelongsToResourceEnricher<TSource, TDestinationEntity, TDestination>(
                foreignKeyAccessor, 
                repositoryAccessorFactory(database));
        }

        /// <summary>
        /// Create a has many enricher for the given relationship.
        /// </summary>
        /// <typeparam name="TSource">The resource type to create the enricher for.</typeparam>
        /// <typeparam name="TDestination">The resource type on the other end of the relationship that is being enriched from.</typeparam>
        /// <param name="relationship">The relationship to create the enricher from.</param>
        /// <param name="database">The database to perform the enrichment within.</param>
        /// <returns>The enricher for the given resource and relationship.</returns>
        IResourceEnricher<TSource, TDestination> CreateHasManyEnricher<TSource, TDestination>(IHasManyRelationship relationship, IDatabase database)
        {
            var parameters = new[]
            {
                Expression.Parameter(typeof(IHasManyRelationship)),
                Expression.Parameter(typeof(IDatabase))
            };

            var methodCallExpression = Expression.Call(
                Expression.Constant(this),
                nameof(CreateHasManyEnricher),
                new[]
                {
                    typeof(TSource),
                    relationship.RelatedTo.BaseType,
                    relationship.RelatedTo
                },
                // ReSharper disable once CoVariantArrayConversion
                parameters);

            var @delegate = Expression
                .Lambda<Func<IHasManyRelationship, IDatabase, IResourceEnricher<TSource, TDestination>>>(
                    methodCallExpression,
                    parameters)
                .Compile();

            return @delegate(relationship, database);
        }

        /// <summary>
        /// Create a belongs to enricher for the given relationship.
        /// </summary>
        /// <param name="relationship">The relationship to create the enricher from.</param>
        /// <param name="database">The database to perform the enrichment within.</param>
        /// <returns>The enricher for the given resource and relationship.</returns>
        IResourceEnricher<TSource, TDestination> CreateHasManyEnricher<TSource, TDestinationEntity, TDestination>(IHasManyRelationship relationship, IDatabase database)
            where TSource : IEntityWithId
            where TDestinationEntity : IEntityWithId
            where TDestination : IEntityWithId
        {
            var foreignKeyAccessor = _resourceAccessorFactory.GetOrCreateHasManyAccessor<TSource, TDestination>(relationship);
            var repositoryAccessorFactory = _repositoryAccessorFactory.GetOrCreateHasManyAccessor<TDestinationEntity>(relationship);

            return new DelegatingHasManyResourceEnricher<TSource, TDestinationEntity, TDestination>(
                foreignKeyAccessor, 
                repositoryAccessorFactory(database));
        }
    }
}