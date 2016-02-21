using System;

namespace Hypermedia.Metadata.Runtime
{
    public sealed class RuntimeRelationship : IRelationship
    {
        readonly RelationshipType _type;
        readonly string _name;
        readonly IField _field;
        readonly IField _viaField;
        readonly UriTemplate _uriTemplate;
        readonly Type _relatedTo;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The relationship type.</param>
        /// <param name="name">The name of the relationship.</param>
        /// <param name="relatedTo">The entity type that the relationship is related to.</param>
        /// <param name="field">The field that the relationship is linked to.</param>
        /// <param name="viaField">The field that the relationship is linked via.</param>
        /// <param name="uriTemplate">The URI template that represents the link to the relationship.</param>
        internal RuntimeRelationship(RelationshipType type, string name, Type relatedTo, IField field, IField viaField, UriTemplate uriTemplate)
        {
            _type = type;
            _name = name;
            _relatedTo = relatedTo;
            _field = field;
            _viaField = viaField;
            _uriTemplate = uriTemplate;
        }

        /// <summary>
        /// Gets the relationship type.
        /// </summary>
        public RelationshipType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the URI template that defines the location of the relationship.
        /// </summary>
        public UriTemplate UriTemplate
        {
            get { return _uriTemplate; }
        }

        /// <summary>
        /// Gets the entity type that the relationship is related to.
        /// </summary>
        public Type RelatedTo
        {
            get { return _relatedTo; }
        }

        /// <summary>
        /// Gets the field that the relationship is linked to.
        /// </summary>
        public IField Field
        {
            get { return _field; }
        }

        /// <summary>
        /// Gets the field that the relationship is linked via.
        /// </summary>
        public IField ViaField
        {
            get { return _viaField; }
        }
    }
}
