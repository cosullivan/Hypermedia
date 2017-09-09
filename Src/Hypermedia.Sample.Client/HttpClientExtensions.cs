using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Hypermedia.Sample.Client
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Send a POST request with a cancellation token as an asynchronous operation.
        /// </summary>
        /// <param name="httpClient">The client to perform the operation on.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        public static Task<HttpResponseMessage> PatchAsync(
            this HttpClient httpClient,
            string requestUri, 
            HttpContent content, 
            CancellationToken cancellationToken)
        {
            return PatchAsync(httpClient, new Uri(requestUri, UriKind.RelativeOrAbsolute), content, cancellationToken);
        }

        /// <summary>
        /// Send a PATCH request with a cancellation token as an asynchronous operation.
        /// </summary>
        /// <param name="httpClient">The client to perform the operation on.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        public static Task<HttpResponseMessage> PatchAsync(
            this HttpClient httpClient, 
            Uri requestUri, 
            HttpContent content, 
            CancellationToken cancellationToken)
        {
            return httpClient.SendAsync(
                new HttpRequestMessage(new HttpMethod("PATCH"), requestUri)
                {
                    Content = content
                }, 
                cancellationToken);
        }
    }
}