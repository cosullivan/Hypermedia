using System;

namespace Hypermedia.Metadata.Runtime
{
    public sealed class RuntimeRelationship : IRelationship
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The relationship type.</param>
        /// <param name="name">The name of the relationship.</param>
        /// <param name="options">The relationship options.</param>
        /// <param name="relatedTo">The entity type that the relationship is related to.</param>
        /// <param name="field">The field that the relationship is linked to.</param>
        /// <param name="viaField">The field that the relationship is linked via.</param>
        /// <param name="uriTemplate">The URI template that represents the link to the relationship.</param>
        internal RuntimeRelationship(RelationshipType type, string name, RelationshipOptions options, Type relatedTo, IField field, IField viaField, UriTemplate uriTemplate)
        {
            Type = type;
            Name = name;
            Options = options;
            RelatedTo = relatedTo;
            Field = field;
            ViaField = viaField;
            UriTemplate = uriTemplate;
        }

        /// <summary>
        /// Gets the relationship type.
        /// </summary>
        public RelationshipType Type { get; }

        /// <summary>
        /// Gets the list of options for the relationship.
        /// </summary>
        public RelationshipOptions Options { get; }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the URI template that defines the location of the relationship.
        /// </summary>
        public UriTemplate UriTemplate { get; }

        /// <summary>
        /// Gets the entity type that the relationship is related to.
        /// </summary>
        public Type RelatedTo { get; }

        /// <summary>
        /// Gets the field that the relationship is linked to.
        /// </summary>
        public IField Field { get; }

        /// <summary>
        /// Gets the field that the relationship is linked via.
        /// </summary>
        public IField ViaField { get; }
    }
}