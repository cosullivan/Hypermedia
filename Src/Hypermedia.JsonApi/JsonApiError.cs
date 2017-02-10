namespace Hypermedia.JsonApi
{
    public sealed class JsonApiError
    {
        /// <summary>
        /// The HTTP status code applicable to this problem, expressed as a string value.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// An application-specific error code, expressed as a string value.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// A short, human-readable summary of the problem.
        /// </summary>
        /// <remarks>This should not change from occurrence to occurrence of the problem, except for purposes of localization.</remarks>
        public string Title { get; set; }

        /// <summary>
        /// A human-readable explanation specific to this occurrence of the problem. 
        /// </summary>
        /// <remarks>Like title, this field’s value can be localized.</remarks>
        public string Detail { get; set; }
    }
}