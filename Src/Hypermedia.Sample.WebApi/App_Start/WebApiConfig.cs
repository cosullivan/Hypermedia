using System.Web.Http;
using System.Web.Http.Cors;
using Autofac.Integration.WebApi;
using Hypermedia.Configuration;
using Hypermedia.JsonApi.WebApi;
using Hypermedia.Metadata;
using Hypermedia.Sample.Resources;
using Hypermedia.Sample.WebApi.Resources;

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

            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

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
                .With<UserResource>("users")
                    .Id(nameof(UserResource.Id))
                    .HasMany<PostResource>("posts")
                        .Template("/v1/users/{id}/posts", "id", resource => resource.Id)
                .With<PostResource>("posts")
                    .Id(nameof(PostResource.Id))
                    .BelongsTo<UserResource>(nameof(PostResource.OwnerUser))
                        .Via(nameof(PostResource.OwnerUserId))
                        .Template("/v1/users/{id}", "id", resource => resource.OwnerUserId)
                    .HasMany<CommentResource>(nameof(PostResource.Comments))
                        .Template("/v1/posts/{id}/comments", "id", resource => resource.Id)
                .With<CommentResource>("comments")
                    .Id(nameof(CommentResource.Id))
                    .BelongsTo<UserResource>(nameof(CommentResource.User))
                        .Via(nameof(CommentResource.UserId))
                        .Template("/v1/users/{id}", "id", resource => resource.UserId)
                    .BelongsTo<PostResource>(nameof(CommentResource.Post))
                        .Via(nameof(CommentResource.PostId))
                        .Template("/v1/posts/{id}", "id", resource => resource.PostId)
                .Build();
        }
    }
}
