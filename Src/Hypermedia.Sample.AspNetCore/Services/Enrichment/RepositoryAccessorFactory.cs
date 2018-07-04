using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Hypermedia.Metadata;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.AspNetCore.Services.Enrichment
{
    public sealed class RepositoryAccessorFactory : IRepositoryAccessorFactory
    {
        readonly ConcurrentDictionary<IRelationship, object> _cache = new ConcurrentDictionary<IRelationship, object>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contract resolver.</param>
        public RepositoryAccessorFactory(IContractResolver contractResolver)
        {
            ContractResolver = contractResolver;
        }

        /// <summary>
        /// Returns an instance of a resource repository accessor for a Belongs To relationship.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to return.</typeparam>
        /// <param name="relationship">The relationship that defines the accessor to return.</param>
        /// <returns>The resource repository accessor instance.</returns>
        public Func<IDatabase, IRepositoryAccessor<TEntity>> GetOrCreateBelongsToAccessor<TEntity>(IBelongsToRelationship relationship)
        {
            return (Func<IDatabase, IRepositoryAccessor<TEntity>>)_cache.GetOrAdd(relationship, r => CreateBelongsToAccessor<TEntity>());
        }

        /// <summary>
        /// Returns an instance of a resource repository accessor for a Belongs To relationship.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to return.</typeparam>
        /// <returns>The resource repository accessor instance.</returns>
        Func<IDatabase, IRepositoryAccessor<TEntity>> CreateBelongsToAccessor<TEntity>()
        {
            var propertyName = GuessDatabasePropertyName(typeof(TEntity));
            var specializedMethodName = nameof(IRepository<IEntityWithId>.GetByIdAsync);

            return CreateRepositoryAccessorFactory<TEntity>(propertyName, specializedMethodName);
        }

        /// <summary>
        /// Returns an instance of a resource repository accessor for a Has Many relationship.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to return.</typeparam>
        /// <param name="relationship">The relationship that defines the accessor to return.</param>
        /// <returns>The resource repository accessor instance.</returns>
        public Func<IDatabase, IRepositoryAccessor<TEntity>> GetOrCreateHasManyAccessor<TEntity>(IHasManyRelationship relationship)
        {
            return (Func<IDatabase, IRepositoryAccessor<TEntity>>)_cache.GetOrAdd(relationship, r => CreateHasManyAccessor<TEntity>((IHasManyRelationship)r));
        }

        /// <summary>
        /// Returns an instance of a resource repository accessor for a Has Many relationship.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to return.</typeparam>
        /// <param name="relationship">The relationship that defines the accessor to return.</param>
        /// <returns>The resource repository accessor instance.</returns>
        Func<IDatabase, IRepositoryAccessor<TEntity>> CreateHasManyAccessor<TEntity>(IHasManyRelationship relationship)
        {
            var propertyName = GuessDatabasePropertyName(typeof(TEntity));

            return CreateRepositoryAccessorFactory<TEntity>(propertyName, $"GetBy{GuessForeignKeyPropertyName(relationship)}Async");
        }

        /// <summary>
        /// Guess the name of the property on the database instance that contains the repository for the given type.
        /// </summary>
        /// <param name="type">The type to guess the repository name for.</param>
        /// <returns>The assumed name of the property on the database that contains the repository for the given type.</returns>
        static string GuessDatabasePropertyName(Type type)
        {
            return $"{type.Name}s";
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
        /// Create a repository accessor factory function.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to return.</typeparam>
        /// <param name="databasePropertyName">The name of the property on the database instance that contains the repository.</param>
        /// <param name="repositoryMethodName">The name of the repository method to invoke.</param>
        /// <returns>The factory method that returns a repository accessor.</returns>
        static Func<IDatabase, IRepositoryAccessor<TEntity>> CreateRepositoryAccessorFactory<TEntity>(
            string databasePropertyName, 
            string repositoryMethodName)
        {
            var databaseParameter = Expression.Parameter(typeof(IDatabase));
            var repositoryParameter = Expression.Property(databaseParameter, databasePropertyName);

            var keysParameter = Expression.Parameter(typeof(IReadOnlyList<int>));
            var cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken));

            var lambdaExpression = Expression
                .Lambda<Func<IReadOnlyList<int>, CancellationToken, Task<IReadOnlyList<TEntity>>>>(
                    Expression.Call(
                        repositoryParameter,
                        GetRepositoryMethod(repositoryParameter, repositoryMethodName),
                        keysParameter,
                        cancellationTokenParameter),
                    keysParameter,
                    cancellationTokenParameter);

            var accessor = typeof(DelegatingRepositoryAccessor<>).MakeGenericType(typeof(TEntity));

            return Expression
                .Lambda<Func<IDatabase, IRepositoryAccessor<TEntity>>>(
                    Expression.New(
                        accessor.GetConstructors()[0],
                        lambdaExpression),
                    databaseParameter)
                .Compile();
        }

        /// <summary>
        /// Returns the repository method from the given member expression.
        /// </summary>
        /// <param name="memberExpression">The member expression to return the method from.</param>
        /// <param name="name">The name of the method to return.</param>
        /// <returns>The repository method.</returns>
        static MethodInfo GetRepositoryMethod(MemberExpression memberExpression, string name)
        {
            return GetRepositoryMethod(((PropertyInfo)memberExpression.Member).PropertyType, name);
        }

        /// <summary>
        /// Returns the repository method with the correct signature.
        /// </summary>
        /// <param name="type">The type to return the repository method from.</param>
        /// <param name="name">The name of the repository method to return.</param>
        /// <returns>The method info that represents the repository method.</returns>
        static MethodInfo GetRepositoryMethod(Type type, string name)
        {
            var method = type.GetMethod(name, new [] { typeof(IReadOnlyList<int>), typeof(CancellationToken) });

            if (method != null)
            {
                return method;
            }

            foreach (var interfaceType in type.GetInterfaces())
            {
                method = GetRepositoryMethod(interfaceType, name);

                if (method != null)
                {
                    return method;
                }
            }

            return null;
        }
        
        /// <summary>
        /// The contract resolver.
        /// </summary>
        public IContractResolver ContractResolver { get; }
    }
}