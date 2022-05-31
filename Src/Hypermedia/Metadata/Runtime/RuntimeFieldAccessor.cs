using System;
using System.Linq;
using System.Reflection;

namespace Hypermedia.Metadata.Runtime
{
    internal sealed class RuntimeFieldAccessor : IFieldAccessor
    {
        readonly PropertyInfo _memberInfo;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="property">The property to access from.</param>
        internal RuntimeFieldAccessor(PropertyInfo property)
        {
            _memberInfo = property;
        }

        /// <summary>
        /// Returns a field accessor from the given type and field.
        /// </summary>
        /// <typeparam name="T">The entity type to return the accessor for.</typeparam>
        /// <param name="field">The name of the field.</param>
        /// <param name="allowNonPublic">Searches for non-public properties of the same name too</param>
        /// <returns>The field accessor for the given field name.</returns>
        internal static IFieldAccessor From<T>(string field)
        {
            var property = typeof(T).GetRuntimeProperties().First(rp => rp.Name == field);

            if (property == null)
            {
                throw new ArgumentException(
                    $"\"{field}\" property not found on type \"{typeof(T).FullName}\".\nCheck your contract resolver configuration.",
                    nameof(field));
            }

            return new RuntimeFieldAccessor(property);
        }

        /// <summary>
        /// Gets the actual value of the field.
        /// </summary>
        /// <param name="instance">The instance to return the value from.</param>
        /// <returns>The value of the field from the instance.</returns>
        public object GetValue(object instance)
        {
            if (_memberInfo.CanRead == false)
            {
                return null;
            }

            return _memberInfo.GetValue(instance);
        }

        /// <summary>
        /// Sets the value for the field.
        /// </summary>
        /// <param name="instance">The instance to set the value on.</param>
        /// <param name="value">The value to set for the field.</param>
        public void SetValue(object instance, object value)
        {
            if (_memberInfo.CanWrite == false)
            {
                return;
            }

            _memberInfo.SetValue(instance, value);
        }

        /// <summary>
        /// Gets the value type.
        /// </summary>
        public Type ValueType => _memberInfo.PropertyType;

        /// <summary>
        /// Indicates whether the value accessor gives the ability to read from the underlying instance.
        /// </summary>
        public bool CanRead => _memberInfo.CanRead;

        /// <summary>
        /// Indicates whether the value accessor gives the ability to write to the underlying instance.
        /// </summary>
        public bool CanWrite => _memberInfo.CanWrite;
    }
}
