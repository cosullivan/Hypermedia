using System;
using System.Collections.Specialized;
using System.Linq;

namespace Hypermedia.WebApi.Json
{
    public static class NameValueCollectionExtensions
    {
        const string PrettifyParameterName = "$prettify";

        /// <summary>
        /// Returns a value indicating whether or not to prettify the response.
        /// </summary>
        /// <param name="collection">The collection to check the value on.</param>
        /// <returns>true if the response should be prettified, false if not.</returns>
        public static bool Prettify(this NameValueCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (collection[PrettifyParameterName] != null)
            {
                return new[] { "yes", "1", "true" }.Contains(collection[PrettifyParameterName], StringComparer.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}