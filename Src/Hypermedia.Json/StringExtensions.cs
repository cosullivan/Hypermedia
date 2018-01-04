using System;
using System.Collections.Generic;
using System.Linq;

namespace Hypermedia.Json
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Convert a dasherized string input into a camel case representation.
        /// </summary>
        /// <param name="parts">The list of string parts to return as a camelized string.</param>
        /// <returns>The camelized output.</returns>
        internal static string Camelize(this IEnumerable<string> parts)
        {
            if (parts == null)
            {
                throw new ArgumentNullException(nameof(parts));
            }

            return String.Concat(parts.Select(CamelCase));
        }

        /// <summary>
        /// Convert the given input string to a camel case representation.
        /// </summary>
        /// <param name="input">The input string to convert.</param>
        /// <returns>The converted output.</returns>
        static string CamelCase(string input)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            if (input.Length == 1)
            {
                return input.ToUpper();
            }

            return $"{input[0].ToString().ToUpper()}{input.Substring(1)}";
        }

        /// <summary>
        /// Split the input based on upper-case character boundaries.
        /// </summary>
        /// <param name="input">The input to split.</param>
        /// <param name="predicate">The predicate to match to determine the split.</param>
        /// <returns>The list of parts that the input was split into.</returns>
        public static IEnumerable<string> SplitAt(this string input, Func<char, bool> predicate)
        {
            int i;
            int last;
            for (i = 1, last = 0; i < input.Length; i++)
            {
                if (predicate(input[i]))
                {
                    yield return input.Substring(last, i - last).ToLower();
                    last = i;
                }
            }

            yield return input.Substring(last, i - last).ToLower();
        }

        /// <summary>
        /// Returns the sequence of input's as a sequence of lower case strings.
        /// </summary>
        /// <param name="parts">The input parts to return as a lower case sequence.</param>
        /// <returns>The lower case sequence.</returns>
        public static IEnumerable<string> ToLowerCase(this IEnumerable<string> parts)
        {
            if (parts == null)
            {
                throw new ArgumentNullException(nameof(parts));
            }

            return parts.Select(i => i.ToLower());
        }

        /// <summary>
        /// Join the string parts with the given separator.
        /// </summary>
        /// <param name="parts">The string parts to join.</param>
        /// <param name="separator">The separator to join the string parts with.</param>
        /// <returns>The string that was created by joining the parts.</returns>
        public static string Join(this IEnumerable<string> parts, string separator)
        {
            if (parts == null)
            {
                throw new ArgumentNullException(nameof(parts));
            }

            return String.Join(separator, parts);
        }
    }
}