using Hypermedia.Metadata;
using Hypermedia.Metadata.Runtime;

namespace Hypermedia.Configuration
{
    public sealed class FieldDeserializationBuilder<T> : FieldBuilder<T>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The parent builder.</param>
        /// <param name="field">The field to build on.</param>
        internal FieldDeserializationBuilder(FieldBuilder<T> builder, RuntimeField field) : base(builder, field) { }

        /// <summary>
        /// Include the field for serialization.
        /// </summary>
        /// <returns>The builder to continue building on.</returns>
        public FieldDeserializationBuilder<T> Include()
        {
            Options(FieldOptions.Deserializable, true);

            return this;
        }

        /// <summary>
        /// Ignore the field when deserializing.
        /// </summary>
        /// <returns>The builder to continue building on.</returns>
        public FieldDeserializationBuilder<T> Ignore()
        {
            Options(FieldOptions.Serializable, false);

            return this;
        }
    }
}