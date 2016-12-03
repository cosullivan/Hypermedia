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
        /// <param name="name">The name of the relationship.</param>
        internal RuntimeRelationship(RelationshipType type, string name)
        {
            Type = type;
            Name = name;
            Options = FieldOptions.Relationship;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The relationship type.</param>
        /// <param name="field">The field to initialize the relationship from.</param>
        internal RuntimeRelationship(RelationshipType type, RuntimeField field)
        {
            Type = type;
            Name = field.Name;
            Options = field.Options | FieldOptions.Relationship;
            Accessor = field.Accessor;
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
        public IField BackingField { get; internal set; }
    }
}