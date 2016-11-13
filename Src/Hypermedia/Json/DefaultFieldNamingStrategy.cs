namespace Hypermedia.Json
{
    public sealed class DefaultFieldNamingStrategy : IFieldNamingStratgey
    {
        /// <summary>
        /// Returns the name as to how it is represented for this strategy.
        /// </summary>
        /// <param name="name">The name to modified according to the strategy.</param>
        /// <returns>The name that was modified according to the strategy.</returns>
        public string GetName(string name)
        {
            return name;
        }

        /// <summary>
        /// Resolve the name from the strategy back to the field name.
        /// </summary>
        /// <param name="name">The name that was defined from the strategy.</param>
        /// <returns>The name that exists in the model.</returns>
        public string ResolveName(string name)
        {
            return name;
        }
    }
}
