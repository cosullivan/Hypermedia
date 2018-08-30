namespace Hypermedia.AspNetCore
{
    public sealed class HypermediaAspNetCoreException : HypermediaException
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The message arguments.</param>
        public HypermediaAspNetCoreException(string format, params object[] args) : base(format, args) { }
    }
}