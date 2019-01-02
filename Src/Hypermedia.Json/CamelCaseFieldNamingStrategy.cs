using System;

namespace Hypermedia.Json
{
    public sealed class CamelCaseFieldNamingStrategy : IFieldNamingStrategy
    {
        public static readonly IFieldNamingStrategy Instance = new CamelCaseFieldNamingStrategy();

        /// <summary>
        /// Returns the name as to how it is represented for this strategy.
        /// </summary>
        /// <param name="name">The name to modified according to the strategy.</param>
        /// <returns>The name that was modified according to the strategy.</returns>
        public string GetName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.Length == 1)
            {
                return name.ToLower();
            }

            return $"{name[0].ToString().ToLower()}{name.Substring(1)}";
        }

        /// <summary>
        /// Resolve the name from the strategy back to the field name.
        /// </summary>
        /// <param name="name">The name that was defined from the strategy.</param>
        /// <returns>The name that exists in the model.</returns>
        public string ResolveName(string name)
        {
            if (name.Length == 1)
            {
                return name.ToUpper();
            }

            return $"{name[0].ToString().ToUpper()}{name.Substring(1)}";
        }
    }
}