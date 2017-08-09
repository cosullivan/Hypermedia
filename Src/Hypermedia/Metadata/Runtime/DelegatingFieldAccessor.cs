using System;

namespace Hypermedia.Metadata.Runtime
{
    public sealed class DelegatingFieldAccessor<TEntity, TValue> : IFieldAccessor
    {
        readonly Func<TEntity, TValue> _getter;
        readonly Action<TEntity, TValue> _setter;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="getter">The getter function.</param>
        public DelegatingFieldAccessor(Func<TEntity, TValue> getter) : this(getter, (entity, value) => { }) {}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="getter">The getter function.</param>
        /// <param name="setter">The setter function.</param>
        public DelegatingFieldAccessor(Func<TEntity, TValue> getter, Action<TEntity, TValue> setter)
        {
            if (getter == null)
            {
                throw new ArgumentNullException(nameof(getter));
            }

            if (setter == null)
            {
                throw new ArgumentNullException(nameof(setter));
            }

            _getter = getter;
            _setter = setter;
        }

        /// <summary>
        /// Gets the actual value of the field.
        /// </summary>
        /// <param name="instance">The instance to return the value from.</param>
        /// <returns>The value of the field from the instance.</returns>
        public TValue GetValue(TEntity instance)
        {
            return _getter(instance);
        }

        /// <summary>
        /// Gets the actual value of the field.
        /// </summary>
        /// <param name="instance">The instance to return the value from.</param>
        /// <returns>The value of the field from the instance.</returns>
        object IFieldAccessor.GetValue(object instance)
        {
            return GetValue((TEntity)instance);
        }

        /// <summary>
        /// Sets the value for the field.
        /// </summary>
        /// <param name="instance">The instance to set the value on.</param>
        /// <param name="value">The value to set for the field.</param>
        public void SetValue(TEntity instance, TValue value)
        {
            _setter(instance, value);
        }

        /// <summary>
        /// Sets the value for the field.
        /// </summary>
        /// <param name="instance">The instance to set the value on.</param>
        /// <param name="value">The value to set for the field.</param>
        void IFieldAccessor.SetValue(object instance, object value)
        {
            SetValue((TEntity)instance, (TValue)value);
        }

        /// <summary>
        /// Gets the value type.
        /// </summary>
        public Type ValueType => typeof(TValue);

        /// <summary>
        /// Indicates whether the value accessor gives the ability to read from the underlying instance.
        /// </summary>
        public bool CanRead => _getter != null;

        /// <summary>
        /// Indicates whether the value accessor gives the ability to write to the underlying instance.
        /// </summary>
        public bool CanWrite => _setter != null;
    }
}