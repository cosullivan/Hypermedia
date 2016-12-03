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
        /// Gets the list of options for the relationship.
        /// </summary>
        RelationshipOptions Options { get; }

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

        /// <summary>
        /// Gets the field that the relationship is linked via.
        /// </summary>
        IField ViaField { get; }
    }

    public static class RelationshipExtensions
    {
        /// <summary>
        /// Gets the value from the given entity.
        /// </summary>
        /// <param name="relationship">The relationship to return the value from.</param>
        /// <param name="entity">The entity to retur the relationship value from.</param>
        /// <returns>The value that represents the relationship, be it a linking key or an instance of the actual related item.</returns>
        public static object GetValue(this IRelationship relationship, object entity)
        {
            if (relationship.ViaField != null)
            {
                return relationship.ViaField.GetValue(entity);
            }

            return relationship.Field?.GetValue(entity);
        }

        /// <summary>
        /// Returns a value indicating whether or not the relationship adheres to the list of specified options.
        /// </summary>
        /// <param name="relationship">The relationship to test the options against.</param>
        /// <param name="options">The list of options to test on the relationship.</param>
        /// <returns>true if the relationship contains the list of options, false if not.</returns>
        public static bool Is(this IRelationship relationship, RelationshipOptions options)
        {
            if (relationship == null)
            {
                throw new ArgumentNullException(nameof(relationship));
            }

            return (relationship.Options & options) == options;
        }
    }
}