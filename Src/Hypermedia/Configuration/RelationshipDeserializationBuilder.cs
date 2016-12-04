using Hypermedia.Metadata;
using Hypermedia.Metadata.Runtime;

namespace Hypermedia.Configuration
{
    public sealed class RelationshipDeserializationBuilder<T> : RelationshipBuilder<T>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The parent builder.</param>
        /// <param name="relationship">The relationship to build on.</param>
        internal RelationshipDeserializationBuilder(RelationshipBuilder<T> builder, RuntimeRelationship relationship) : base(builder, relationship) { }

        /// <summary>
        /// Ignore the relationship when deserializing.
        /// </summary>
        /// <returns>The builder to continue building on.</returns>
        public RelationshipDeserializationBuilder<T> Ignore()
        {
            Options(FieldOptions.Deserializable, false);

            return this;
        }

        /// <summary>
        /// Serialize the relationship as an embedded item.
        /// </summary>
        /// <returns>The builder to continue building on.</returns>
        public RelationshipDeserializationBuilder<T> Embedded()
        {
            Options(FieldOptions.DeserializeAsEmbedded);

            return this;
        }
    }
}