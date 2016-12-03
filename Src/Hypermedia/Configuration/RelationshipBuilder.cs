using System;
using Hypermedia.Metadata;
using Hypermedia.Metadata.Runtime;

namespace Hypermedia.Configuration
{
    public sealed class RelationshipBuilder<T> : IContractBuilder<T>
    {
        readonly IContractBuilder<T> _builder;
        readonly RuntimeRelationship _relationship;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The parent builder.</param>
        /// <param name="relationship">The relationship to build on.</param>
        internal RelationshipBuilder(IContractBuilder<T> builder, RuntimeRelationship relationship)
        {
            _builder = builder;
            _relationship = relationship;
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
        public RelationshipBuilder<T> BackingField(string field)
        {
            //_builder.Field(field);
            //_field = field;

            return this;
        }

        /// <summary>
        /// Sets the given options for the field.
        /// </summary>
        /// <param name="options">The list of options to set.</param>
        /// <param name="setOptionOn">true if the options are to be set, false if not.</param>
        /// <returns>The field builder to continue building on.</returns>
        RelationshipBuilder<T> Options(FieldOptions options, bool setOptionOn = true)
        {
            if (setOptionOn)
            {
                _relationship.Options |= options;
            }
            else
            {
                _relationship.Options &= ~(options);
            }

            return this;
        }

        /// <summary>
        /// Defines the relationship as being readonly.
        /// </summary>
        /// <returns>The relationship builder to continue building on.</returns>
        public RelationshipBuilder<T> Embedded()
        {
            return Options(FieldOptions.Embedded);
        }

        /// <summary>
        /// Defines the relationship as being readonly.
        /// </summary>
        /// <returns>The relationship builder to continue building on.</returns>
        public RelationshipBuilder<T> ReadOnly()
        {
            return Options(FieldOptions.Serializable, true).Options(FieldOptions.Deserializable, false);
        }

        /// <summary>
        /// Defines the relationship as being write-only.
        /// </summary>
        /// <returns>The relationship builder to continue building on.</returns>
        public RelationshipBuilder<T> WriteOnly()
        {
            return Options(FieldOptions.Serializable, false).Options(FieldOptions.Deserializable, true);
        }

        /// <summary>
        /// Returns a template builder for the relationship.
        /// </summary>
        /// <param name="format">The format of the template.</param>
        /// <returns>The template builder instance.</returns>
        public UriTemplateBuilder<T> Template(string format)
        {
            _relationship.UriTemplate = new UriTemplate(format);

            return new UriTemplateBuilder<T>(_builder, _relationship.UriTemplate);
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
    }
}