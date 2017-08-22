using System;

namespace Hypermedia.Metadata
{
    public interface IRelationship : IField
    {
        /// <summary>
        /// Gets the relationship type.
        /// </summary>
        RelationshipType Type { get; }

        /// <summary>
        /// A predicate to determine if the relationship exists.
        /// </summary>
        Func<object, bool> Exists { get; }

        /// <summary>
        /// Gets the URI template that defines the location of the relationship.
        /// </summary>
        UriTemplate UriTemplate { get; }

        /// <summary>
        /// Gets the entity type that the relationship is related to.
        /// </summary>
        Type RelatedTo { get; }
    }
}