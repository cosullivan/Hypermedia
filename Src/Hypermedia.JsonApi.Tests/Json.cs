using System.IO;
using JsonLite.Ast;

namespace Hypermedia.JsonApi.Tests
{
    internal static class JsonContent
    {
        internal static JsonObject GetObject(string name)
        {
            using (var stream = File.OpenRead($"..\\..\\{name}.json"))
            {
                return (JsonObject)JsonLite.Json.CreateAst(stream);
            }
        }
    }
}