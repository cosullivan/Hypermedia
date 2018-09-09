using System.Collections.Generic;
using System.Linq;
using Hypermedia.Json;
using Hypermedia.Metadata;
using Hypermedia.WebApi;
using JsonLite.Ast;

namespace Hypermedia.JsonApi.WebApi
{
    public sealed class JsonApiPatch<T> : IPatch<T>
    {
        readonly IFieldNamingStrategy _fieldNamingStratgey;
        readonly JsonObject _jsonValue;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The resource contractor resolver.</param>
        /// <param name="fieldNamingStratgey">The field naming strategy to use.</param>
        /// <param name="jsonValue">The root document node.</param>
        public JsonApiPatch(IContractResolver contractResolver, IFieldNamingStrategy fieldNamingStratgey, JsonObject jsonValue)
        {
            _fieldNamingStratgey = fieldNamingStratgey;
            _jsonValue = jsonValue;

            ContractResolver = contractResolver;
        }

        /// <summary>
        /// Attempt to resolve the contract that is represented in the JSON object.
        /// </summary>
        /// <param name="contractResolver">The contract resolver to use.</param>
        /// <param name="jsonObject">The JSON object that contains the contract definition.</param>
        /// <param name="contract">The contract that was resolved.</param>
        /// <returns>true if the contract could be resolved, false if not.</returns>
        bool TryResolveContact(IContractResolver contractResolver, JsonObject jsonObject, out IContract contract)
        {
            var typeAttribute = jsonObject?["type"];

            if (typeAttribute == null)
            {
                contract = null;
                return false;
            }

            return contractResolver.TryResolve(((JsonString) typeAttribute).Value, out contract);
        }

        /// <summary>
        /// Attempt to patch the given entity.
        /// </summary>
        /// <param name="entity">The entity to apply the patch to.</param>
        /// <param name="contractResolver">The contract resolver to use.</param>
        /// <returns>true if the entity could be patched, false if not.</returns>
        public bool TryPatch(T entity, IContractResolver contractResolver)
        {
            try
            {
                var jsonObject = _jsonValue["data"] as JsonObject;

                if (TryResolveContact(contractResolver, jsonObject, out var contract) == false)
                {
                    return false;
                }

                var serializer = new JsonApiSerializer(
                    new JsonApiSerializerOptions(new ContractResolver(contract))
                    {
                        FieldNamingStrategy = _fieldNamingStratgey
                    });

                serializer.DeserializeEntity(contract, jsonObject, entity);

                return true;
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { }

            return false;
        }

        /// <summary>
        /// Returns the list of members that are being patched.
        /// </summary>
        public IReadOnlyList<IMember> Members()
        {
            var jsonObject = _jsonValue["data"] as JsonObject;

            if (TryResolveContact(ContractResolver, jsonObject, out var contract) == false)
            {
                return new IMember[0];
            }

            return ExtractMembers(contract, jsonObject).ToList();
        }

        /// <summary>
        /// Determine the members that are defined in the patch content.
        /// </summary>
        /// <param name="contract">The contract to use to return the members from.</param>
        /// <param name="jsonObject">The JSON object that defines the attributes & relationships.</param>
        /// <returns>The list of members that are defined in the patch content.</returns>
        IEnumerable<IMember> ExtractMembers(IContract contract, JsonObject jsonObject)
        {
            var dictionary = contract.Fields.ToDictionary(k => _fieldNamingStratgey.GetName(k.Name));

            if (jsonObject["attributes"] is JsonObject attributes)
            {
                foreach (var attribute in attributes.Members)
                {
                    if (dictionary.TryGetValue(attribute.Name, out IField member))
                    {
                        yield return member;
                    }
                }
            }

            if (jsonObject["relationships"] is JsonObject relationships)
            {
                foreach (var relationship in relationships.Members)
                {
                    if (dictionary.TryGetValue(relationship.Name, out IField member))
                    {
                        yield return member;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the resource contract resolver that is to be used for the patching.
        /// </summary>
        public IContractResolver ContractResolver { get; }
    }
}