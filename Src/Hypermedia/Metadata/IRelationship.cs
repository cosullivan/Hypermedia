using System;

namespace Hypermedia.Metadata
{
    public interface IRelationship : IMember
    {
        /// <summary>
        /// Gets the relationship type.
        /// </summary>
        RelationshipType Type { get; }

        /// <summary>
        /// Gets the URI template that defines the location of the relationship.
        /// </summary>
        UriTemplate UriTemplate { get; }

        /// <summary>
        /// Gets the entity type that the relationship is related to.
        /// </summary>
        Type RelatedTo { get; }

        /// <summary>
        /// Gets the field that the relationship is linked to.
        /// </summary>
        IField Field { get; }
    }
}