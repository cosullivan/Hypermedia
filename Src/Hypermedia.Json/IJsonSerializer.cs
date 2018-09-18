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
    }
}