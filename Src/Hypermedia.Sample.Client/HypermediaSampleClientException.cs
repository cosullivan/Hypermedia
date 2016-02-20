using System;

namespace Hypermedia.Sample.Client
{
    public sealed class HypermediaSampleClientException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public HypermediaSampleClientException(string message) : base(message) { }
    }
}
