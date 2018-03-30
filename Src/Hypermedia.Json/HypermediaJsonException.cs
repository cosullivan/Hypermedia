namespace Hypermedia.Json
{
    public sealed class HypermediaJsonException : HypermediaException
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The message arguments.</param>
        public HypermediaJsonException(string format, params object[] args) : base(format, args) { }
    }
}