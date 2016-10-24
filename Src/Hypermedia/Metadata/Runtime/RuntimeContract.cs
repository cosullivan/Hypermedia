using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Hypermedia.Metadata.Runtime
{
    [DebuggerDisplay("Name={Name} Type={ClrType}")]
    public class RuntimeContract : IContract
    {
        readonly string _type;
        readonly Type _clrType;
        readonly IReadOnlyList<IField> _fields;
        readonly IReadOnlyList<IRelationship> _relationships;

        /// <summary>
        /// Constructor.
        /// </summary>
        static RuntimeContract()
        {
            Inflector = new ResourceInflector();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The name of the entity type.</param>
        /// <param name="clrType">The CLR type that the entity type is mapped to.</param>
        /// <param name="fields">The list of fields for the entity.</param>
        /// <param name="relationships">The list of relationships for the entity.</param>
        internal RuntimeContract(string type, Type clrType, IReadOnlyList<IField> fields, IReadOnlyList<IRelationship> relationships)
        {
            _type = type;
            _clrType = clrType;
            _fields = fields;
            _relationships = relationships;
        }

        /// <summary>
        /// Create a default runtime type from the given entity type.
        /// </summary>
        /// <param name="type">The CLR type that the entity is mapped to.</param>
        /// <returns>The entity type that represents a default configuration of the given entity type.</returns>
        internal static IContract CreateRuntimeType(Type type)
        {
            var name = Inflector.Pluralize(type.Name.ToLower());

            return new RuntimeContract(name, type, CreateRuntimeFields(type), new List<RuntimeRelationship>());
        }

        /// <summary>
        /// Create a default runtime type from the given entity.
        /// </summary>
        /// <param name="type">The CLR type that the entity is mapped to.</param>
        /// <param name="name">The name of the runtime type.</param>
        /// <returns>The entity type that represents a default configuration of the given entity type.</returns>
        internal static IContract CreateRuntimeType(Type type, string name)
        {
            return new RuntimeContract(name, type, CreateRuntimeFields(type), new List<RuntimeRelationship>());
        }

        /// <summary>
        /// Creates the runtime fields for a given type.
        /// </summary>
        /// <param name="type">The type to create the runtime fields for.</param>
        /// <returns>The list of runtime fields for the given type.</returns>
        internal static IReadOnlyList<RuntimeField> CreateRuntimeFields(Type type)
        {
            return type.GetTypeInfo().DeclaredProperties.Select(RuntimeField.CreateRuntimeField).ToList();
        }

        /// <summary>
        /// Gets the name of the metadata model.
        /// </summary>
        public string Name
        {
            get { return _type; }
        }

        /// <summary>
        /// Gets the CLR type that the metadata maps to.
        /// </summary>
        public Type ClrType
        {
            get { return _clrType; }
        }

        /// <summary>
        /// Gets a list of the fields that are available on the model.
        /// </summary>
        public IReadOnlyList<IField> Fields
        {
            get { return _fields; }
        }

        /// <summary>
        /// Gets a list of relationships that are available on the model.
        /// </summary>
        public IReadOnlyList<IRelationship> Relationships
        {
            get { return _relationships; }
        }

        /// <summary>
        /// Gets or sets the inflector to use when creating contracts at runtime.
        /// </summary>
        public static IResourceInflector Inflector { get; set; }
    }

    internal sealed class RuntimeContract<T> : RuntimeContract
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The name of the entity type.</param>
        /// <param name="fields">The list of fields for the entity.</param>
        /// <param name="relationships">The list of relationships for the entity.</param>
        internal RuntimeContract(string type, IReadOnlyList<RuntimeField<T>> fields, IReadOnlyList<RuntimeRelationship> relationships) : base(type, typeof(T), fields, relationships) { }

        /// <summary>
        /// Create a default runtime type from the given entity.
        /// </summary>
        /// <returns>The entity type that represents a default configuration of the given entity type.</returns>
        internal static IContract CreateRuntimeType()
        {
            var name = Inflector.Pluralize(typeof(T).Name.ToLower());

            return new RuntimeContract<T>(name, CreateRuntimeFields(), new List<RuntimeRelationship>());
        }

        /// <summary>
        /// Create a default runtime type from the given entity.
        /// </summary>
        /// <param name="name">The name of the runtime type.</param>
        /// <returns>The entity type that represents a default configuration of the given entity type.</returns>
        internal static IContract CreateRuntimeType(string name)
        {
            return new RuntimeContract<T>(name, CreateRuntimeFields(), new List<RuntimeRelationship>());
        }

        /// <summary>
        /// Creates the runtime fields for a given type.
        /// </summary>
        /// <returns>The list of runtime fields for the given type.</returns>
        internal static IReadOnlyList<RuntimeField<T>> CreateRuntimeFields()
        {
            return typeof(T).GetTypeInfo().DeclaredProperties.Select(RuntimeField<T>.CreateRuntimeField).ToList();
        }
    }
}
