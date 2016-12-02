namespace Hypermedia.Json
{
    public interface IFieldNamingStrategy
    {
        /// <summary>
        /// Returns the name as to how it is represented for this strategy.
        /// </summary>
        /// <param name="name">The name to modified according to the strategy.</param>
        /// <returns>The name that was modified according to the strategy.</returns>
        string GetName(string name);

        /// <summary>
        /// Resolve the name from the strategy back to the field name.
        /// </summary>
        /// <param name="name">The name that was defined from the strategy.</param>
        /// <returns>The name that exists in the model.</returns>
        string ResolveName(string name);
    }
}