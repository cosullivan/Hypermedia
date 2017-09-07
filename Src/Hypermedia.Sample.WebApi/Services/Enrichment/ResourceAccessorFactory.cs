using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using Hypermedia.Metadata;

namespace Hypermedia.Sample.WebApi.Services.Enrichment
{
    public sealed class ResourceAccessorFactory : IResourceAccessorFactory
    {
        readonly ConcurrentDictionary<IRelationship, object> _cache = new ConcurrentDictionary<IRelationship, object>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contract resolver.</param>
        public ResourceAccessorFactory(IContractResolver contractResolver)
        {
            ContractResolver = contractResolver;
        }

        /// <summary>
        /// Get or create an instance of a belongs to resource accessor.
        /// </summary>
        /// <typeparam name="TSource">The type of the source resource to enrich.</typeparam>
        /// <typeparam name="TDestination">The type of the destination resource that is enriched on the source.</typeparam>
        /// <param name="relationship">The relationship to create the accessor for.</param>
        /// <returns>The belongs to resource accessor.</returns>
        public IBelongsToResourceAccessor<TSource, TDestination> GetOrCreateBelongsToAccessor<TSource, TDestination>(IBelongsToRelationship relationship)
        {
            return (IBelongsToResourceAccessor<TSource, TDestination>) _cache.GetOrAdd(relationship, r => CreateBelongsToAccessor<TSource, TDestination>((IBelongsToRelationship)r));
        }

        /// <summary>
        /// Get or create an instance of a belongs to resource accessor.
        /// </summary>
        /// <typeparam name="TSource">The type of the source resource to enrich.</typeparam>
        /// <typeparam name="TDestination">The type of the destination resource that is enriched on the source.</typeparam>
        /// <param name="relationship">The relationship to create the accessor for.</param>
        /// <returns>The belongs to resource accessor.</returns>
        IBelongsToResourceAccessor<TSource, TDestination> CreateBelongsToAccessor<TSource, TDestination>(IBelongsToRelationship relationship)
        {
            var sourceParameter = Expression.Parameter(typeof(TSource));
            var destinationParameter = Expression.Parameter(typeof(TDestination));

            return new DelegatingBelongsToResourceAccessor<TSource, TDestination>(
                CreateBelongsToAccessorExpression<TSource>(relationship, sourceParameter).Compile(),
                Expression
                    .Lambda<Action<TSource, TDestination>>(
                        Expression.Assign(
                            Expression.PropertyOrField(
                                sourceParameter, 
                                relationship.Name),
                            destinationParameter),
                        sourceParameter,
                        destinationParameter)
                    .Compile());
        }

        /// <summary>
        /// Create the belongs to getter expression.
        /// </summary>
        /// <typeparam name="TSource">The type of the source resource to enrich.</typeparam>
        /// <param name="relationship">The relationship to create the accessor for.</param>
        /// <param name="sourceParameter">The source parameter for the expression.</param>
        /// <returns>The expression that returns the getter for the relationship.</returns>
        static Expression<Func<TSource, int?>> CreateBelongsToAccessorExpression<TSource>(IBelongsToRelationship relationship, ParameterExpression sourceParameter)
        {
            if (relationship.BackingField.Accessor.ValueType == typeof(int?))
            {
                return Expression
                    .Lambda<Func<TSource, int?>>(
                        Expression.PropertyOrField(
                            sourceParameter,
                            relationship.BackingField.Name),
                        sourceParameter);
            }

            return Expression
                .Lambda<Func<TSource, int?>>(
                    Expression.Convert(
                        Expression.PropertyOrField(
                            sourceParameter,
                            relationship.BackingField.Name),
                        typeof(int?)),
                    sourceParameter);
        }

        /// <summary>
        /// Get or create an instance of a has many resource accessor.
        /// </summary>
        /// <typeparam name="TSource">The type of the source resource to enrich.</typeparam>
        /// <typeparam name="TDestination">The type of the destination resource that is enriched on the source.</typeparam>
        /// <param name="relationship">The relationship to create the accessor for.</param>
        /// <returns>The has many resource accessor.</returns>
        public IHasManyResourceAccessor<TSource, TDestination> GetOrCreateHasManyAccessor<TSource, TDestination>(IHasManyRelationship relationship)
        {
            return (IHasManyResourceAccessor<TSource, TDestination>)_cache.GetOrAdd(relationship, r => CreateHasManyAccessor<TSource, TDestination>((IHasManyRelationship)r));
        }

        /// <summary>
        /// Get or create an instance of a has many resource accessor.
        /// </summary>
        /// <typeparam name="TSource">The type of the source resource to enrich.</typeparam>
        /// <typeparam name="TDestination">The type of the destination resource that is enriched on the source.</typeparam>
        /// <param name="relationship">The relationship to create the accessor for.</param>
        /// <returns>The has many resource accessor.</returns>
        IHasManyResourceAccessor<TSource, TDestination> CreateHasManyAccessor<TSource, TDestination>(IHasManyRelationship relationship)
        {
            var sourceParameter = Expression.Parameter(typeof(TSource));
            var destinationParameter = Expression.Parameter(typeof(TDestination));
            var destinationEnumerableParameter = Expression.Parameter(typeof(IEnumerable<TDestination>));

            var constructor = typeof(List<TDestination>).GetConstructor(new[] { typeof(IEnumerable<TDestination>) });

            return new DelegatingHasManyResourceAccessor<TSource, TDestination>(
                Expression
                    .Lambda<Func<TDestination, int>>(
                        Expression.PropertyOrField(
                            destinationParameter,
                            GuessForeignKeyPropertyName(relationship)),
                        destinationParameter)
                    .Compile(),
                Expression
                    .Lambda<Action<TSource, IEnumerable<TDestination>>>(
                        Expression.Assign(
                            Expression.PropertyOrField(
                                sourceParameter,
                                relationship.Name),
                            Expression.Convert(
                                Expression.New(constructor, destinationEnumerableParameter),
                                typeof(IReadOnlyList<TDestination>))),
                        sourceParameter,
                        destinationEnumerableParameter)
                    .Compile());
        }

        /// <summary>
        /// Returns the name of the foreign key for the relationship.
        /// </summary>
        /// <param name="relationship">The relationship to guess the name of the foreign key from.</param>
        /// <returns>The name of the foreign key property.</returns>
        string GuessForeignKeyPropertyName(IHasManyRelationship relationship)
        {
            var inverse = relationship.Inverse(ContractResolver) as IBelongsToRelationship;

            return inverse?.BackingField?.Name;
        }

        /// <summary>
        /// The contract resolver.
        /// </summary>
        public IContractResolver ContractResolver { get; }
    }
}