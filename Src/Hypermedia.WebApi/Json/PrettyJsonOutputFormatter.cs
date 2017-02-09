using System.IO;
using JsonLite.Ast;

namespace Hypermedia.WebApi.Json
{
    public sealed class PrettyJsonOutputFormatter : IJsonOutputFormatter
    {
        public static readonly IJsonOutputFormatter Instance = new PrettyJsonOutputFormatter();

        /// <summary>
        /// Write the JSON value to the stream.
        /// </summary>
        /// <param name="writer">The writer to output to.</param>
        /// <param name="jsonValue">The JSON value to write.</param>
        public void Write(StreamWriter writer, JsonValue jsonValue)
        {
            writer.Write(jsonValue.Stringify(true));
        }
    }
}