using System;
using JsonLite.Ast;

namespace Hypermedia.JsonApi
{
    internal struct JsonApiEntityKey
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonObject">The JSON object that contains the key information.</param>
        internal JsonApiEntityKey(JsonObject jsonObject)
        {
            // ReSharper disable once JoinNullCheckWithUsage
            if (jsonObject == null)
            {
                throw new ArgumentNullException(nameof(jsonObject));
            }

            Type = null;
            Id = null;

            foreach (var member in jsonObject.Members)
            {
                switch (member.Name)
                {
                    case "type":
                        Type = ((JsonString)member.Value).Value;
                        break;

                    case "id":
                        Id = member.Value.Stringify();
                        break;
                }
            }

            if (Type == null)
            {
                throw new ArgumentException("Could not find the type.");
            }
        }

        /// <summary>
        /// The type of the entity.
        /// </summary>
        internal string Type { get; }

        /// <summary>
        /// The ID of the entity.
        /// </summary>
        internal string Id { get; }
    }
}