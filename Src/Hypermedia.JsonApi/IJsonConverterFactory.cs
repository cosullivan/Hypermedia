using System;

namespace Hypermedia.JsonApi
{
    public interface IJsonConverterFactory
    {
        /// <summary>
        /// Attempts to create an instance of a JSON converter for the given type.
        /// </summary>
        /// <param name="type">The type to create the converter for.</param>
        /// <param name="jsonConverter">The JSON converter for the given type.</param>
        /// <returns>true if a converter could be created, false if not.</returns>
        bool TryCreateInstance(Type type, out IJsonConverter jsonConverter);
    }
}
