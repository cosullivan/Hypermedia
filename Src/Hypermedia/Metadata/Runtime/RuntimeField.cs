using System;
using System.Reflection;

namespace Hypermedia.Metadata.Runtime
{
    internal class RuntimeField : IField
    {
        readonly string _name;
        readonly FieldOptions _options;
        readonly PropertyInfo _memberInfo;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyInfo">The property info that the property is mapped to.</param>
        internal RuntimeField(PropertyInfo propertyInfo) : this(propertyInfo.Name, propertyInfo, CreateDefaultOptions(propertyInfo)) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="memberInfo">The member info that the property is mapped to.</param>
        /// <param name="options">The field options.</param>
        internal RuntimeField(string name, PropertyInfo memberInfo, FieldOptions options)
        {
            _name = name;
            _options = options;
            _memberInfo = memberInfo;
        }

        /// <summary>
        /// Creates the default options for the property info.
        /// </summary>
        /// <param name="propertyInfo">The property info to create the default options for.</param>
        /// <returns>The field options for the given property info.</returns>
        static FieldOptions CreateDefaultOptions(PropertyInfo propertyInfo)
        {
            var options = FieldOptions.None;

            if (propertyInfo.CanWrite == false)
            {
                options = options | FieldOptions.CanDeserialize;
            }

            return options;
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
        /// Gets the name of the member.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the CLR type that the member maps to.
        /// </summary>
        public Type ClrType
        {
            get { return _memberInfo.PropertyType; }
        }

        /// <summary>
        /// Gets the list of options for the field.
        /// </summary>
        public FieldOptions Options
        {
            get { return _options; }
        }
    }

    internal sealed class RuntimeField<T> : RuntimeField
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyInfo">The property info that the property is mapped to.</param>
        internal RuntimeField(PropertyInfo propertyInfo) : base(propertyInfo) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="propertyOrField">The name of the property (or field) that the field is mapped to.</param>
        /// <param name="options">The field options.</param>
        internal RuntimeField(string name, string propertyOrField, FieldOptions options) : 
            base(name, typeof(T).GetRuntimeProperty(propertyOrField), options) { }
    }
}
