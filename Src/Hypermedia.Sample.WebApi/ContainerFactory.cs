using System;
using System.IO;
using System.Web.Hosting;
using Autofac;
using Autofac.Integration.WebApi;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.WebApi
{
    public static class ContainerFactory
    {
        /// <summary>
        /// Creates an instance of the container builder.
        /// </summary>
        /// <returns>The container builder instance.</returns>
        public static ContainerBuilder CreateContainerBuilder()
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(typeof(ContainerFactory).Assembly);

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

            builder.Register(c => UserRepository.FromXml(Path.Combine(baseFolder, "users.xml")))
                .As<UserRepository>()
                .SingleInstance();

            builder.Register(c => PostRepository.FromXml(Path.Combine(baseFolder, "posts.xml")))
                .As<PostRepository>()
                .SingleInstance();
        }

        /// <summary>
        /// Create an instance of the container.
        /// </summary>
        /// <returns>The instance of the container that was built.</returns>
        public static IContainer CreateContainer()
        {
            return CreateContainerBuilder().Build();
        }
    }
}