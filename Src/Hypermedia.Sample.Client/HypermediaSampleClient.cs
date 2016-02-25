using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Hypermedia.Configuration;
using Hypermedia.JsonApi.Client;
using Hypermedia.Metadata;
using Hypermedia.Sample.Resources;

namespace Hypermedia.Sample.Client
{
    public class HypermediaSampleClient : IDisposable
    {
        const string MediaTypeName = "application/vnd.api+json";

        readonly HttpClient _httpClient;
        readonly IResourceContractResolver _resourceContractResolver;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="endpoint">The endpoint to connect the client to.</param>
        /// <param name="accessToken">The access token.</param>
        public HypermediaSampleClient(string endpoint, string accessToken)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(endpoint) };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeName));

            _resourceContractResolver = CreateResolver();
        }

        /// <summary>
        /// Returns the resource contract resolver for the known types.
        /// </summary>
        /// <returns>The resource contract resolver to use when serializing the types.</returns>
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

        /// <summary>
        /// Returns a list of users.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="skip">The number of users to skip from the start.</param>
        /// <param name="take">The number of users to return.</param>
        /// <returns>The list of users.</returns>
        public async Task<IReadOnlyList<UserResource>> GetUsersAsync(int skip = 0, int take = 10, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _httpClient.GetAsync($"v1/users?skip={skip}&take={take}", cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsJsonApiManyAsync<UserResource>(_resourceContractResolver);
        }

        /// <summary>
        /// Return the user with the given resource ID.
        /// </summary>
        /// <param name="id">The ID of the user to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user with the given resource id.</returns>
        public async Task<UserResource> GetUserByIdAsync(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _httpClient.GetAsync($"v1/users/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsJsonApiAsync<UserResource>(_resourceContractResolver);
        }

        /// <summary>
        /// Returns a list of posts.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="skip">The number of posts to skip from the start.</param>
        /// <param name="take">The number of posts to return.</param>
        /// <returns>The list of posts.</returns>
        public async Task<IReadOnlyList<PostResource>> GetPostsAsync(int skip = 0, int take = 10, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _httpClient.GetAsync($"v1/posts?skip={skip}&take={take}", cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsJsonApiManyAsync<PostResource>(_resourceContractResolver);
        }

        /// <summary>
        /// Return the post with the given resource ID.
        /// </summary>
        /// <param name="id">The ID of the post to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The post with the given resource id.</returns>
        public async Task<PostResource> GetPostByIdAsync(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _httpClient.GetAsync($"v1/posts/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsJsonApiAsync<PostResource>(_resourceContractResolver);
        }

        /// <summary>
        /// Return the comments for the given post ID.
        /// </summary>
        /// <param name="postId">The ID of the post to return the comments for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The comments with the given post id.</returns>
        public async Task<IReadOnlyList<CommentResource>> GetCommentsByPostIdAsync(int postId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _httpClient.GetAsync($"v1/posts/{postId}/comments", cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsJsonApiManyAsync<CommentResource>(_resourceContractResolver);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
