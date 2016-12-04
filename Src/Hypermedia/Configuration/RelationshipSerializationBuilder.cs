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
        /// Ignore the relationship when serializing.
        /// </summary>
        /// <returns>The builder to continue building on.</returns>
        public RelationshipSerializationBuilder<T> Ignore()
        {
            Options(FieldOptions.Serializable, false);

            return this;
        }

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
}