using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hypermedia.AspNetCore;
using Hypermedia.Sample.AspNetCore.Resources;
using Hypermedia.Sample.AspNetCore.Services.Enrichment;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.StackOverflow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hypermedia.Sample.AspNetCore.Controllers
{
    public abstract class ResourceController : Controller
    {
        /// <summary>
        /// Called before the action method is invoked.
        /// </summary>
        /// <param name="executingContext">The action executing context.</param>
        /// <param name="next">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate" /> to execute. Invoke this delegate in the body of <see cref="M:Microsoft.AspNetCore.Mvc.Controller.OnActionExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext,Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate)" /> to continue execution of the action.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> instance.</returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext executingContext, ActionExecutionDelegate next)
        {
            if (executingContext == null)
            {
                throw new ArgumentNullException(nameof(executingContext));
            }

            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            OnActionExecuting(executingContext);

            if (executingContext.Result != null)
            {
                return;
            }

            var contractResolver = Startup.CreateContractResolver();

            Database = new StackOverflowDatabase(AppDomain.CurrentDomain.GetData("DataDirectory").ToString());

            ResourceEnrichment = new ResourceEnrichmentService(
                new CompositeResourceEnricherFactory(
                    // Custom enrichers can get put at the top of the list here
                    new ResourceEnricherFactory(
                        new ResourceAccessorFactory(contractResolver),
                        new RepositoryAccessorFactory(contractResolver))));

            var executedContext = await next();
            OnActionExecuted(executedContext);
        }
        
        /// <summary>
        /// The database instance for the scope of the controller.
        /// </summary>
        protected IDatabase Database { get; private set; }

        /// <summary>
        /// The resource enrichment service.
        /// </summary>
        protected IResourceEnrichmentService ResourceEnrichment { get; private set; }
    }

    public abstract class ResourceController<TEntity, TResource> : ResourceController
    {
        /// <summary>
        /// Returns the list of entities as a list of resources.
        /// </summary>
        /// <param name="entities">The list of entities to return as resources.</param>
        /// <returns>The list of resources that represent the entities.</returns>
        public IActionResult Ok(IEnumerable<TEntity> entities)
        {
            return Ok(entities.SelectList(Resource.Map<TEntity, TResource>));
        }

        /// <summary>
        /// Returns the list of entities as a list of resources with the optional enrichment.
        /// </summary>
        /// <param name="entities">The list of entities to return as resources.</param>
        /// <param name="requestMetadata">The request metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of resources that represent the entities.</returns>
        public Task<IActionResult> OkAsync(IEnumerable<TEntity> entities, IRequestMetadata<TResource> requestMetadata = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return OkAsync(entities.SelectList(Resource.Map<TEntity, TResource>), requestMetadata, cancellationToken);
        }

        /// <summary>
        /// Returns the entitiy as a resource with the optional enrichment.
        /// </summary>
        /// <param name="entity">The list of entities to return as a resource.</param>
        /// <param name="requestMetadata">The request metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The resource that represents the entity.</returns>
        public Task<IActionResult> OkAsync(TEntity entity, IRequestMetadata<TResource> requestMetadata = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return OkAsync(entity.Map<TEntity, TResource>(), requestMetadata, cancellationToken);
        }

        /// <summary>
        /// Returns the list of resources with the optional enrichment.
        /// </summary>
        /// <param name="resources">The list resources to return.</param>
        /// <param name="requestMetadata">The request metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of resources to return.</returns>
        public async Task<IActionResult> OkAsync(IReadOnlyList<TResource> resources, IRequestMetadata<TResource> requestMetadata = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (requestMetadata?.Include != null)
            {
                await ResourceEnrichment.EnrichAsync(Database, resources, requestMetadata.Include, cancellationToken);
            }

            return Ok(resources);
        }

        /// <summary>
        /// Returns the resource with the optional enrichment.
        /// </summary>
        /// <param name="resource">The resource to return.</param>
        /// <param name="requestMetadata">The request metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The resource to return.</returns>
        public async Task<IActionResult> OkAsync(TResource resource, IRequestMetadata<TResource> requestMetadata = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (requestMetadata?.Include != null)
            {
                await ResourceEnrichment.EnrichAsync(Database, new[] { resource }, requestMetadata.Include, cancellationToken);
            }

            return Ok(resource);
        }

        /// <summary>
        /// Returns the entity as a resource.
        /// </summary>
        /// <param name="entity">The entity to return as a resource.</param>
        /// <returns>The resource that represents the entity.</returns>
        public IActionResult Ok(TEntity entity)
        {
            return Ok(entity.Map<TEntity, TResource>());
        }
    }
}