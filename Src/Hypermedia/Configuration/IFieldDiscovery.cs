using System.Collections.Generic;
using System.Reflection;

namespace Hypermedia.Configuration
{
    public interface IFieldDiscovery
    {
        /// <summary>
        /// Discover the fields that are available on the type.
        /// </summary>
        /// <param name="type">The type to discover the properties on.</param>
        /// <returns>The list of properties on the type.</returns>
        IEnumerable<PropertyInfo> Discover(TypeInfo type);
    }
}