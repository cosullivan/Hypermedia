using System;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using Autofac.Integration.WebApi;
using Hypermedia.Configuration;
using Hypermedia.JsonApi.WebApi;
using Hypermedia.Metadata;
using Hypermedia.Sample.Resources;
using Hypermedia.Sample.WebApi.Resources;
using Hypermedia.WebApi;
using ExceptionLogger = Hypermedia.Sample.WebApi.Services.ExceptionLogger;

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

            config.Services.Add(typeof(IExceptionLogger), new ExceptionLogger());

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

            var resolver = CreateResolver();

            configuration.Formatters.Add(new JsonMediaTypeFormatter(resolver));
            configuration.Formatters.Add(new JsonApiMediaTypeFormatter(resolver));
        }

        /// <summary>
        /// Creates an instance of the resource contract resolver for the configured model.
        /// </summary>
        /// <returns>The resource contract resolver for the configured model.</returns>
        static IContractResolver CreateResolver()
        {
            return new Builder()
                .With<UserResource>("users")
                    .Id(nameof(UserResource.Id))
                    .Calculated("DaysSinceCreation", Resource.CalculateDaysSinceCreation)
                    .HasMany<PostResource>("posts")
                        .Template("/v1/users/{id}/posts", "id", resource => resource.Id)
                .With<PostResource>("posts")
                    .Id(nameof(PostResource.Id))
                    .Calculated("DaysSinceCreation", Resource.CalculateDaysSinceCreation)
                    .BelongsTo<UserResource>(nameof(PostResource.OwnerUser))
                        .Via(nameof(PostResource.OwnerUserId))
                        .Template("/v1/users/{id}", "id", resource => resource.OwnerUserId)
                    .HasMany<CommentResource>(nameof(PostResource.Comments))
                        .Template("/v1/posts/{id}/comments", "id", resource => resource.Id)
                .With<CommentResource>("comments")
                    .Id(nameof(CommentResource.Id))
                    .Calculated("DaysSinceCreation", Resource.CalculateDaysSinceCreation)
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
