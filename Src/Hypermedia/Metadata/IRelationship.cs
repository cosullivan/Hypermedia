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

        /// <summary>
        /// The name of the relationship on the other side.
        /// </summary>
        string InverseName { get; }
    }

    public static class RelationshipExtensions
    {
        /// <summary>
        /// Resolve the inverse relationship.
        /// </summary>
        /// <param name="relationship">The relationship to resolve the inverse from.</param>
        /// <param name="contractResolver">The contract resolver that contains the nessessary contracts for resolution.</param>
        /// <returns>The inverse relationship.</returns>
        public static IRelationship Inverse(this IRelationship relationship, IContractResolver contractResolver)
        {
            if (relationship == null)
            {
                throw new ArgumentNullException(nameof(relationship));
            }

            if (String.IsNullOrWhiteSpace(relationship.InverseName) || contractResolver.TryResolve(relationship.RelatedTo, out IContract other) == false)
            {
                return null;
            }

            return other.Relationship(relationship.InverseName);
        }
    }
}