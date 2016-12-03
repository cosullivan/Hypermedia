using System;

namespace Hypermedia.Metadata
{
    public interface IFieldAccessor
    {
        /// <summary>
        /// Gets the actual value of the field.
        /// </summary>
        /// <param name="instance">The instance to return the value from.</param>
        /// <returns>The value of the field from the instance.</returns>
        object GetValue(object instance);

        /// <summary>
        /// Sets the value for the field.
        /// </summary>
        /// <param name="instance">The instance to set the value on.</param>
        /// <param name="value">The value to set for the field.</param>
        void SetValue(object instance, object value);

        /// <summary>
        /// Gets the value type.
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// Indicates whether the value accessor gives the ability to read from the underlying instance.
        /// </summary>
        bool CanRead { get; }

        /// <summary>
        /// Indicates whether the value accessor gives the ability to write to the underlying instance.
        /// </summary>
        bool CanWrite { get; }
    }
}