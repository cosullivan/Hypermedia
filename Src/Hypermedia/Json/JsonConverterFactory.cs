using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Hypermedia.Json
{
    internal sealed class JsonConverterFactory : IJsonConverterFactory
    {
        static readonly IReadOnlyList<IJsonConverter> KnownConverters = new[]
        {
            PrimitiveConverter.Instance,
            NullableConverter.Instance,
            EnumConverter.Instance,
            EnumerableConverter.Instance,
            ComplexConverter.Instance
        };

        readonly IDictionary<Type, IJsonConverter> _resolvedConverters = new Dictionary<Type, IJsonConverter>();
        readonly ReaderWriterLockSlim _resolvedConvertersLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Create an instance of the JSON converter for the given type.
        /// </summary>
        /// <param name="type">The type to create the converter for.</param>
        /// <returns>The JSON converter for the given type.</returns>
        /// <remarks>This will always ensure that a converter is returned.</remarks>
        public IJsonConverter CreateInstance(Type type)
        {
            _resolvedConvertersLock.EnterUpgradeableReadLock();

            try
            {
                IJsonConverter converter;
                if (_resolvedConverters.TryGetValue(type, out converter))
                {
                    return converter;
                }

                _resolvedConvertersLock.EnterWriteLock();

                try
                {
                    converter = DiscoverConverter(type);
                    Debug.Assert(converter != null, "Could not find a converter.");

                    _resolvedConverters[type] = converter;

                    return converter;
                }
                finally 
                {
                    _resolvedConvertersLock.ExitWriteLock();
                }
            }
            finally 
            {
                _resolvedConvertersLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Discover the converter from the given type.
        /// </summary>
        /// <param name="type">The type to discover the converter from.</param>
        /// <returns>The JSON converter for the given type.</returns>
        static IJsonConverter DiscoverConverter(Type type)
        {
            var attribute = type.GetTypeInfo().GetCustomAttribute(typeof(JsonConverterAttribute), false) as JsonConverterAttribute;

            if (attribute != null)
            {
                var converter = Activator.CreateInstance(attribute.ConverterType) as IJsonConverter;

                if (converter == null)
                {
                    throw new JsonException("Can not create an instance of an IJsonConverter from the type '{0}'.", attribute.ConverterType);
                }

                return converter;
            }

            return KnownConverters.First(converter => converter.CanConvert(type));
        }
    }
}
