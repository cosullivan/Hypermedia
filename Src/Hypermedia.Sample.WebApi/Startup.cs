using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using Autofac.Integration.WebApi;
using Hypermedia.Configuration;
using Hypermedia.Json;
using Hypermedia.JsonApi.WebApi;
using Hypermedia.Metadata;
using Hypermedia.Sample.Resources;
using Hypermedia.Sample.WebApi;
using Hypermedia.Sample.WebApi.Services;
using Hypermedia.WebApi;
using Hypermedia.WebApi.Json;
using Microsoft.Owin;
using Owin;
using ExceptionLogger = Hypermedia.Sample.WebApi.Services.ExceptionLogger;

[assembly: OwinStartup(typeof(Startup))]

namespace Hypermedia.Sample.WebApi
{
    public class Startup
    {
        /// <summary>
        /// The configuration entry point for the startup.
        /// </summary>
        /// <param name="app">The app builder to configure.</param>
        public void Configuration(IAppBuilder app)
        {
            var contractResolver = CreateContractResolver();
            var container = ContainerFactory.CreateContainer(contractResolver);

            var config = new HttpConfiguration
            {
                DependencyResolver = new AutofacWebApiDependencyResolver(container),
                IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly
            };

            config.MapHttpAttributeRoutes();
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
            config.Services.Add(typeof(IExceptionLogger), new ExceptionLogger());

            ConfigureFormatters(config, contractResolver);

            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);
        }

        /// <summary>
        /// Configures formatters.
        /// </summary>
        /// <param name="configuration">The HTTP configuration.</param>
        /// <param name="contractResolver">The contract resolver.</param>
        static void ConfigureFormatters(HttpConfiguration configuration, IContractResolver contractResolver)
        {
            configuration.Formatters.Remove(configuration.Formatters.XmlFormatter);
            configuration.Formatters.Remove(configuration.Formatters.JsonFormatter);

            configuration.Formatters.Add(new JsonMediaTypeFormatter(contractResolver, DefaultFieldNamingStrategy.Instance));
            configuration.Formatters.Add(new JsonApiMetadataMediaTypeFormatter(contractResolver));

            configuration.ParameterBindingRules.Add(p =>
            {
                if (p.ParameterType.IsGenericType && p.ParameterType.GetGenericTypeDefinition() == typeof(IRequestMetadata<>))
                {
                    return new JsonApiRequestMetadataParameterBinding(p, contractResolver, p.ParameterType.GenericTypeArguments[0]);
                }

                return null;
            });
        }

        /// <summary>
        /// Creates an instance of the resource contract resolver for the configured model.
        /// </summary>
        /// <returns>The resource contract resolver for the configured model.</returns>
        public static IContractResolver CreateContractResolver()
        {
            var builder = new Builder();

            builder.With<UserResource>("users")
                .Id(nameof(UserResource.Id))
                .HasMany<PostResource>("posts")
                    .Inverse(nameof(PostResource.OwnerUser))
                    .Template("/v1/users/{id}/posts", "id", resource => resource.Id);

            builder.With<PostResource>("posts")
                .Id(nameof(PostResource.Id))
                .BelongsTo<UserResource>(nameof(PostResource.OwnerUser))
                    .BackingField(nameof(PostResource.OwnerUserId))
                    .Inverse(nameof(UserResource.Posts))
                    .Template("/v1/users/{id}", "id", resource => resource.OwnerUserId)
                .BelongsTo<UserResource>(nameof(PostResource.ApproverUser), resource => resource.ApproverId.HasValue)
                    .BackingField(nameof(PostResource.ApproverId))
                    .Template("/v1/users/{id}", "id", resource => resource.ApproverId)
                .HasMany<CommentResource>(nameof(PostResource.Comments))
                    //.Serialization()
                    //    .Embedded()
                    .Inverse(nameof(CommentResource.Post))
                    .Template("/v1/posts/{id}/comments", "id", resource => resource.Id);

            builder.With<CommentResource>("comments")
                .Id(nameof(CommentResource.Id))
                .BelongsTo<UserResource>(nameof(CommentResource.User))
                    .BackingField(nameof(CommentResource.UserId))
                    .Template("/v1/users/{id}", "id", resource => resource.UserId)
                .BelongsTo<PostResource>(nameof(CommentResource.Post))
                    .BackingField(nameof(CommentResource.PostId))
                    .Inverse(nameof(PostResource.Comments))
                    .Template("/v1/posts/{id}", "id", resource => resource.PostId);

            return builder.Build();
        }
    }
}