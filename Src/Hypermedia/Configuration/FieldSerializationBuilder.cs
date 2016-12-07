using Hypermedia.Metadata;
using Hypermedia.Metadata.Runtime;

namespace Hypermedia.Configuration
{
    public sealed class FieldSerializationBuilder<T> : FieldBuilder<T>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The parent builder.</param>
        /// <param name="field">The field to build on.</param>
        internal FieldSerializationBuilder(FieldBuilder<T> builder, RuntimeField field) : base(builder, field) { }

        /// <summary>
        /// Include the field for serialization.
        /// </summary>
        /// <returns>The builder to continue building on.</returns>
        public FieldSerializationBuilder<T> Include()
        {
            Options(FieldOptions.Serializable, true);

            return this;
        }

        /// <summary>
        /// Ignore the field when serializing.
        /// </summary>
        /// <returns>The builder to continue building on.</returns>
        public FieldSerializationBuilder<T> Ignore()
        {
            Options(FieldOptions.Serializable, false);

            return this;
        }
    }
}