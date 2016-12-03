using System;

namespace Hypermedia.Metadata.Runtime
{
    internal sealed class RuntimeRelationship : RuntimeField
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        /// <param name="clrType">The CLR type for the field.</param>
        /// <param name="accessor">The field accessor.</param>
        /// <param name="options">The relationship options.</param>
        /// <param name="type">The relationship type.</param>
        /// <param name="relatedTo">The entity type that the relationship is related to.</param>
        /// <param name="viaField">The field that the relationship is linked via.</param>
        /// <param name="uriTemplate">The URI template that represents the link to the relationship.</param>
        internal RuntimeRelationship(
            string name, 
            Type clrType, 
            IFieldAccessor accessor, 
            FieldOptions options,
            RelationshipType type, 
            Type relatedTo, 
            IField viaField, 
            UriTemplate uriTemplate) : base(name, clrType, accessor, options)
        {
            Type = type;
            RelatedTo = relatedTo;
            ViaField = viaField;
            UriTemplate = uriTemplate;
        }

        /// <summary>
        /// Gets the relationship type.
        /// </summary>
        public RelationshipType Type { get; }
        
        /// <summary>
        /// Gets the URI template that defines the location of the relationship.
        /// </summary>
        public UriTemplate UriTemplate { get; }

        /// <summary>
        /// Gets the entity type that the relationship is related to.
        /// </summary>
        public Type RelatedTo { get; }

        /// <summary>
        /// Gets the field that the relationship is linked via.
        /// </summary>
        public IField ViaField { get; }
    }
}