namespace Hypermedia.Metadata
{
    public interface IBelongsToRelationship : IRelationship
    {
        /// <summary>
        /// Gets the field that the relationship is linked via.
        /// </summary>
        IField BackingField { get; }
    }
}