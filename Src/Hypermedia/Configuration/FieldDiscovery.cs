using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hypermedia.Configuration
{
    public sealed class FieldDiscovery : IFieldDiscovery
    {
        public static readonly IFieldDiscovery Shallow = new FieldDiscovery(ShallowDiscovery);
        public static readonly IFieldDiscovery Deep = new FieldDiscovery(DeepDiscovery);

        readonly Func<TypeInfo, IEnumerable<PropertyInfo>> _delegate;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="delegate">The delegate function that performs the resolution.</param>
        FieldDiscovery(Func<TypeInfo, IEnumerable<PropertyInfo>> @delegate)
        {
            _delegate = @delegate;
        }

        /// <summary>
        /// Discover the fields that are available on the type.
        /// </summary>
        /// <param name="type">The type to discover the properties on.</param>
        /// <returns>The list of properties on the type.</returns>
        public IEnumerable<PropertyInfo> Discover(TypeInfo type)
        {
            return _delegate(type);
        }

        /// <summary>
        /// Discover the fields that are available on the type.
        /// </summary>
        /// <param name="type">The type to discover the properties on.</param>
        /// <returns>The list of properties on the type.</returns>
        static IEnumerable<PropertyInfo> ShallowDiscovery(TypeInfo type)
        {
            return type.DeclaredProperties;
        }

        /// <summary>
        /// Discover the fields that are available on the type.
        /// </summary>
        /// <param name="type">The type to discover the properties on.</param>
        /// <returns>The list of properties on the type.</returns>
        static IEnumerable<PropertyInfo> DeepDiscovery(TypeInfo type)
        {
            var properties = ShallowDiscovery(type);

            if (type.BaseType != null && type.BaseType != typeof(object))
            {
                properties = properties.Union(DeepDiscovery(type.BaseType.GetTypeInfo()));
            }

            return properties;
        }
    }
}