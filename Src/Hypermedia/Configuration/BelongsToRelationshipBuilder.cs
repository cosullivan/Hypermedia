using Hypermedia.Metadata.Runtime;

namespace Hypermedia.Configuration
{
    public class BelongsToRelationshipBuilder<T> : RelationshipBuilder<T>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The parent builder.</param>
        /// <param name="relationship">The relationship to build on.</param>
        internal BelongsToRelationshipBuilder(IContractBuilder<T> builder, RuntimeRelationship relationship) : base(builder, relationship) { }

        /// <summary>
        /// Sets the field that the relationship link is stored through.
        /// </summary>
        /// <param name="field">The field that links the relationship.</param>
        /// <returns>The relationship builder build the relationship.</returns>
        public BelongsToRelationshipBuilder<T> BackingField(string field)
        {
            var builder = Builder.Field(field);

            Instance.BackingField = builder.Instance;

            return this;
        }
    }
}