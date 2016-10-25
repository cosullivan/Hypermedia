namespace Hypermedia.JsonApi.WebApi
{
    public sealed class JsonApiMediaTypeFormatterOptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="includePath">The path that defines the related resources to include.</param>
        public JsonApiMediaTypeFormatterOptions(string includePath)
        {
            IncludePath = includePath;
        }

        /// <summary>
        /// The path that defines the related resources to include.
        /// </summary>
        public string IncludePath { get; }
    }
}