using System;

namespace Hypermedia.Metadata
{
    [Flags]
    public enum RelationshipOptions
    {
        /// <summary>
        /// No options defined.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// The relationship should be embedded.
        /// </summary>
        Embedded = 0x01,

        /// <summary>
        /// The default options for a relationship.
        /// </summary>
        Default = None
    }
}