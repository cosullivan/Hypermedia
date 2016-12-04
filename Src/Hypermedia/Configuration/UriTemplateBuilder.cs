using System;
using System.Collections.Generic;
using Hypermedia.Metadata;

namespace Hypermedia.Configuration
{
    public class UriTemplateBuilder<T> : DelegatingContractBuilder<T>
    {
        readonly UriTemplate _uriTemplate;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The parent builder.</param>
        /// <param name="uriTemplate">The URI template that is being editted.</param>
        internal UriTemplateBuilder(IContractBuilder<T> builder, UriTemplate uriTemplate) : base(builder)
        {
            _uriTemplate = uriTemplate;
        }

        /// <summary>
        /// Sets a parameter mapping for the template.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="selector">The selector method to extract the parameter from the resource.</param>
        /// <returns>The URI template builder.</returns>
        public UriTemplateBuilder<T> Parameter(string name, Func<T, object> selector)
        {
            _uriTemplate.Parameters = new List<UriTemplateParameter>(_uriTemplate.Parameters)
            {
                new UriTemplateParameter(name, t => selector((T) t))
            };

            return this;
        }
    }
}