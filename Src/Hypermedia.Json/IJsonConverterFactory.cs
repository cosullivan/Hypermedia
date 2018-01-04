using System;

namespace Hypermedia.Json
{
    public interface IJsonConverterFactory
    {
        /// <summary>
        /// Create an instance of the JSON converter for the given type.
        /// </summary>
        /// <param name="type">The type to create the converter for.</param>
        /// <returns>The JSON converter for the given type.</returns>
        /// <remarks>This will always ensure that a converter is returned.</remarks>
        IJsonConverter CreateInstance(Type type);
    }
}