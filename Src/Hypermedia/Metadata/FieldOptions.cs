using System;

namespace Hypermedia.Metadata
{
    [Flags]
    public enum FieldOptions
    {
        /// <summary>
        /// No options defined.
        /// </summary>
        None = 0,

        /// <summary>
        /// The field is the primary identifier.
        /// </summary>
        Id = 0x01,

        /// <summary>
        /// The field should serialize.
        /// </summary>
        Serializable = 0x02,

        /// <summary>
        /// The field should deserialize.
        /// </summary>
        Deserializable = 0x04,

        /// <summary>
        /// The field is a realtionship.
        /// </summary>
        Relationship = 0x08,

        /// <summary>
        /// The relationship should be embedded when serialized.
        /// </summary>
        SerializeAsEmbedded = 0x10,

        /// <summary>
        /// The relationship should be embedded when deserialized.
        /// </summary>
        DeserializeAsEmbedded = 0x20,

        /// <summary>
        /// This field is used as a backing field for a BelongsTo relationship.
        /// </summary>
        BackingField = 0x40,
    }
}