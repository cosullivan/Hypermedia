using System.IO;
using JsonLite.Ast;

namespace Hypermedia.WebApi.Json
{
    public interface IJsonOutputFormatter
    {
        /// <summary>
        /// Write the JSON value to the stream.
        /// </summary>
        /// <param name="writer">The writer to output to.</param>
        /// <param name="jsonValue">The JSON value to write.</param>
        void Write(StreamWriter writer, JsonValue jsonValue);
    }
}