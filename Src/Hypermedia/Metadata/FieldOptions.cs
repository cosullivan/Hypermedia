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
        CanSerialize = 0x02,

        /// <summary>
        /// The field should deserialize.
        /// </summary>
        CanDeserialize = 0x04,

        /// <summary>
        /// The default set of options.
        /// </summary>
        Default = CanSerialize | CanDeserialize
    }
}