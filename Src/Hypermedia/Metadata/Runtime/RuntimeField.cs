using System.Diagnostics;
using System.Reflection;

namespace Hypermedia.Metadata.Runtime
{
    [DebuggerDisplay("{Name}")]
    internal class RuntimeField : IField
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        protected RuntimeField() { }

        /// <summary>
        /// The name of the field.
        /// </summary>
        /// <param name="contract">The contract that the field belongs to.</param>
        /// <param name="name">The name of the field.</param>
        internal RuntimeField(IContract contract, string name)
        {
            Contract = contract;
            Name = name;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyInfo">The property information to create the field from.</param>
        protected RuntimeField(PropertyInfo propertyInfo)
        {
            Name = propertyInfo.Name;
            Accessor = new RuntimeFieldAccessor(propertyInfo);
            Options = CreateDefaultOptions(propertyInfo);
        }

        /// <summary>
        /// Creates a runtime field from a property info.
        /// </summary>
        /// <param name="propertyInfo">The property info to create the runtime field for.</param>
        /// <returns>The runtime field that wraps the given property info.</returns>
        internal static RuntimeField CreateRuntimeField(PropertyInfo propertyInfo)
        {
            return new RuntimeField(propertyInfo);
        }

        /// <summary>
        /// Creates the default options for the property info.
        /// </summary>
        /// <param name="propertyInfo">The property info to create the default options for.</param>
        /// <returns>The field options for the given property info.</returns>
        internal static FieldOptions CreateDefaultOptions(PropertyInfo propertyInfo)
        {
            var options = FieldOptions.None;

            if (propertyInfo.CanRead)
            {
                options = options | FieldOptions.Serializable;
            }

            if (propertyInfo.CanWrite)
            {
                options = options | FieldOptions.Deserializable;
            }

            return options;
        }

        /// <summary>
        /// The contract that the field belongs to.
        /// </summary>
        public IContract Contract { get; }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        public string Name { get; internal set; }
        
        /// <summary>
        /// Gets the field accessor.
        /// </summary>
        public IFieldAccessor Accessor { get; internal set; }

        /// <summary>
        /// Gets the list of options for the field.
        /// </summary>
        public FieldOptions Options { get; internal set; }
    }

    internal sealed class RuntimeField<T> : RuntimeField
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyInfo">The property information to create the field from.</param>
        RuntimeField(PropertyInfo propertyInfo) : base(propertyInfo) { }

        /// <summary>
        /// Creates a runtime field from a property name.
        /// </summary>
        /// <param name="name">The name of the field to create from.</param>
        /// <returns>The runtime field that wraps the given property info.</returns>
        internal static RuntimeField<T> CreateRuntimeField(string name)
        {
            var property = typeof(T).GetRuntimeProperty(name);

            return CreateRuntimeField(property);
        }

        /// <summary>
        /// Creates a runtime field from a property info.
        /// </summary>
        /// <param name="propertyInfo">The property info to create the runtime field for.</param>
        /// <returns>The runtime field that wraps the given property info.</returns>
        internal new static RuntimeField<T> CreateRuntimeField(PropertyInfo propertyInfo)
        {
            return new RuntimeField<T>(propertyInfo);
        }
    }
}