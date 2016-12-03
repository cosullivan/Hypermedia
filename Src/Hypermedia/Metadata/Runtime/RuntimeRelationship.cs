using System;
using System.Diagnostics;

namespace Hypermedia.Metadata.Runtime
{
    [DebuggerDisplay("{Name}")]
    internal class RuntimeRelationship : RuntimeField, IRelationship
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The relationship type.</param>
        internal RuntimeRelationship(RelationshipType type)
        {
            Type = type;
        }

        /// <summary>
        /// Gets the relationship type.
        /// </summary>
        public RelationshipType Type { get; }
        
        /// <summary>
        /// Gets the URI template that defines the location of the relationship.
        /// </summary>
        public UriTemplate UriTemplate { get; internal set; }

        /// <summary>
        /// Gets the entity type that the relationship is related to.
        /// </summary>
        public Type RelatedTo { get; internal set; }

        /// <summary>
        /// Gets the field that the relationship is linked via.
        /// </summary>
        public IField ViaField { get; internal set; }
    }
}