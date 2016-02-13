using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Hypermedia.Metadata;
using Hypermedia.Metadata.Runtime;
using JsonLite;

namespace Hypermedia.JsonApi.Client
{
    public static class HttpContentExtensions
    {
        /// <summary>
        /// Read the content as a JSONAPI response object.
        /// </summary>
        /// <param name="httpContent">The HTTP content to read the JSONAPI response from.</param>
        /// <returns>The JSONAPI response element that was read from the stream in the HTTP content.</returns>
        public static async Task<JsonApiResponse> ReadAsJsonApiAsync(this HttpContent httpContent)
        {
            if (httpContent == null)
            {
                throw new ArgumentNullException(nameof(httpContent));
            }

            var jsonAst = Json.CreateAst(await httpContent.ReadAsStringAsync());

            return new JsonApiResponse(jsonAst);
        }

        /// <summary>
        /// Read the content as a JSONAPI response object.
        /// </summary>
        /// <param name="httpContent">The HTTP content to read the JSONAPI response from.</param>
        /// <returns>The JSONAPI response element that was read from the stream in the HTTP content.</returns>
        public static Task<TEntity> ReadAsJsonApiAsync<TEntity>(this HttpContent httpContent)
        {
            var resourceContractResolver = new ResourceContractResolver(RuntimeResourceContract<TEntity>.CreateRuntimeType());

            return httpContent.ReadAsJsonApiAsync<TEntity>(resourceContractResolver);
        }

        /// <summary>
        /// Read the content as a JSONAPI response object.
        /// </summary>
        /// <param name="httpContent">The HTTP content to read the JSONAPI response from.</param>
        /// <param name="resourceContractResolver">The resource contract resolver use to resolve types during deserialization.</param>
        /// <returns>The JSONAPI response element that was read from the stream in the HTTP content.</returns>
        public static async Task<TEntity> ReadAsJsonApiAsync<TEntity>(this HttpContent httpContent, IResourceContractResolver resourceContractResolver)
        {
            if (httpContent == null)
            {
                throw new ArgumentNullException(nameof(httpContent));
            }

            var response = await httpContent.ReadAsJsonApiAsync();

            return response.Get<TEntity>(resourceContractResolver);
        }

        /// <summary>
        /// Read the content as a JSONAPI response object.
        /// </summary>
        /// <param name="httpContent">The HTTP content to read the JSONAPI response from.</param>
        /// <returns>The JSONAPI response element that was read from the stream in the HTTP content.</returns>
        public static Task<List<TEntity>> ReadAsJsonApiManyAsync<TEntity>(this HttpContent httpContent)
        {
            var resourceContractResolver = new ResourceContractResolver(RuntimeResourceContract<TEntity>.CreateRuntimeType());

            return httpContent.ReadAsJsonApiManyAsync<TEntity>(resourceContractResolver);
        }

        /// <summary>
        /// Read the content as a JSONAPI response object.
        /// </summary>
        /// <param name="httpContent">The HTTP content to read the JSONAPI response from.</param>
        /// <param name="resourceContractResolver">The resource contract resolver use to resolve types during deserialization.</param>
        /// <returns>The JSONAPI response element that was read from the stream in the HTTP content.</returns>
        public static async Task<List<TEntity>> ReadAsJsonApiManyAsync<TEntity>(this HttpContent httpContent, IResourceContractResolver resourceContractResolver)
        {
            if (httpContent == null)
            {
                throw new ArgumentNullException(nameof(httpContent));
            }

            var response = await httpContent.ReadAsJsonApiAsync();

            return response.GetMany<TEntity>(resourceContractResolver).ToList();
        }
    }
}