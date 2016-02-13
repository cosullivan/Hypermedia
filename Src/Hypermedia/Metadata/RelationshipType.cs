namespace Hypermedia.Metadata
{
    public enum RelationshipType
    {
        /// <summary>
        /// The relationship is modelled as a parent with zero to many children.
        /// </summary>
        HasMany,

        /// <summary>
        /// The relationship is modeled as the inverse of the HasMany relationship.
        /// </summary>
        BelongsTo
    }
}