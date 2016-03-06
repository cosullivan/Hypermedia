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
    }
}
