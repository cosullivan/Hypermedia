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
        /// <param name="input">The dasherized input.</param>
        /// <returns>The camelized output.</returns>
        internal static string Camelize(this string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return String.Concat(input.Split('-').Select(CamelCase));
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
        /// Return the dasherized version of the given input.
        /// </summary>
        /// <param name="input">The name to dasherize.</param>
        /// <returns>The dasherized version of the input.</returns>
        internal static string Dasherize(this string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return String.Join("-", SplitAtUpperCase(input).Select(part => part.ToLower()));
        }

        /// <summary>
        /// Split the input based on upper-case character boundaries.
        /// </summary>
        /// <param name="input">The input to split.</param>
        /// <returns>The list of parts that the input was split into.</returns>
        static IEnumerable<string> SplitAtUpperCase(string input)
        {
            int i;
            int last;
            for (i = 0, last = 0; i < input.Length; i++)
            {
                if (Char.IsUpper(input[i]))
                {
                    yield return input.Substring(last, i - last);
                    last = i;
                }
            }

            yield return input.Substring(last, i - last);
        }

        /// <summary>
        /// Return the string with the first character set to lowercase.
        /// </summary>
        /// <param name="input">The input string to convert.</param>
        /// <returns>The representation of the input with the first character set to lowercase.</returns>
        internal static string LowerFirstCharacter(this string input)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            if (input.Length == 1)
            {
                return input.ToLower();
            }

            return $"{input[0].ToString().ToLower()}{input.Substring(1)}";
        }
    }
}
