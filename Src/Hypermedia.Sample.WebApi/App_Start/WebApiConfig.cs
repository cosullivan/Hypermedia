using System.Web.Http;
using Autofac.Integration.WebApi;
using Hypermedia.Configuration;
using Hypermedia.JsonApi.WebApi;
using Hypermedia.Metadata;

namespace Hypermedia.Sample.WebApi
{
    public static class WebApiConfig
    {
        /// <summary>
        /// Register the configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(ContainerFactory.CreateContainer());

            ConfigureFormatters(config);
        }

        /// <summary>
        /// Configures formatters.
        /// </summary>
        /// <param name="configuration">The HTTP configuration.</param>
        static void ConfigureFormatters(HttpConfiguration configuration)
        {
            configuration.Formatters.Remove(configuration.Formatters.XmlFormatter);
            configuration.Formatters.Remove(configuration.Formatters.JsonFormatter);

            configuration.Formatters.Add(new JsonApiMediaTypeFormatter(CreateResolver()));
        }

        /// <summary>
        /// Creates an instance of the resource contract resolver for the configured model.
        /// </summary>
        /// <returns>The resource contract resolver for the configured model.</returns>
        static IResourceContractResolver CreateResolver()
        {
            return new Builder()
                .With<User>("users")
                    .Id(nameof(User.Id))
                    .HasMany<Post>("posts")
                        .Template("/v1/users/{id}/posts", "id", resource => resource.Id)
                .With<Post>("posts")
                    .Id(nameof(Post.Id))
                    //.Ignore(nameof(Post.OwnerUser))
                    .Ignore(nameof(Post.OwnerUserId))
                    .BelongsTo<User>("user")
                        .Via(nameof(Post.OwnerUser))
                        .Template("/v1/users/{id}", "id", resource => resource.OwnerUserId)
                .Build();
        }
    }
}
