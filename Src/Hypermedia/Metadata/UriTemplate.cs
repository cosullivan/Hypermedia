using System;
using System.Collections.Generic;

namespace Hypermedia.Metadata
{
    public sealed class UriTemplate
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The format of the URI template.</param>
        /// <param name="parameters">The list of parameters for the template.</param>
        public UriTemplate(string format, params UriTemplateParameter[] parameters)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            Format = format;
            Parameters = new List<UriTemplateParameter>();
        }

        /// <summary>
        /// Bind a resource instance to the template.
        /// </summary>
        /// <param name="resource">The resource instance to bind to the template.</param>
        /// <returns>The URI template with the resource bound to it.</returns>
        public string Bind(object resource)
        {
            var template = Format;

            foreach (var parameter in Parameters)
            {
                var name = $"{{{parameter.Name}}}";
                var value = $"{parameter.Accessor(resource)}";

                template = template.Replace(name, value);
            }

            return template;
        }

        /// <summary>
        /// The format of the URI.
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// The list of parameters for the template.
        /// </summary>
        public IReadOnlyList<UriTemplateParameter> Parameters { get; internal set; }
    }
}