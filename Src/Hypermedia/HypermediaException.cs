using System;

namespace Hypermedia
{
    public class HypermediaException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The message arguments.</param>
        public HypermediaException(string format, params object[] args) : base(String.Format(format, args)) { }
    }
}