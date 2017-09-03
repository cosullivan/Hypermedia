using System;

namespace Hypermedia.Metadata.Runtime
{
    internal abstract class RuntimeRelationship : RuntimeField, IRelationship
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contract">The contract that the field belongs to.</param>
        /// <param name="name">The name of the relationship.</param>
        protected RuntimeRelationship(IContract contract, string name) : base(contract, name)
        {
            Options = FieldOptions.Relationship;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="field">The field to initialize the relationship from.</param>
        protected RuntimeRelationship(RuntimeField field) : this(field.Contract, field.Name)
        {
            Options = field.Options | FieldOptions.Relationship;
            Accessor = field.Accessor;
        }

        /// <summary>
        /// Gets the relationship type.
        /// </summary>
        public abstract RelationshipType Type { get; }

        /// <summary>
        /// A predicate to determine if the relationship exists.
        /// </summary>
        public Func<object, bool> Exists { get; internal set; }

        /// <summary>
        /// Gets the URI template that defines the location of the relationship.
        /// </summary>
        public UriTemplate UriTemplate { get; internal set; }

        /// <summary>
        /// Gets the entity type that the relationship is related to.
        /// </summary>
        public Type RelatedTo { get; internal set; }

        /// <summary>
        /// The name of the relationship on the other side.
        /// </summary>
        public string InverseName { get; internal set; }
    }
}