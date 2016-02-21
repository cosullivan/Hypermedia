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
    }
}