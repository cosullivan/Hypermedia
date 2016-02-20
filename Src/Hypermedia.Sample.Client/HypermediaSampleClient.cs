using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Hypermedia.Configuration;
using Hypermedia.JsonApi.Client;
using Hypermedia.Metadata;

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
                .With<User>("users")
                    .Id(nameof(Resource.Id))
                .With<Post>("posts")
                    .Id(nameof(Resource.Id))
                    .BelongsTo<User>("OwnerUser")
                        .Via(nameof(Post.OwnerUserId))
                        .Template("/v1/users/{id}", "id", resource => resource.OwnerUserId)
                .Build();
        }

        /// <summary>
        /// Returns a list of users.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="skip">The number of users to skip from the start.</param>
        /// <param name="take">The number of users to return.</param>
        /// <returns>The list of users.</returns>
        public async Task<IReadOnlyList<User>> GetUsersAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _httpClient.GetAsync($"v1/users?skip={skip}&take={take}", cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsJsonApiManyAsync<User>(_resourceContractResolver);
        }

        /// <summary>
        /// Return the user with the given resource ID.
        /// </summary>
        /// <param name="id">The ID of the user to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user with the given resource id.</returns>
        public async Task<User> GetUserByIdAsync(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _httpClient.GetAsync($"v1/users/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsJsonApiAsync<User>(_resourceContractResolver);
        }

        /// <summary>
        /// Returns a list of posts.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="skip">The number of posts to skip from the start.</param>
        /// <param name="take">The number of posts to return.</param>
        /// <returns>The list of posts.</returns>
        public async Task<IReadOnlyList<Post>> GetPostsAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _httpClient.GetAsync($"v1/posts?skip={skip}&take={take}", cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsJsonApiManyAsync<Post>(_resourceContractResolver);
        }

        /// <summary>
        /// Return the post with the given resource ID.
        /// </summary>
        /// <param name="id">The ID of the post to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The post with the given resource id.</returns>
        public async Task<Post> GetPostByIdAsync(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _httpClient.GetAsync($"v1/posts/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsJsonApiAsync<Post>(_resourceContractResolver);
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
