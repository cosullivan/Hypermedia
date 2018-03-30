using Hypermedia.Configuration;
using Hypermedia.JsonApi.AspNetCore;
using Hypermedia.Metadata;
using Hypermedia.Sample.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Hypermedia.Sample.AspNetCore
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var contractResolver = CreateContractResolver();

            services.AddMvc().UseHypermediaFormatters(contractResolver);

            services.AddRouting(routing => { });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
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