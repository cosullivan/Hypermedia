using JsonLite;

namespace Hypermedia.JsonApi
{
    public sealed class JsonApiException : JsonException
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The message arguments.</param>
        public JsonApiException(string format, params object[] args) : base(format, args) { }
    }
}