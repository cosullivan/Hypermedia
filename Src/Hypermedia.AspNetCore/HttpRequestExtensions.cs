using System;
using Hypermedia.Json;
using Microsoft.AspNetCore.Http;

namespace Hypermedia.AspNetCore
{
    internal static class HttpRequestExtensions
    {
        /// <summary>
        /// Returns the field naming strategy, taking into account the possibility that it could be overridden.
        /// </summary>
        /// <param name="request">The request information that is to be used to determine the field naming strategy to use.</param>
        /// <param name="parameter">The name of the query string parameter that defines the field naming strategy.</param>
        /// <returns>The per-request field naming strategy to use.</returns>
        internal static IFieldNamingStrategy GetFieldNamingStrategy(this HttpRequest request, string parameter)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Query.TryGetValue(parameter, out var value) == false)
            {
                return null;
            }

            switch (value)
            {
                case "none":
                    return DefaultFieldNamingStrategy.Instance;

                case "camel":
                    return CamelCaseFieldNamingStrategy.Instance;

                case "dash":
                    return DasherizedFieldNamingStrategy.Instance;

                case "snake":
                    return SnakeCaseNamingStrategy.Instance;
            }

            return null;
        }
    }
}
