using System;
using JsonLite.Ast;

namespace Hypermedia.Json
{
    public interface IJsonDeserializer
    {
        /// <summary>
        /// Deserialize the given JSON value according to the specified CLR type.
        /// </summary>
        /// <param name="type">The CLR type to deserialize the JSON value to.</param>
        /// <param name="jsonValue">The JSON value to deserialize.</param>
        /// <returns>The CLR object that the JSON value was deserialized from.</returns>
        object DeserializeValue(Type type, JsonValue jsonValue);
    }
}