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

        readonly IJsonConverterFactory _defaultConverterFactory;
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
        /// <param name="defaultConverterFactory">The default converter factory to use.</param>
        /// <param name="converters">The list of available converters.</param>
        public JsonConverterFactory(IJsonConverterFactory defaultConverterFactory, IEnumerable<IJsonConverter> converters)
        {
            _defaultConverterFactory = defaultConverterFactory;
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
            if (_defaultConverterFactory != null)
            {
                var converter = _defaultConverterFactory.CreateInstance(type);

                if (converter != null)
                {
                    return converter;
                }
            }

            return GetOrCreateInstance(type);
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