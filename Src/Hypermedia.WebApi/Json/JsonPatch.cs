using System;
using System.Collections.Generic;
using Hypermedia.Json;
using Hypermedia.Json.Converters;
using Hypermedia.Metadata;
using JsonLite.Ast;

namespace Hypermedia.WebApi.Json
{
    public sealed class JsonPatch<T> : IPatch<T>
    {
        readonly IFieldNamingStrategy _fieldNamingStratgey;
        readonly JsonValue _jsonValue;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contractor resolver.</param>
        /// <param name="fieldNamingStratgey">The field naming strategy that is in use.</param>
        /// <param name="jsonValue">The root document node.</param>
        public JsonPatch(IContractResolver contractResolver, IFieldNamingStrategy fieldNamingStratgey, JsonValue jsonValue)
        {
            _fieldNamingStratgey = fieldNamingStratgey;
            _jsonValue = jsonValue;

            ContractResolver = contractResolver;
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
                if (contractResolver.TryResolve(typeof(T), out IContract contract) == false)
                {
                    return false;
                }

                var serializer = new JsonSerializer(
                    new JsonConverterFactory(
                        JsonConverterFactory.Default,
                        new ContractConverter(ContractResolver, _fieldNamingStratgey),
                        new ComplexConverter(_fieldNamingStratgey)));

                var converter = new ContractConverter(contractResolver, _fieldNamingStratgey);
                converter.DeserializeObject(serializer, (JsonObject)_jsonValue, contract, entity);

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
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the resource contract resolver that is to be used for the patching.
        /// </summary>
        public IContractResolver ContractResolver { get; }
    }
}