using System.Collections.Generic;
using JsonLite.Ast;

namespace Hypermedia.Json
{
    public interface IJsonExtension
    {
        /// <summary>
        /// The extensible data for the instance.
        /// </summary>
        /// <remarks>When serializing, this information is added to the output. When deserializing, this information
        /// is added from members that don't deserialize to a property of the instance.</remarks>
        ICollection<JsonMember> Data { get; set; }
    }
}