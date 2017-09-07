using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Autofac;
using Autofac.Integration.Owin;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.WebApi.Resources;
using Hypermedia.Sample.WebApi.Services.Enrichment;
using Hypermedia.WebApi;

namespace Hypermedia.Sample.WebApi.Controllers
{
    public abstract class ResourceController : ApiController
    {
        /// <summary>
        /// Executes asynchronously a single HTTP operation.
        /// </summary>
        /// <param name="httpControllerContext">The controller context for a single HTTP operation.</param>
        /// <param name="cancellationToken">The cancellation token assigned for the HTTP operation.</param>
        /// <returns>The newly started task.</returns>
        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext httpControllerContext, CancellationToken cancellationToken)
        {
            Container = httpControllerContext.Request.GetOwinContext().GetAutofacLifetimeScope();
            Database = Container.Resolve<IDatabase>();
            ResourceEnrichment = Container.Resolve<IResourceEnrichmentService>();

            return base.ExecuteAsync(httpControllerContext, cancellationToken);
        }

        /// <summary>
        /// Returns a NoContent response.
        /// </summary>
        /// <returns>The action result that represents no content.</returns>
        protected IHttpActionResult NoContent()
        {
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Returns a Forbidden response.
        /// </summary>
        /// <returns>The action result that represents a forbidden response.</returns>
        protected IHttpActionResult Forbidden()
        {
            return StatusCode(HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// The container instance.
        /// </summary>
        protected ILifetimeScope Container { get; private set; }

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
        public IHttpActionResult Ok(IEnumerable<TEntity> entities)
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
        public Task<IHttpActionResult> OkAsync(IEnumerable<TEntity> entities, IRequestMetadata<TResource> requestMetadata = null, CancellationToken cancellationToken = default(CancellationToken))
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
        public Task<IHttpActionResult> OkAsync(TEntity entity, IRequestMetadata<TResource> requestMetadata = null, CancellationToken cancellationToken = default(CancellationToken))
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
        public async Task<IHttpActionResult> OkAsync(IReadOnlyList<TResource> resources, IRequestMetadata<TResource> requestMetadata = null, CancellationToken cancellationToken = default(CancellationToken))
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
        public async Task<IHttpActionResult> OkAsync(TResource resource, IRequestMetadata<TResource> requestMetadata = null, CancellationToken cancellationToken = default(CancellationToken))
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
        public IHttpActionResult Ok(TEntity entity)
        {
            return Ok(entity.Map<TEntity, TResource>());
        }
    }
}