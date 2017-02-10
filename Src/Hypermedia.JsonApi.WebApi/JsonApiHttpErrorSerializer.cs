using System.Collections;
using System.Linq;
using System.Web.Http;
using JsonLite.Ast;

namespace Hypermedia.JsonApi.WebApi
{
    internal sealed class JsonApiHttpErrorSerializer : IJsonApiSerializer
    {
        internal static readonly JsonApiHttpErrorSerializer Instance = new JsonApiHttpErrorSerializer();

        /// <summary>
        /// Serialize a list of entities.
        /// </summary>
        /// <param name="entities">The list of entities to serialize.</param>
        /// <returns>The JSON object that represents the serialized entity.</returns>
        JsonObject IJsonApiSerializer.SerializeMany(IEnumerable entities)
        {
            var x = entities.OfType<HttpError>().ToList();
            var y = entities.Cast<object>().First();

            return JsonApiErrorSerializer.Instance.SerializeMany(entities.OfType<HttpError>().Select(Map));
        }

        /// <summary>
        /// Serialize the an entity.
        /// </summary>
        /// <param name="entity">The entity to serialize.</param>
        /// <returns>The JSON object that represents the serialized entity.</returns>
        JsonObject IJsonApiSerializer.Serialize(object entity)
        {
            return JsonApiErrorSerializer.Instance.Serialize(Map((HttpError)entity));
        }

        /// <summary>
        /// Map the HTTP error to a JsonApiError.
        /// </summary>
        /// <param name="error">The HTTP error to map.</param>
        /// <returns>The JSON API error that was mapped from the HTTP Error.</returns>
        static JsonApiError Map(HttpError error)
        {
            return new JsonApiError();
        }
    }
}