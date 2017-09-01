using System;
using Hypermedia.Metadata;
using Hypermedia.Metadata.Runtime;

namespace Hypermedia.Configuration
{
    public abstract class RelationshipBuilder<T> : DelegatingContractBuilder<T>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The parent builder.</param>
        /// <param name="relationship">The relationship to build on.</param>
        internal RelationshipBuilder(IContractBuilder<T> builder, RuntimeRelationship relationship) : base(builder)
        {
            Instance = relationship;
        }

        /// <summary>
        /// Sets the field accessor for the given field.
        /// </summary>
        /// <param name="accessor">The field accessor for the given field.</param>
        /// <returns>The builder to continue building on.</returns>
        public RelationshipBuilder<T> Accessor(IFieldAccessor accessor)
        {
            Instance.Accessor = accessor;

            if (Instance.Accessor.CanRead)
            {
                Options(FieldOptions.Serializable);
            }

            if (Instance.Accessor.CanWrite)
            {
                Options(FieldOptions.Deserializable);
            }

            return this;
        }

        /// <summary>
        /// Sets the property that the field is to be mapped to.
        /// </summary>
        /// <param name="property">The name of the property that the field is mapped to.</param>
        /// <returns>The relationship builder to continue building on.</returns>
        public RelationshipBuilder<T> From(string property)
        {
            Instance.Accessor = RuntimeFieldAccessor.From<T>(property);

            return this;
        }

        /// <summary>
        /// Sets the given options for the field.
        /// </summary>
        /// <param name="options">The list of options to set.</param>
        /// <param name="setOptionOn">true if the options are to be set, false if not.</param>
        /// <returns>The relationship builder to continue building on.</returns>
        protected RelationshipBuilder<T> Options(FieldOptions options, bool setOptionOn = true)
        {
            if (setOptionOn)
            {
                Instance.Options |= options;
            }
            else
            {
                Instance.Options &= ~(options);
            }

            return this;
        }

        /// <summary>
        /// Returns a serialization builder to provide fine grain control over serialization.
        /// </summary>
        /// <returns>The relationship builder to continue building on.</returns>
        public RelationshipSerializationBuilder<T> Serialization()
        {
            return new RelationshipSerializationBuilder<T>(this, Instance);
        }

        /// <summary>
        /// Returns a deserialization builder to provide fine grain control over deserialization.
        /// </summary>
        /// <returns>The relationship builder to continue building on.</returns>
        public RelationshipDeserializationBuilder<T> Deserialization()
        {
            return new RelationshipDeserializationBuilder<T>(this, Instance);
        }

        /// <summary>
        /// Sets the name of the member on the inverse relationship.
        /// </summary>
        /// <param name="name">The name of the member for the inverse.</param>
        /// <returns>The relationship builder to continue building on.</returns>
        public RelationshipBuilder<T> Inverse(string name)
        {
            Instance.InverseName = name;

            return this;
        }

        /// <summary>
        /// Returns a template builder for the relationship.
        /// </summary>
        /// <param name="format">The format of the template.</param>
        /// <returns>The template builder instance.</returns>
        public UriTemplateBuilder<T> Template(string format)
        {
            Instance.UriTemplate = new UriTemplate(format);

            return new UriTemplateBuilder<T>(Builder, Instance.UriTemplate);
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
        /// The instance that is being built upon.
        /// </summary>
        internal RuntimeRelationship Instance { get; }
    }
}