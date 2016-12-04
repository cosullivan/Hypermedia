using Hypermedia.Metadata.Runtime;

namespace Hypermedia.Configuration
{
    public class HasManyRelationshipBuilder<T> : RelationshipBuilder<T>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The parent builder.</param>
        /// <param name="relationship">The relationship to build on.</param>
        internal HasManyRelationshipBuilder(IContractBuilder<T> builder, RuntimeRelationship relationship) : base(builder, relationship) { }
    }
}