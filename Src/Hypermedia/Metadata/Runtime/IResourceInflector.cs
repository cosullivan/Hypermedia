namespace Hypermedia.Metadata.Runtime
{
    public interface IResourceInflector
    {
        /// <summary>
        /// Return the plural of the given word.
        /// </summary>
        /// <param name="word">The word to return the plural for.</param>
        /// <returns>The plural of the given singular word.</returns>
        string Pluralize(string word);
    }
}
