using System;
using System.Diagnostics;
using System.Reflection;

namespace Hypermedia.Metadata.Runtime
{
    [DebuggerDisplay("{Name}")]
    internal class RuntimeField : IField
    {
        /// <summary>
        /// Creates a runtime field from a property info.
        /// </summary>
        /// <param name="propertyInfo">The property info to create the runtime field for.</param>
        /// <returns>The runtime field that wraps the given property info.</returns>
        internal static RuntimeField CreateRuntimeField(PropertyInfo propertyInfo)
        {
            return new RuntimeField
            {
                Name = propertyInfo.Name,
                ClrType = propertyInfo.PropertyType,
                Accessor = new RuntimeFieldAccessor(propertyInfo),
                Options = CreateDefaultOptions(propertyInfo)
            };
        }

        /// <summary>
        /// Creates the default options for the property info.
        /// </summary>
        /// <param name="propertyInfo">The property info to create the default options for.</param>
        /// <returns>The field options for the given property info.</returns>
        internal static FieldOptions CreateDefaultOptions(PropertyInfo propertyInfo)
        {
            var options = FieldOptions.None;

            if (propertyInfo.CanWrite == false)
            {
                options = options | FieldOptions.Deserializable;
            }

            return options;
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the CLR type that the member maps to.
        /// </summary>
        public Type ClrType { get; internal set; }
        
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
        /// Creates a runtime field from a property info.
        /// </summary>
        /// <param name="propertyInfo">The property info to create the runtime field for.</param>
        /// <returns>The runtime field that wraps the given property info.</returns>
        internal new static RuntimeField<T> CreateRuntimeField(PropertyInfo propertyInfo)
        {
            return new RuntimeField<T>
            {
                Name = propertyInfo.Name,
                ClrType = propertyInfo.PropertyType,
                Accessor = new RuntimeFieldAccessor(propertyInfo),
                Options = CreateDefaultOptions(propertyInfo)
            };
        }
    }
}