using System;

namespace Hypermedia.Json
{
    public sealed class JsonConverterAttribute : Attribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="converterType">The converter type.</param>
        public JsonConverterAttribute(Type converterType)
        {
            if (converterType == null)
            {
                throw new ArgumentNullException(nameof(converterType));
            }

            ConverterType = converterType;
        }

        /// <summary>
        /// Gets the converter type.
        /// </summary>
        public Type ConverterType { get; }
    }
}