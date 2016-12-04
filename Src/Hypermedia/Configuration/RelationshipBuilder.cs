using System;
using Hypermedia.Metadata;
using Hypermedia.Metadata.Runtime;

namespace Hypermedia.Configuration
{
    public sealed class RelationshipSerializationBuilder<T> : RelationshipBuilder<T>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The parent builder.</param>
        /// <param name="relationship">The relationship to build on.</param>
        internal RelationshipSerializationBuilder(RelationshipBuilder<T> builder, RuntimeRelationship relationship) : base(builder, relationship) { }

        /// <summary>
        /// Serialize the relationship as an embedded item.
        /// </summary>
        /// <returns>The builder to continue building on.</returns>
        public RelationshipSerializationBuilder<T> Embedded()
        {
            Options(FieldOptions.SerializeAsEmbedded);

            return this;
        }
    }

    public sealed class RelationshipDeserializationBuilder<T> : RelationshipBuilder<T>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The parent builder.</param>
        /// <param name="relationship">The relationship to build on.</param>
        internal RelationshipDeserializationBuilder(RelationshipBuilder<T> builder, RuntimeRelationship relationship) : base(builder, relationship) { }

        /// <summary>
        /// Serialize the relationship as an embedded item.
        /// </summary>
        /// <returns>The builder to continue building on.</returns>
        public RelationshipDeserializationBuilder<T> Embedded()
        {
            Options(FieldOptions.DeserializeAsEmbedded);

            return this;
        }

        /// <summary>
        /// Sets the field that the relationship link is stored through.
        /// </summary>
        /// <param name="field">The field that links the relationship.</param>
        /// <returns>The relationship builder build the relationship.</returns>
        public RelationshipDeserializationBuilder<T> BackingField(string field)
        {
            //_builder.Field(field);
            //_field = field;

            return this;
        }
    }

    public class RelationshipBuilder<T> : DelegatingContractBuilder<T>
    {
        readonly RuntimeRelationship _relationship;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The parent builder.</param>
        /// <param name="relationship">The relationship to build on.</param>
        internal RelationshipBuilder(IContractBuilder<T> builder, RuntimeRelationship relationship) : base(builder)
        {
            _relationship = relationship;
        }

        /// <summary>
        /// Sets the field accessor for the given field.
        /// </summary>
        /// <param name="accessor">The field accessor for the given field.</param>
        /// <returns>The builder to continue building on.</returns>
        public RelationshipBuilder<T> Accessor(IFieldAccessor accessor)
        {
            _relationship.Accessor = accessor;

            if (_relationship.Accessor.CanRead)
            {
                Options(FieldOptions.Serializable);
            }

            if (_relationship.Accessor.CanWrite)
            {
                Options(FieldOptions.Deserializable);
            }

            return this;
        }

        /// <summary>
        /// Sets the property that the field is to be mapped to.
        /// </summary>
        /// <param name="property">The name of the property that the field is mapped to.</param>
        /// <returns>The field builder to continue building on.</returns>
        public RelationshipBuilder<T> From(string property)
        {
            _relationship.Accessor = RuntimeFieldAccessor.From<T>(property);

            return this;
        }

        /// <summary>
        /// Sets the given options for the field.
        /// </summary>
        /// <param name="options">The list of options to set.</param>
        /// <param name="setOptionOn">true if the options are to be set, false if not.</param>
        /// <returns>The field builder to continue building on.</returns>
        protected RelationshipBuilder<T> Options(FieldOptions options, bool setOptionOn = true)
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
        /// Returns a serialization builder to provide fine grain control over serialization.
        /// </summary>
        /// <returns>The relationship builder to continue building on.</returns>
        public RelationshipSerializationBuilder<T> Serialization()
        {
            return new RelationshipSerializationBuilder<T>(this, _relationship);
        }

        /// <summary>
        /// Returns a deserialization builder to provide fine grain control over deserialization.
        /// </summary>
        /// <returns>The relationship builder to continue building on.</returns>
        public RelationshipDeserializationBuilder<T> Deserialization()
        {
            return new RelationshipDeserializationBuilder<T>(this, _relationship);
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

            return new UriTemplateBuilder<T>(Builder, _relationship.UriTemplate);
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