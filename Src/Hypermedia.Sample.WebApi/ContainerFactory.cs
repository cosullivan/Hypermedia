using System.Web.Hosting;
using Autofac;
using Autofac.Integration.WebApi;
using Hypermedia.Metadata;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.StackOverflow;
using Hypermedia.Sample.WebApi.Services.Enrichment;

namespace Hypermedia.Sample.WebApi
{
    public static class ContainerFactory
    {
        /// <summary>
        /// Creates an instance of the container builder.
        /// </summary>
        /// <param name="contractResolver">The defined contract resolver.</param>
        /// <returns>The container builder instance.</returns>
        public static ContainerBuilder CreateContainerBuilder(IContractResolver contractResolver)
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(typeof(ContainerFactory).Assembly);

            builder.RegisterInstance(contractResolver)
                .As<IContractResolver>()
                .SingleInstance();

            RegisterRepositories(builder, @"mythology.stackexchange.com");

            return builder;
        }

        /// <summary>
        /// Register the repositories for the specified site.
        /// </summary>
        /// <param name="builder">The container builder to register the repositories on.</param>
        /// <param name="site">The name of the site to load the repositories for.</param>
        static void RegisterRepositories(ContainerBuilder builder, string site)
        {
            var baseFolder = HostingEnvironment.MapPath($"//App_Data//{site}");

            builder.Register(c => new StackOverflowDatabase(baseFolder))
                .As<IDatabase>()
                .SingleInstance();
            
            RegisterServices(builder);
        }

        /// <summary>
        /// Register all of the services.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        static void RegisterServices(ContainerBuilder builder)
        {
            builder
                .RegisterType<ResourceAccessorFactory>()
                .As<IResourceAccessorFactory>()
                .SingleInstance();

            builder
                .RegisterType<RepositoryAccessorFactory>()
                .As<IRepositoryAccessorFactory>()
                .SingleInstance();

            builder
                .Register(c =>
                    new CompositeResourceEnricherFactory(
                        // Custom enrichers can get put at the top of the list here
                        new ResourceEnricherFactory(
                            c.Resolve<IResourceAccessorFactory>(),
                            c.Resolve<IRepositoryAccessorFactory>())))
                .As<IResourceEnricherFactory>()
                .InstancePerDependency();


            builder
                .RegisterType<ResourceEnrichmentService>()
                .As<IResourceEnrichmentService>()
                .SingleInstance();
        }

        /// <summary>
        /// Create an instance of the container.
        /// </summary>
        /// <param name="contractResolver">The defined contract resolver.</param>
        /// <returns>The instance of the container that was built.</returns>
        public static IContainer CreateContainer(IContractResolver contractResolver)
        {
            return CreateContainerBuilder(contractResolver).Build();
        }
    }
}