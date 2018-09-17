using JsonLite.Ast;

namespace Hypermedia.JsonApi
{
    public sealed class MissingContractContext
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The resource type that was missing.</param>
        /// <param name="jsonObject">The JSON object that contains the type that was missing.</param>
        public MissingContractContext(string type, JsonObject jsonObject)
        {
            Type = type;
            JsonObject = jsonObject;
        }

        /// <summary>
        /// The resource type that was missing.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// The JSON object that contains the type that was missing.
        /// </summary>
        public JsonObject JsonObject { get;  }
    }
}