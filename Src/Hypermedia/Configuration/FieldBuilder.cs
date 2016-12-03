using Hypermedia.Metadata;
using Hypermedia.Metadata.Runtime;

namespace Hypermedia.Configuration
{
    public sealed class FieldBuilder<T> : IContractBuilder<T>
    {
        readonly IContractBuilder<T> _builder;
        readonly RuntimeField _field;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The parent builder.</param>
        /// <param name="field">The field to build on.</param>
        internal FieldBuilder(IContractBuilder<T> builder, RuntimeField field)
        {
            _builder = builder;
            _field = field;

            if (_field.Accessor == null)
            {
                _field.Accessor = RuntimeFieldAccessor.From<T>(_field.Name);
            }
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
        /// Sets the field accessor for the given field.
        /// </summary>
        /// <param name="accessor">The field accessor for the given field.</param>
        /// <returns>The field builder to continue building on.</returns>
        public FieldBuilder<T> Accessor(IFieldAccessor accessor)
        {
            _field.Accessor = accessor;

            if (_field.Accessor.CanRead)
            {
                Options(FieldOptions.Serializable);
            }

            if (_field.Accessor.CanWrite)
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
        public FieldBuilder<T> From(string property)
        {
            _field.Accessor = RuntimeFieldAccessor.From<T>(property);

            return this;
        }

        /// <summary>
        /// Renames the field.
        /// </summary>
        /// <param name="name">The new name to apply to the field.</param>
        /// <returns>The field builder to continue building on.</returns>
        /// <remarks>If the mapping property has not been set, the current name is set
        /// as the mapping property and then the new name is applied.</remarks>
        public FieldBuilder<T> Rename(string name)
        {
            if (_field.Accessor == null)
            {
                Accessor(RuntimeFieldAccessor.From<T>(_field.Name));
            }

            _field.Name = name;

            return this;
        }

        /// <summary>
        /// Sets the given options for the field.
        /// </summary>
        /// <param name="options">The list of options to set.</param>
        /// <param name="setOptionOn">true if the options are to be set, false if not.</param>
        /// <returns>The field builder to continue building on.</returns>
        internal FieldBuilder<T> Options(FieldOptions options, bool setOptionOn = true)
        {
            if (setOptionOn)
            {
                _field.Options |= options;
            }
            else
            {
                _field.Options &= ~(options);
            }

            return this;
        }

        /// <summary>
        /// Defines whether the given field is the primary ID field.
        /// </summary>
        /// <param name="value">true if the field is the primary ID field, false if not.</param>
        /// <returns>The field builder to continue building on.</returns>
        public FieldBuilder<T> Id(bool value = true)
        {
            return Options(FieldOptions.Id, value);
        }

        /// <summary>
        /// Defines whether the given field is to be ignored.
        /// </summary>
        /// <param name="value">true to ignore the field, false if not.</param>
        /// <returns>The field builder to continue building on.</returns>
        public FieldBuilder<T> Ignore(bool value = true)
        {
            return Options(FieldOptions.Serializable, value == false).Options(FieldOptions.Deserializable, value == false);
        }

        /// <summary>
        /// Defines the field as being readonly.
        /// </summary>
        /// <returns>The field builder to continue building on.</returns>
        public FieldBuilder<T> ReadOnly()
        {
            return Options(FieldOptions.Serializable, true).Options(FieldOptions.Deserializable, false);
        }

        /// <summary>
        /// Defines the field as being write-only.
        /// </summary>
        /// <returns>The field builder to continue building on.</returns>
        public FieldBuilder<T> WriteOnly()
        {
            return Options(FieldOptions.Serializable, false).Options(FieldOptions.Deserializable, true);
        }
    }
}