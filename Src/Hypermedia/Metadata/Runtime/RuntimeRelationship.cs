using System;
using System.Diagnostics;
using System.Reflection;

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
            Options = FieldOptions.Default | FieldOptions.Relationship;
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
            ClrType = field.ClrType;
            Options = field.Options | FieldOptions.Relationship;
            Accessor = field.Accessor;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The relationship type.</param>
        /// <param name="propertyInfo">The property information to create the field from.</param>
        internal RuntimeRelationship(RelationshipType type, PropertyInfo propertyInfo) : base(propertyInfo)
        {
            Type = type;
        }

        /// <summary>
        /// Creates a runtime relationship field from a property info.
        /// </summary>
        /// <param name="type">The relationship type.</param>
        /// <param name="propertyInfo">The property info to create the runtime relationship field for.</param>
        /// <returns>The runtime relationship field that wraps the given property info.</returns>
        internal static RuntimeRelationship CreateRuntimeField(RelationshipType type, PropertyInfo propertyInfo)
        {
            return new RuntimeRelationship(type, propertyInfo);
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

    internal sealed class RuntimeRelationship<T> : RuntimeRelationship
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The relationship type.</param>
        /// <param name="propertyInfo">The property information to create the field from.</param>
        RuntimeRelationship(RelationshipType type, PropertyInfo propertyInfo) : base(type, propertyInfo) { }

        /// <summary>
        /// Creates a runtime relationship field from a property name.
        /// </summary>
        /// <param name="type">The relationship type.</param>
        /// <param name="name">The name of the relationship field to create from.</param>
        /// <returns>The runtime relationship field that wraps the given property info.</returns>
        internal static RuntimeRelationship<T> CreateRuntimeField(RelationshipType type, string name)
        {
            var property = typeof(T).GetRuntimeProperty(name);

            return CreateRuntimeField(type, property);
        }

        /// <summary>
        /// Creates a runtime relationship field from a property info.
        /// </summary>
        /// <param name="type">The relationship type.</param>
        /// <param name="propertyInfo">The property info to create the runtime relationship field for.</param>
        /// <returns>The runtime relationship field that wraps the given property info.</returns>
        internal new static RuntimeRelationship<T> CreateRuntimeField(RelationshipType type, PropertyInfo propertyInfo)
        {
            return new RuntimeRelationship<T>(type, propertyInfo);
        }
    }
}