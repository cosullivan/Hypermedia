using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Hypermedia.Json.Converters;

namespace Hypermedia.Json
{
    public sealed class JsonConverterFactory : IJsonConverterFactory
    {
        static readonly IJsonConverter[] KnownConverters = new[]
        {
            PrimitiveConverter.Instance,
            NullableConverter.Instance,
            EnumConverter.Instance,
            EnumerableConverter.Instance
        };

        public static IJsonConverterFactory Default = new JsonConverterFactory(KnownConverters);

        readonly IJsonConverterFactory _fallbackConverterFactory;
        readonly IReadOnlyList<IJsonConverter> _converters;
        readonly IDictionary<Type, IJsonConverter> _resolvedConverters = new Dictionary<Type, IJsonConverter>();
        readonly ReaderWriterLockSlim _resolvedConvertersLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="converters">The list of available converters.</param>
        JsonConverterFactory(params IJsonConverter[] converters) : this(Default, converters) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fallbackConverterFactory">The fallback converter factory to use if no overrides are present.</param>
        /// <param name="converters">The list of available converters.</param>
        public JsonConverterFactory(IJsonConverterFactory fallbackConverterFactory, params IJsonConverter[] converters)
        {
            _fallbackConverterFactory = fallbackConverterFactory;
            _converters = converters.Union(KnownConverters).ToList();
        }

        /// <summary>
        /// Create an instance of the JSON converter for the given type.
        /// </summary>
        /// <param name="type">The type to create the converter for.</param>
        /// <returns>The JSON converter for the given type.</returns>
        /// <remarks>This will always ensure that a converter is returned.</remarks>
        public IJsonConverter CreateInstance(Type type)
        {
            var converter = GetOrCreateInstance(type);

            if (converter != null)
            {
                return converter;
            }

            if (_fallbackConverterFactory != null)
            {
                return _fallbackConverterFactory.CreateInstance(type);
            }

            throw new InvalidOperationException($"No converter could be found for the type '{type}'.");
        }

        /// <summary>
        /// Create an instance of the JSON converter for the given type.
        /// </summary>
        /// <param name="type">The type to create the converter for.</param>
        /// <returns>The JSON converter for the given type.</returns>
        /// <remarks>This will always ensure that a converter is returned.</remarks>
        IJsonConverter GetOrCreateInstance(Type type)
        {
            _resolvedConvertersLock.EnterUpgradeableReadLock();

            try
            {
                if (_resolvedConverters.TryGetValue(type, out var converter))
                {
                    return converter;
                }

                _resolvedConvertersLock.EnterWriteLock();

                try
                {
                    converter = DiscoverConverter(type);

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
        IJsonConverter DiscoverConverter(Type type)
        {
            if (type.GetTypeInfo().GetCustomAttribute(typeof(JsonConverterAttribute), false) is JsonConverterAttribute attribute)
            {
                var converter = Activator.CreateInstance(attribute.ConverterType) as IJsonConverter;

                if (converter == null)
                {
                    throw new JsonException("Can not create an instance of an IJsonConverter from the type '{0}'.", attribute.ConverterType);
                }

                return converter;
            }

            return _converters.FirstOrDefault(converter => converter.CanConvert(type));
        }
    }
}