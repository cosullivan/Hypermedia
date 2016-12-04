using System.Diagnostics;

namespace Hypermedia.Metadata.Runtime
{
    [DebuggerDisplay("{Name}")]
    internal class RuntimeHasManyRelationship : RuntimeRelationship, IHasManyRelationship
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        internal RuntimeHasManyRelationship(string name) : base(name) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="field">The field to initialize the relationship from.</param>
        internal RuntimeHasManyRelationship(RuntimeField field) : base(field) { }

        /// <summary>
        /// Gets the relationship type.
        /// </summary>
        public override RelationshipType Type
        {
            get { return RelationshipType.HasMany; }
        }
    }
}