using System;
using System.Collections.Generic;

namespace Hypermedia.Metadata.Runtime
{
    public sealed class ResourceInflector : IResourceInflector
    {
        public static readonly IDictionary<string, string> KnownPlurals = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Return the plural of the given word.
        /// </summary>
        /// <param name="word">The word to return the plural for.</param>
        /// <returns>The plural of the given singular word.</returns>
        public string Pluralize(string word)
        {
            if (KnownPlurals.TryGetValue(word, out string plural))
            {
                return plural;
            }

            return $"{word}s";
        }
    }
}