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
    }
}
