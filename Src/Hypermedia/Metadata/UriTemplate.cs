using System.Collections.Generic;

namespace Hypermedia.Metadata
{
    public sealed class UriTemplate
    {
        readonly string _format;
        readonly IReadOnlyList<UriTemplateParameter> _parameters;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The format of the URI template.</param>
        /// <param name="parameters">The list of parameters for the template.</param>
        public UriTemplate(string format, params UriTemplateParameter[] parameters)
        {
            _format = format;
            _parameters = parameters;
        }

        /// <summary>
        /// Bind a resource instance to the template.
        /// </summary>
        /// <param name="resource">The resource instance to bind to the template.</param>
        /// <returns>The URI template with the resource bound to it.</returns>
        public string Bind(object resource)
        {
            var template = _format;

            foreach (var parameter in _parameters)
            {
                var name = $"{{{parameter.Name}}}";
                var value = $"{parameter.Accessor(resource)}";

                template = template.Replace(name, value);
            }

            return template;
        }
    }
}
