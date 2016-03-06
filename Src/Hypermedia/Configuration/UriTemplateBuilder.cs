using System;
using System.Collections.Generic;
using Hypermedia.Metadata;

namespace Hypermedia.Configuration
{
    public class UriTemplateBuilder<T> : IContractBuilder<T>
    {
        readonly IContractBuilder<T> _builder;
        readonly string _format;
        readonly List<UriTemplateParameter> _parameters = new List<UriTemplateParameter>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The parent builder.</param>
        /// <param name="format">The URI template format.</param>
        internal UriTemplateBuilder(IContractBuilder<T> builder, string format)
        {
            _builder = builder;
            _format = format;
        }

        /// <summary>
        /// Create an instance of the Uri Template.
        /// </summary>
        /// <returns>The intance of the Uri Template.</returns>
        internal UriTemplate CreateTemplate()
        {
            return new UriTemplate(_format, _parameters.ToArray());
        }

        /// <summary>
        /// Build a resource contract resolver with the known types.
        /// </summary>
        /// <returns>The resource contract resolver that is aware of the types that were configured through the builder.</returns>
        public IContractResolver Build()
        {
            return _builder.Build();
        }

        /// <summary>
        /// Returns a Resource Builder for a resource type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the resource to return the builder for.</typeparam>
        /// <param name="discovery">The type discovery mechanism.</param>
        /// <returns>The resource builder to configure.</returns>
        public ContractBuilder<TEntity> With<TEntity>(ITypeDiscovery discovery)
        {
            return _builder.With<TEntity>(discovery);
        }

        /// <summary>
        /// Returns a field.
        /// </summary>
        /// <param name="name">The name of the field to return.</param>
        /// <returns>The field builder build the field.</returns>
        public FieldBuilder<T> Field(string name)
        {
            return _builder.Field(name);
        }

        /// <summary>
        /// Returns a BelongsTo relationship.
        /// </summary>
        /// <param name="name">The name of the relationship to return.</param>
        /// <returns>The relationship builder build the relationship.</returns>
        public RelationshipBuilder<T> BelongsTo<TOther>(string name)
        {
            return _builder.BelongsTo<TOther>(name);
        }

        /// <summary>
        /// Returns a HasMany relationship.
        /// </summary>
        /// <param name="name">The name of the relationship to return.</param>
        /// <returns>The relationship builder build the relationship.</returns>
        public RelationshipBuilder<T> HasMany<TOther>(string name)
        {
            return _builder.HasMany<TOther>(name);
        }

        /// <summary>
        /// Sets a parameter mapping for the template.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="selector">The selector method to extract the parameter from the resource.</param>
        /// <returns>The URI template builder.</returns>
        public UriTemplateBuilder<T> Parameter(string name, Func<T, object> selector)
        {
            _parameters.Add(new UriTemplateParameter(name, t => selector((T)t)));

            return this;
        }
    }
}
