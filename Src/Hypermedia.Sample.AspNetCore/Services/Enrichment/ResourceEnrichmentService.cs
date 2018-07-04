using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Hypermedia.AspNetCore;
using Hypermedia.Metadata;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.AspNetCore.Services.Enrichment
{
    public sealed class ResourceEnrichmentService : IResourceEnrichmentService
    {
        readonly ConcurrentDictionary<IRelationship, object> _delegateCache = new ConcurrentDictionary<IRelationship, object>();
        readonly IResourceEnricherFactory _resourceEnricherFactory;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="resourceEnricherFactory">The resource enricher factory.</param>
        public ResourceEnrichmentService(IResourceEnricherFactory resourceEnricherFactory)
        {
            _resourceEnricherFactory = resourceEnricherFactory;
        }

        /// <summary>
        /// Enrich the list of resources.
        /// </summary>
        /// <typeparam name="TResource">The resource type to enrich.</typeparam>
        /// <param name="database">The database to perform the enrichment within.</param>
        /// <param name="resources">The list of resources to enrich.</param>
        /// <param name="memberPaths">The list of member access paths that define what to enrich.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which asynchronously performs the operation.</returns>
        public async Task EnrichAsync<TResource>(
            IDatabase database,
            IReadOnlyList<TResource> resources, 
            IReadOnlyList<MemberPath> memberPaths, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var memberPath in memberPaths.Where(m => m.Member is IRelationship))
            {
                var context = new EnrichmentContext<TResource>(database, resources, memberPath);

                await EnrichAsync(context, cancellationToken);
            }
        }

        /// <summary>
        /// Enrich the list of resources.
        /// </summary>
        /// <typeparam name="TResource">The resource type to enrich.</typeparam>
        /// <param name="context">The enrichment context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which asynchronously performs the operation.</returns>
        async Task EnrichAsync<TResource>(EnrichmentContext<TResource> context, CancellationToken cancellationToken)
        {
            var relationship = (IRelationship) context.MemberPath.Member;

            var @delegate = GetOrCreateEnrichmentDelegate<TResource>(relationship);

            await @delegate(context, cancellationToken);
        }

        /// <summary>
        /// Gets (or creates if it doesnt exist) a delegate that can be called to execute the enrichment.
        /// </summary>
        /// <typeparam name="TResource">The resource type to enrich.</typeparam>
        /// <param name="relationship">The relationship that defines how the enrichment should occurr.</param>
        /// <returns>The delegate to perform the enrichment.</returns>
        Func<EnrichmentContext<TResource>, CancellationToken, Task> GetOrCreateEnrichmentDelegate<TResource>(IRelationship relationship)
        {
            return (Func<EnrichmentContext<TResource>, CancellationToken, Task>)_delegateCache.GetOrAdd(relationship, CreateEnrichmentDelegate<TResource>);
        }

        /// <summary>
        /// Create a delegate that can be used to perform the enrichment.
        /// </summary>
        /// <typeparam name="TResource">The resource type to enrich.</typeparam>
        /// <param name="relationship">The relationship that defines how the enrichment should occurr.</param>
        /// <returns>The delegate to perform the enrichment.</returns>
        Func<EnrichmentContext<TResource>, CancellationToken, Task> CreateEnrichmentDelegate<TResource>(IRelationship relationship)
        {
            var parameters = new[]
            {
                Expression.Parameter(typeof(EnrichmentContext<TResource>)),
                Expression.Parameter(typeof(CancellationToken))
            };

            var methodCallExpression = Expression.Call(
                Expression.Constant(this),
                nameof(EnrichAsync),
                new[]
                {
                    typeof(TResource),
                    relationship.RelatedTo
                },
                // ReSharper disable once CoVariantArrayConversion
                parameters);

            return Expression.Lambda<Func<EnrichmentContext<TResource>, CancellationToken, Task>>(methodCallExpression, parameters).Compile();
        }

        /// <summary>
        /// Perform the enrichment for the given source and it's children.
        /// </summary>
        /// <typeparam name="TSource">The resource type to create the enricher for.</typeparam>
        /// <typeparam name="TDestination">The resource type on the other end of the relationship that is being enriched from.</typeparam>
        /// <param name="context">The enrichment context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which asynchronously performs the operation.</returns>
        async Task EnrichAsync<TSource, TDestination>(
            EnrichmentContext<TSource> context,
            CancellationToken cancellationToken)
        {
            var enricher = _resourceEnricherFactory.CreateEnricher<TSource, TDestination>((IRelationship)context.MemberPath.Member, context.Database);

            var destination = await enricher.EnrichAsync(context.Resources, cancellationToken);

            await EnrichAsync(context.Database, destination, context.MemberPath.Children, cancellationToken);
        }

        #region EnrichmentContext

        class EnrichmentContext<TResource>
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="database">The database to perform the operation within.</param>
            /// <param name="resources">The list of resources that are to be enriched.</param>
            /// <param name="memberPath">The member path to enrich.</param>
            public EnrichmentContext(IDatabase database, IReadOnlyList<TResource> resources, MemberPath memberPath)
            {
                Database = database;
                Resources = resources;
                MemberPath = memberPath;
            }

            /// <summary>
            /// The database to perform the operation within.
            /// </summary>
            public IDatabase Database { get; }

            /// <summary>
            /// The list of resources that are to be enriched.
            /// </summary>
            public IReadOnlyList<TResource> Resources { get; }

            /// <summary>
            /// The member path to enrich.
            /// </summary>
            public MemberPath MemberPath { get; }
        }

        #endregion
    }
}