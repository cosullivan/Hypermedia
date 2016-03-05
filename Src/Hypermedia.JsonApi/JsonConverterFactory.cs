using System;
using System.Collections.Generic;
using System.Reflection;
using Hypermedia.JsonApi.Converters;

namespace Hypermedia.JsonApi
{
    internal sealed class JsonConverterFactory : IJsonConverterFactory
    {
        internal static readonly IDictionary<Type, IJsonConverter> DefaultConverters = new Dictionary<Type, IJsonConverter>
        {
            { typeof(string), StringConverter.Instance },
            { typeof(Guid), GuidConverter.Instance },
            { typeof(decimal), DecimalConverter.Instance },
            { typeof(double), DoubleConverter.Instance },
            { typeof(float), FloatConverter.Instance },
            { typeof(short), Int16Converter.Instance },
            { typeof(int), Int32Converter.Instance },
            { typeof(long), Int64Converter.Instance },
            { typeof(DateTime), DateTimeConverter.Instance },
            { typeof(DateTimeOffset), DateTimeOffsetConverter.Instance },
        };

        readonly IDictionary<Type, IJsonConverter> _converters;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal JsonConverterFactory() : this(DefaultConverters) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="converters">The available list of converters.</param>
        internal JsonConverterFactory(IDictionary<Type, IJsonConverter> converters)
        {
            _converters = converters;
        }

        /// <summary>
        /// Attempts to create an instance of a JSON converter for the given type.
        /// </summary>
        /// <param name="type">The type to create the converter for.</param>
        /// <param name="jsonConverter">The JSON converter for the given type.</param>
        /// <returns>true if a converter could be created, false if not.</returns>
        public bool TryCreateInstance(Type type, out IJsonConverter jsonConverter)
        {
            if (_converters.TryGetValue(type, out jsonConverter))
            {
                return true;
            }

            if (type.GetTypeInfo().IsEnum)
            {
                jsonConverter = EnumConverter.Instance;
                return true;
            }

            var attribute = type.GetTypeInfo().GetCustomAttribute(typeof(JsonConverterAttribute), false) as JsonConverterAttribute;

            if (attribute != null)
            {
                jsonConverter = Activator.CreateInstance(attribute.ConverterType) as IJsonConverter;

                if (jsonConverter == null)
                {
                    throw new JsonApiException("Can not create an instance of an IJsonConverter from the type '{0}'.", attribute.ConverterType);
                }

                return true;
            }

            return false;
        }
    }
}
