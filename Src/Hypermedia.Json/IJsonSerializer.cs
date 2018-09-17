using System;
using JsonLite.Ast;

namespace Hypermedia.Json
{
    public interface IJsonSerializer
    {
        /// <summary>
        /// Serialize an inline object.
        /// </summary>
        /// <param name="value">The value to serialization inline.</param>
        /// <returns>The JSON value which represents the inline serialization of the value.</returns>
        JsonValue SerializeValue(object value);

        /// <summary>
        /// Deserialize the given JSON value according to the specified CLR type.
        /// </summary>
        /// <param name="type">The CLR type to deserialize the JSON value to.</param>
        /// <param name="jsonValue">The JSON value to deserialize.</param>
        /// <returns>The CLR object that the JSON value was deserialized from.</returns>
        object DeserializeValue(Type type, JsonValue jsonValue);

        ///// <summary>
        ///// The field naming strategy.
        ///// </summary>
        //IFieldNamingStrategy FieldNamingStrategy { get; }
    }
}