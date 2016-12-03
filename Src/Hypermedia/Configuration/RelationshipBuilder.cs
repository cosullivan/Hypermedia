using System;
using System.Collections.Generic;
using System.Linq;
using Hypermedia.Metadata;
using Hypermedia.Metadata.Runtime;

namespace Hypermedia.Configuration
{
    public sealed class RelationshipBuilder<T> : IContractBuilder<T>
    {
        readonly IContractBuilder<T> _builder;
        readonly string _name;
        readonly Type _relatedTo;
        readonly RelationshipType _type;
        RelationshipOptions _options = RelationshipOptions.Default;
        string _field;
        UriTemplateBuilder<T> _uriTemplateBuilder;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The parent builder.</param>
        /// <param name="name">The name of the relationship.</param>
        /// <param name="relatedTo">The type of the model that this relationship is related to.</param>
        /// <param name="type">The relationship type.</param>
        internal RelationshipBuilder(IContractBuilder<T> builder, string name, Type relatedTo, RelationshipType type)
        {
            _builder = builder;
            _name = name;
            _relatedTo = relatedTo;
            _type = type;
        }

        /// <summary>
        /// Create the runtime relationship.
        /// </summary>
        /// <param name="fields">The list of available fields to map to.</param>
        /// <returns>The runtime relationship proxy class.</returns>
        internal RuntimeRelationship CreateRuntimeRelationship(IReadOnlyList<IField> fields)
        {
            var field = _name == null
                ? null
                : fields.SingleOrDefault(f => String.Equals(f.Name, _name, StringComparison.OrdinalIgnoreCase));

            var viaField = _field == null 
                ? null
                : fields.SingleOrDefault(f => String.Equals(f.Name, _field, StringComparison.OrdinalIgnoreCase));

            var template = _uriTemplateBuilder != null
                ? _uriTemplateBuilder.CreateTemplate()
                : null;

            return new RuntimeRelationship(_type, _name, _options, _relatedTo, field, viaField, template);
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
        /// Sets the field that the relationship link is stored through.
        /// </summary>
        /// <param name="field">The field that links the relationship.</param>
        /// <returns>The relationship builder build the relationship.</returns>
        public RelationshipBuilder<T> Via(string field)
        {
            _field = field;

            return this;
        }

        /// <summary>
        /// Sets the given options for the relationship.
        /// </summary>
        /// <param name="options">The list of options to set.</param>
        /// <param name="setOptionOn">true if the options are to be set, false if not.</param>
        /// <returns>The relationship builder to continue building on.</returns>
        public RelationshipBuilder<T> Options(RelationshipOptions options, bool setOptionOn = true)
        {
            if (setOptionOn)
            {
                _options |= options;
            }
            else
            {
                _options &= ~(options);
            }

            return this;
        }

        /// <summary>
        /// Defines the relationship as being readonly.
        /// </summary>
        /// <returns>The relationship builder to continue building on.</returns>
        public RelationshipBuilder<T> Embedded()
        {
            return Options(RelationshipOptions.Embedded);
        }

        /// <summary>
        /// Defines the relationship as being readonly.
        /// </summary>
        /// <returns>The relationship builder to continue building on.</returns>
        public RelationshipBuilder<T> ReadOnly()
        {
            if (_name != null)
            {
                _builder.Field(_name).ReadOnly();
            }

            return this;
        }

        /// <summary>
        /// Defines the relationship as being write-only.
        /// </summary>
        /// <returns>The relationship builder to continue building on.</returns>
        public RelationshipBuilder<T> WriteOnly()
        {
            if (_name != null)
            {
                _builder.Field(_name).WriteOnly();
            }

            return this;
        }

        /// <summary>
        /// Returns a template builder for the relationship.
        /// </summary>
        /// <param name="format">The format of the template.</param>
        /// <returns>The template builder instance.</returns>
        public UriTemplateBuilder<T> Template(string format)
        {
            _uriTemplateBuilder = new UriTemplateBuilder<T>(_builder, format);

            return _uriTemplateBuilder;
        }

        /// <summary>
        /// Returns a template builder for the relationship.
        /// </summary>
        /// <param name="format">The format of the template.</param>
        /// <param name="parameter">The name of a single parameter.</param>
        /// <param name="selector">The selector for the given parameter name.</param>
        /// <returns>The template builder instance.</returns>
        public UriTemplateBuilder<T> Template(string format, string parameter, Func<T, object> selector)
        {
            return Template(format).Parameter(parameter, selector);
        }

        /// <summary>
        /// Gets the name of relationship.
        /// </summary>
        internal string Name
        {
            get { return _name; }
        }
    }
}