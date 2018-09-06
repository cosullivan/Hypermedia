using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Hypermedia.Configuration;

namespace Hypermedia.Metadata.Runtime
{
    [DebuggerDisplay("Name={Name} Type={ClrType}")]
    public class RuntimeContract : IContract
    {
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
        internal RuntimeContract() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The name of the entity type.</param>
        /// <param name="clrType">The CLR type that the entity type is mapped to.</param>
        /// <param name="fields">The list of fields for the entity.</param>
        internal RuntimeContract(string type, Type clrType, IReadOnlyList<IField> fields)
        {
            Name = type;
            ClrType = clrType;
            Fields = fields;
        }

        /// <summary>
        /// Create a default runtime type from the given entity type.
        /// </summary>
        /// <param name="type">The CLR type that the entity is mapped to.</param>
        /// <returns>The entity type that represents a default configuration of the given entity type.</returns>
        public static IContract CreateRuntimeType(Type type)
        {
            var name = Inflector.Pluralize(type.Name.ToLower());

            return new RuntimeContract(name, type, CreateRuntimeFields(type, FieldDiscovery.Shallow));
        }

        /// <summary>
        /// Create a default runtime type from the given entity type.
        /// </summary>
        /// <param name="type">The CLR type that the entity is mapped to.</param>
        /// <param name="fieldDiscovery">The field discovery instance to use for determining what fields are available.</param>
        /// <returns>The entity type that represents a default configuration of the given entity type.</returns>
        public static IContract CreateRuntimeType(Type type, IFieldDiscovery fieldDiscovery)
        {
            var name = Inflector.Pluralize(type.Name.ToLower());

            return new RuntimeContract(name, type, CreateRuntimeFields(type, fieldDiscovery));
        }

        /// <summary>
        /// Create a default runtime type from the given entity.
        /// </summary>
        /// <param name="type">The CLR type that the entity is mapped to.</param>
        /// <param name="name">The name of the runtime type.</param>
        /// <returns>The entity type that represents a default configuration of the given entity type.</returns>
        public static IContract CreateRuntimeType(Type type, string name)
        {
            return new RuntimeContract(name, type, CreateRuntimeFields(type, FieldDiscovery.Shallow));
        }

        /// <summary>
        /// Create a default runtime type from the given entity.
        /// </summary>
        /// <param name="type">The CLR type that the entity is mapped to.</param>
        /// <param name="name">The name of the runtime type.</param>
        /// <param name="fieldDiscovery">The field discovery instance to use for determining what fields are available.</param>
        /// <returns>The entity type that represents a default configuration of the given entity type.</returns>
        public static IContract CreateRuntimeType(Type type, string name, IFieldDiscovery fieldDiscovery)
        {
            return new RuntimeContract(name, type, CreateRuntimeFields(type, fieldDiscovery));
        }

        /// <summary>
        /// Creates the runtime fields for a given type.
        /// </summary>
        /// <param name="type">The type to create the runtime fields for.</param>
        /// <param name="fieldDiscovery">The field discovery instance to use for determining what fields are available.</param>
        /// <returns>The list of runtime fields for the given type.</returns>
        internal static IReadOnlyList<RuntimeField> CreateRuntimeFields(Type type, IFieldDiscovery fieldDiscovery)
        {
            return fieldDiscovery.Discover(type.GetTypeInfo()).Select(RuntimeField.CreateRuntimeField).ToList();
        }

        /// <summary>
        /// Gets the name of the metadata model.
        /// </summary>
        public string Name { get; internal set; }
        
        /// <summary>
        /// Gets the CLR type that the metadata maps to.
        /// </summary>
        public Type ClrType { get; internal set; }

        /// <summary>
        /// Gets a list of the fields that are available on the model.
        /// </summary>
        public IReadOnlyList<IField> Fields { get; internal set; }
        
        /// <summary>
        /// Gets or sets the inflector to use when creating contracts at runtime.
        /// </summary>
        public static IResourceInflector Inflector { get; set; }
    }

    public sealed class RuntimeContract<T> : RuntimeContract
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The name of the entity type.</param>
        /// <param name="fields">The list of fields for the entity.</param>
        internal RuntimeContract(string type, IReadOnlyList<RuntimeField<T>> fields) : base(type, typeof(T), fields) { }

        /// <summary>
        /// Create a default runtime type from the given entity.
        /// </summary>
        /// <returns>The entity type that represents a default configuration of the given entity type.</returns>
        public static IContract CreateRuntimeType()
        {
            var name = Inflector.Pluralize(typeof(T).Name.ToLower());

            return new RuntimeContract<T>(name, CreateRuntimeFields());
        }

        /// <summary>
        /// Create a default runtime type from the given entity.
        /// </summary>
        /// <param name="name">The name of the runtime type.</param>
        /// <returns>The entity type that represents a default configuration of the given entity type.</returns>
        public static IContract CreateRuntimeType(string name)
        {
            return new RuntimeContract<T>(name, CreateRuntimeFields());
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