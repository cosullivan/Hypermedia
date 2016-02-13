using System;

namespace Hypermedia.Metadata
{
    public interface IField : IMember
    {
        /// <summary>
        /// Gets the list of options for the field.
        /// </summary>
        FieldOptions Options { get; }

        /// <summary>
        /// Gets the CLR type that the member maps to.
        /// </summary>
        Type ClrType { get; }

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

    public static class FieldExtensions
    {
        /// <summary>
        /// Returns a value indicating whether or not the field adheres to the list of specified options.
        /// </summary>
        /// <param name="field">The field to test the options against.</param>
        /// <param name="options">The list of options to test on the field.</param>
        /// <returns>true if the field contains the list of options, false if not.</returns>
        public static bool Is(this IField field, FieldOptions options)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            return (field.Options & options) == options;
        }
    }
}