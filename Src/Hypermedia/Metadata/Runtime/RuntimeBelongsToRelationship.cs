using System.Diagnostics;

namespace Hypermedia.Metadata.Runtime
{
    [DebuggerDisplay("{Name}")]
    internal class RuntimeBelongsToRelationship : RuntimeRelationship, IBelongsToRelationship
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        internal RuntimeBelongsToRelationship(string name) : base(name) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="field">The field to initialize the relationship from.</param>
        internal RuntimeBelongsToRelationship(RuntimeField field) : base(field) { }

        /// <summary>
        /// Gets the relationship type.
        /// </summary>
        public override RelationshipType Type
        {
            get { return RelationshipType.BelongsTo; }
        }

        /// <summary>
        /// Gets the field that the relationship is linked via.
        /// </summary>
        public IField BackingField { get; internal set; }
    }
}