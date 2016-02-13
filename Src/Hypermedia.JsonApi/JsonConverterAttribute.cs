using System;

namespace Hypermedia.JsonApi
{
    public sealed class JsonConverterAttribute : Attribute
    {
        readonly Type _converterType;

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

            _converterType = converterType;
        }

        /// <summary>
        /// Gets the converter type.
        /// </summary>
        public Type ConverterType
        {
            get { return _converterType; }
        }
    }
}
