using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hypermedia.Metadata;
using Hypermedia.Metadata.Runtime;
using JsonLite.Ast;

namespace Hypermedia.JsonApi
{
    public sealed class JsonApiSerializer
    {
        readonly IResourceContractResolver _resourceContractResolver;
        readonly IJsonConverterFactory _jsonConverterFactory = new JsonConverterFactory();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="resourceContractResolver">The resource contract resolver.</param>
        public JsonApiSerializer(IResourceContractResolver resourceContractResolver)
        {
            _resourceContractResolver = resourceContractResolver;
        }

        /// <summary>
        /// Serialize a list of entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of instance to serialize.</typeparam>
        /// <param name="entities">The list of entities to serialize.</param>
        /// <returns>The JSON object that represents the serialized entity.</returns>
        public JsonObject SerializeMany<TEntity>(IEnumerable<TEntity> entities)
        {
            return SerializeMany((IEnumerable)entities);
        }

        /// <summary>
        /// Serialize a list of entities.
        /// </summary>
        /// <param name="entities">The list of entities to serialize.</param>
        /// <returns>The JSON object that represents the serialized entity.</returns>
        public JsonObject SerializeMany(IEnumerable entities)
        {
            var serializer = new Serializer(_jsonConverterFactory, _resourceContractResolver);

            var members = new List<JsonMember>
            {
                new JsonMember("data", new JsonArray(serializer.Serialize(entities).ToList()))
            };

            var included = serializer.SerializeIncluded(entities).ToList();

            if (included.Count > 0)
            {
                members.Add(new JsonMember("included", new JsonArray(included)));
            }

            return new JsonObject(members);
        }

        /// <summary>
        /// Serialize the an entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of instance to serialize.</typeparam>
        /// <param name="entity">The entity to serialize.</param>
        /// <returns>The JSON object that represents the serialized entity.</returns>
        public JsonObject SerializeEntity<TEntity>(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return SerializeEntity((object)entity);
        }

        /// <summary>
        /// Serialize the an entity.
        /// </summary>
        /// <param name="entity">The entity to serialize.</param>
        /// <returns>The JSON object that represents the serialized entity.</returns>
        public JsonObject SerializeEntity(object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var serializer = new Serializer(_jsonConverterFactory, _resourceContractResolver);

            var members = new List<JsonMember>
            {
                new JsonMember("data", serializer.Serialize(new[] { entity }).First())
            };

            var included = serializer.SerializeIncluded(new[] { entity }).ToList();

            if (included.Count > 0)
            {
                members.Add(new JsonMember("included", new JsonArray(included)));
            }

            return new JsonObject(members);
        }

        /// <summary>
        /// Deserialize a collection of items.
        /// </summary>
        /// <param name="jsonObject">The top level object to deserialize.</param>
        /// <returns>The list of items that was deserialized.</returns>
        public IEnumerable<object> DeserializeMany(JsonObject jsonObject)
        {
            var jsonArray = jsonObject["data"] as JsonArray;

            if (jsonArray == null)
            {
                throw new JsonApiException("Can not return a sequence of items as the top level value is only a single entity.");
            }

            var deserializer = new Deserializer(_jsonConverterFactory, _resourceContractResolver);

            return jsonArray.OfType<JsonObject>().Select(deserializer.DeserializeEntity).ToList();
        }

        /// <summary>
        /// Deserialize a JSON object into a CLR type.
        /// </summary>
        /// <param name="jsonObject">The top level JSON object to deserialize into a CLR type.</param>
        /// <returns>The instance that was created.</returns>
        public object DeserializeEntity(JsonObject jsonObject)
        {
            jsonObject = jsonObject["data"] as JsonObject;

            if (jsonObject == null)
            {
                throw new JsonApiException("Can not return a single item as the top level value is an array.");
            }

            return new Deserializer(_jsonConverterFactory, _resourceContractResolver).DeserializeEntity(jsonObject);
        }

        /// <summary>
        /// Deserialize an object.
        /// </summary>
        /// <param name="type">The entity type of the object to deserialize.</param>
        /// <param name="jsonObject">The JSON object that represents the object to deserialize.</param>
        /// <param name="entity">The entity instance to deserialize the fields into.</param>
        internal void DeserializeEntity(IResourceContract type, JsonObject jsonObject, object entity)
        {
            new Deserializer(_jsonConverterFactory, new ResourceContractResolver(type)).DeserializeEntity(type, jsonObject, entity);
        }

        #region Serializer

        class Serializer
        {
            readonly IJsonConverterFactory _jsonConverterFactory;
            readonly IResourceContractResolver _resourceContractResolver;
            readonly HashSet<JsonObject> _visited = new HashSet<JsonObject>(JsonApiEntityKeyEqualityComparer.Instance);

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="jsonConverterFactory">The JSON converter factory.</param>
            /// <param name="resourceContractResolver">The resource contract resolver.</param>
            internal Serializer(IJsonConverterFactory jsonConverterFactory, IResourceContractResolver resourceContractResolver)
            {
                _jsonConverterFactory = jsonConverterFactory;
                _resourceContractResolver = resourceContractResolver;
            }

            /// <summary>
            /// Serialize the list of entities.
            /// </summary>
            /// <param name="entities">The list of entities to serialize.</param>
            /// <returns>The list of entities that were serialzied.</returns>
            internal IEnumerable<JsonObject> Serialize(IEnumerable entities)
            {
                foreach (var entity in entities)
                {
                    IResourceContract resourceContract;
                    if (_resourceContractResolver.TryResolve(entity.GetType(), out resourceContract) == false)
                    {
                        throw new JsonApiException("Can not serialize an unknown resource type '{0}'.", entity.GetType());
                    }

                    var jsonObject = SerializeEntity(resourceContract, entity);

                    if (HasVisited(jsonObject) == false)
                    {
                        yield return jsonObject;
                    }

                    Visit(jsonObject);
                }
            }

            /// <summary>
            /// Returns a value indicating whether the entity that the given JSON object represents has been visited.
            /// </summary>
            /// <param name="jsonObject">The JSON object that defines an entity.</param>
            /// <returns>true if the entity represented by the JSON object has been visitied, false if not.</returns>
            bool HasVisited(JsonObject jsonObject)
            {
                return _visited.Contains(jsonObject);
            }

            /// <summary>
            /// Marks the entity represented by the JSON object as having being visited.
            /// </summary>
            /// <param name="jsonObject">The JSON object that represents an entity.</param>
            void Visit(JsonObject jsonObject)
            {
                _visited.Add(jsonObject);
            }

            /// <summary>
            /// Serialize an entity.
            /// </summary>
            /// <param name="resourceContract">The resource contract.</param>
            /// <param name="entity">The entity to serialize.</param>
            /// <returns>The JSON object that represents the serialized entity.</returns>
            JsonObject SerializeEntity(IResourceContract resourceContract, object entity)
            {
                var members = new List<JsonMember>(SerializeResourceKey(resourceContract, entity));

                var fields = SerializeFields(resourceContract, entity).Where(IsNotNull).ToList();
                if (fields.Any())
                {
                    members.Add(new JsonMember("attributes", new JsonObject(fields)));
                }

                var relationships = SerializeRelationships(resourceContract.Relationships, entity);
                if (relationships.Any())
                {
                    members.Add(new JsonMember("relationships", new JsonObject(relationships)));
                }

                return new JsonObject(members);
            }

            /// <summary>
            /// Serialize the list of fields.
            /// </summary>
            /// <param name="resourceContract">The resource contract that defines the fields to serialize.</param>
            /// <param name="entity">The instance to serialize the fields from.</param>
            /// <returns>The list of JSON values which represent the fields.</returns>
            IReadOnlyList<JsonMember> SerializeFields(IResourceContract resourceContract, object entity)
            {
                var fields = resourceContract.Fields.Where(f => ShouldSerializeField(resourceContract, f)).ToList();

                return SerializeFields(fields, entity);
            }

            /// <summary>
            /// Serialize the list of fields.
            /// </summary>
            /// <param name="fields">The list of fields to serialize.</param>
            /// <param name="entity">The instance to serialize the fields from.</param>
            /// <returns>The list of JSON values which represent the fields.</returns>
            IReadOnlyList<JsonMember> SerializeFields(IEnumerable<IField> fields, object entity)
            {
                return fields.Select(field => SerializeField(field, entity)).ToList();
            }

            /// <summary>
            /// Build the JSON member for the given field.
            /// </summary>
            /// <param name="field">The field to build the JSON member for.</param>
            /// <param name="entity">The entity to extract the value for the JSON member.</param>
            /// <returns>The JSON member which represents the given field on the entity.</returns>
            JsonMember SerializeField(IField field, object entity)
            {
                return new JsonMember(field.Name.LowerFirstCharacter().Dasherize(), SerializeValue(field.GetValue(entity)));
            }

            /// <summary>
            /// Serialize the list of relationships.
            /// </summary>
            /// <param name="relationships">The list of relationships to serialize.</param>
            /// <param name="entity">The instance to serialize the relationships from.</param>
            /// <returns>The list of JSON values which represent the relationships.</returns>
            IReadOnlyList<JsonMember> SerializeRelationships(IEnumerable<IRelationship> relationships, object entity)
            {
                return relationships.Select(relationship => SerializeRelationship(relationship, entity)).ToList();
            }

            /// <summary>
            /// Build a member that represents the relationship.
            /// </summary>
            /// <param name="relationship">The relationship that the member is being built for.</param>
            /// <param name="entity">The entity to build the relationship for.</param>
            /// <returns>The member that represents the relationship for the given entity.</returns>
            JsonMember SerializeRelationship(IRelationship relationship, object entity)
            {
                var uri = relationship.UriTemplate.Bind(entity);

                var members = new List<JsonMember>
                {
                    new JsonMember(
                        "links",
                        new JsonObject(new JsonMember("related", new JsonString(uri))))
                };

                if (ShouldSerializeRelationship(relationship))
                {
                    var data = SerializeRelationshipData(relationship, entity);

                    if (data != null)
                    {
                        members.Add(new JsonMember("data", data));
                    }
                }

                return new JsonMember(relationship.Name.LowerFirstCharacter().Dasherize(), new JsonObject(members));
            }

            /// <summary>
            /// Serialize the relationship data.
            /// </summary>
            /// <param name="relationship">The relationship that the member is being built for.</param>
            /// <param name="entity">The entity to build the relationship for.</param>
            /// <returns>The JSON value that represents the actual relationship data, or null if no data link can be created.</returns>
            JsonValue SerializeRelationshipData(IRelationship relationship, object entity)
            {
                IResourceContract resourceContract;
                if (_resourceContractResolver.TryResolve(relationship.RelatedTo, out resourceContract) == false)
                {
                    throw new JsonApiException(
                        "Could not find the related type '{0}' for the relationship '{1}'.", relationship.RelatedTo, relationship.Name);
                }

                if (relationship.Type == RelationshipType.BelongsTo)
                {
                    return SerializeBelongsTo(relationship, resourceContract, entity);
                }

                return SerializeHasMany(relationship, resourceContract, entity);
            }

            /// <summary>
            /// Serialize a BelongsTo relationship.
            /// </summary>
            /// <param name="relationship">The relationship to serialize.</param>
            /// <param name="resourceContract">The resource contract of the related entity.</param>
            /// <param name="entity">The entity instance to serialize the relationship from.</param>
            /// <returns>The value that represents the data node for the relationship.</returns>
            JsonValue SerializeBelongsTo(IRelationship relationship, IResourceContract resourceContract, object entity)
            {
                var value = relationship.Field.GetValue(entity);

                if (value == null)
                {
                    return null;
                }

                return new JsonObject(SerializeResourceKey(resourceContract, value));
            }

            /// <summary>
            /// Serialize a HasMany relationship.
            /// </summary>
            /// <param name="relationship">The relationship to serialize.</param>
            /// <param name="resourceContract">The resource contract of the related entity.</param>
            /// <param name="entity">The entity instance to serialize the relationship from.</param>
            /// <returns>The value that represents the data node for the relationship.</returns>
            JsonValue SerializeHasMany(IRelationship relationship, IResourceContract resourceContract, object entity)
            {
                var value = relationship.Field.GetValue(entity);

                if (value == null)
                {
                    return null;
                }

                if (TypeHelper.IsEnumerable(value.GetType()) == false)
                {
                    throw new JsonApiException("Can not serialize a HasMany relationship from type that doesnt support IEnumerable.");
                }

                return new JsonArray(SerializeHasMany(resourceContract, (IEnumerable)value).ToList());
            }

            /// <summary>
            /// Serialize the collection to entities into a relationship data node.
            /// </summary>
            /// <param name="resourceContract">The resource contract of the items in the collection.</param>
            /// <param name="collection">The collection of items to serialize.</param>
            /// <returns>The list of JSON object which represent the entity keys.</returns>
            IEnumerable<JsonObject> SerializeHasMany(IResourceContract resourceContract, IEnumerable collection)
            {
                foreach (var item in collection)
                {
                    yield return new JsonObject(SerializeResourceKey(resourceContract, item));
                }
            }

            /// <summary>
            /// Serialize an resource key.
            /// </summary>
            /// <param name="resourceContract">The resource contract.</param>
            /// <param name="value">The value to serialize the key for.</param>
            /// <returns>The JSON object that represents the entity key.</returns>
            IReadOnlyList<JsonMember> SerializeResourceKey(IResourceContract resourceContract, object value)
            {
                var members = new List<JsonMember>
                {
                    new JsonMember("type", new JsonString(resourceContract.Name))
                };

                if (value != null && TypeHelper.IsReferenceType(value.GetType()))
                {
                    var field = resourceContract.Fields(FieldOptions.Id).SingleOrDefault();

                    if (field == null)
                    {
                        return members;
                    }

                    value = field.GetValue(value);
                }

                if (value != null)
                {
                    members.Add(new JsonMember("id", SerializeValue(value)));
                }

                return members;
            }

            /// <summary>
            /// Serialize the list of included entities for the given top level primary entities.
            /// </summary>
            /// <param name="entities">The list of top level entities to serialize the included entities for.</param>
            /// <returns>The list of JSON objects that represent the included entities.</returns>
            internal IEnumerable<JsonObject> SerializeIncluded(IEnumerable entities)
            {
                var included = new List<JsonObject>();

                foreach (var entity in entities)
                {
                    IResourceContract resourceContract;
                    if (_resourceContractResolver.TryResolve(entity.GetType(), out resourceContract) == false)
                    {
                        throw new JsonApiException("Could not find the entity type for the CLR type of '{0}'.", entity.GetType());
                    }

                    included.AddRange(SerializeIncluded(resourceContract.Relationships, entity));
                }

                return included;
            }

            /// <summary>
            /// Serialize the list of included entities starting from the top level.
            /// </summary>
            /// <param name="resourceContract">The resource contract for the entities in the collection.</param>
            /// <param name="collection">The collection of entities to serialize the included items from.</param>
            /// <returns>The list of JSON objects that represent the included fields.</returns>
            IEnumerable<JsonObject> SerializeIncluded(IResourceContract resourceContract, IEnumerable collection)
            {
                var included = new List<JsonObject>();

                foreach (var entity in collection)
                {
                    included.AddRange(SerializeIncluded(resourceContract, entity));
                }

                return included;
            }

            /// <summary>
            /// Serialize the entity as an included type.
            /// </summary>
            /// <param name="resourceContract">The resource contract.</param>
            /// <param name="entity">The entity to serialize as an included type.</param>
            /// <returns>The list of types to included for the entity.</returns>
            IEnumerable<JsonObject> SerializeIncluded(IResourceContract resourceContract, object entity)
            {
                var jsonObject = SerializeEntity(resourceContract, entity);

                if (HasVisited(jsonObject))
                {
                    return new JsonObject[0];
                }

                Visit(jsonObject);

                return new[] { jsonObject }.Union(SerializeIncluded(resourceContract.Relationships, entity));
            }

            /// <summary>
            /// Serialize the relationships on the given entity.
            /// </summary>
            /// <param name="relationships">The list of relationships to serialize the included types from.</param>
            /// <param name="entity">The entity to serialize from.</param>
            /// <returns>The list of types to include for the entity.</returns>
            IEnumerable<JsonObject> SerializeIncluded(IEnumerable<IRelationship> relationships, object entity)
            {
                var included = new List<JsonObject>();

                foreach (var relationship in relationships.Where(ShouldSerializeRelationship))
                {
                    included.AddRange(SerializeIncluded(relationship, entity));
                }

                return included;
            }

            /// <summary>
            /// Serialize the list of included items from the relationship.
            /// </summary>
            /// <param name="relationship">The relationship that defines the related items.</param>
            /// <param name="entity">The entity to serialize the included items from.</param>
            /// <returns>The list of types to include for the entity.</returns>
            IEnumerable<JsonObject> SerializeIncluded(IRelationship relationship, object entity)
            {
                IResourceContract resourceContract;
                if (_resourceContractResolver.TryResolve(relationship.RelatedTo, out resourceContract) == false)
                {
                    throw new JsonApiException(
                        "Could not find the related type '{0}' for the relationship '{1}'.", relationship.RelatedTo, relationship.Name);
                }

                if (relationship.Type == RelationshipType.HasMany)
                {
                    return SerializeIncludedHasMany(relationship, resourceContract, entity);
                }

                return SerializeIncludedBelongsTo(relationship, resourceContract, entity);
            }

            /// <summary>
            /// Serialize the list of included items from the HasMany relationship.
            /// </summary>
            /// <param name="relationship">The relationship that defines the related items.</param>
            /// <param name="resourceContract">The resource contract of the related items.</param>
            /// <param name="entity">The entity to serialize the included items from.</param>
            /// <returns>The list of types to include for the entity.</returns>
            IEnumerable<JsonObject> SerializeIncludedHasMany(IRelationship relationship, IResourceContract resourceContract, object entity)
            {
                var collection = relationship.Field.GetValue(entity);

                if (collection == null)
                {
                    return new JsonObject[0];
                }

                if (TypeHelper.IsEnumerable(collection.GetType()) == false)
                {
                    throw new JsonApiException("Can not serialize a HasMany relationship from type that doesnt support IEnumerable.");
                }

                return SerializeIncluded(resourceContract, (IEnumerable)collection);
            }

            /// <summary>
            /// Serialize the list of included items from the BelongsTo relationship.
            /// </summary>
            /// <param name="relationship">The relationship that defines the related items.</param>
            /// <param name="resourceContract">The resource contract of the related items.</param>
            /// <param name="entity">The entity to serialize the included items from.</param>
            /// <returns>The list of types to include for the entity.</returns>
            IEnumerable<JsonObject> SerializeIncludedBelongsTo(IRelationship relationship, IResourceContract resourceContract, object entity)
            {
                var value = relationship.Field.GetValue(entity);

                if (value == null || TypeHelper.IsReferenceType(value.GetType()) == false)
                {
                    return new JsonObject[0];
                }

                return SerializeIncluded(resourceContract, value);
            }

            /// <summary>
            /// Serialize an inline object.
            /// </summary>
            /// <param name="value">The value to serialization inline.</param>
            /// <returns>The JSON value which represents the inline serialization of the value.</returns>
            JsonValue SerializeValue(object value)
            {
                if (ReferenceEquals(value, null))
                {
                    return JsonNull.Instance;
                }

                var type = value.GetType();

                IJsonConverter jsonConverter;
                if (_jsonConverterFactory.TryCreateInstance(type, out jsonConverter))
                {
                    return jsonConverter.Serialize(value);
                }

                if (type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEnumerable)))
                {
                    return SerializeArray(value);
                }

                var fields = SerializeFields(RuntimeResourceContract.CreateRuntimeType(type), value).Where(IsNotNull).ToList();

                return new JsonObject(fields);
            }

            /// <summary>
            /// Serialize the value as an array.
            /// </summary>
            /// <param name="value">The value to serialize.</param>
            /// <returns>The JSON array which represents the value.</returns>
            JsonArray SerializeArray(object value)
            {
                var collection = ((IEnumerable)value).Cast<object>();

                return new JsonArray(collection.Select(SerializeValue).ToList());
            }

            /// <summary>
            /// Returns a value indicating whether or not the given field should be included when serializing.
            /// </summary>
            /// <param name="resourceContract">The resource contract that the field belongs to.</param>
            /// <param name="field">The field to determine whether or not it should be included.</param>
            /// <returns>true if the field should be included, false if not.</returns>
            static bool ShouldSerializeField(IResourceContract resourceContract, IField field)
            {
                if (resourceContract.Relationships.Any(relationship => relationship.Field == field))
                {
                    return false;
                }

                return field.Is(FieldOptions.Id) == false && field.Is(FieldOptions.CanSerialize);
            }

            /// <summary>
            /// Returns a value indicating whether the given relationship can be serialized.
            /// </summary>
            /// <param name="relationship">The relationship to determine whether it can be serialized.</param>
            /// <returns>true if the relationship can be serialized, false if not.</returns>
            static bool ShouldSerializeRelationship(IRelationship relationship)
            {
                return relationship.RelatedTo != null && relationship.Field != null;
            }

            /// <summary>
            /// Returns a value indicating whether the JSON Member has a non-null value.
            /// </summary>
            /// <param name="jsonMember">The JSON member to test.</param>
            /// <returns>true if the JSON member has a non-null value, false if not.</returns>
            static bool IsNotNull(JsonMember jsonMember)
            {
                return jsonMember.Value.GetType() != typeof(JsonNull);
            }
        }

        #endregion

        #region Deserializer

        class Deserializer
        {
            readonly IJsonConverterFactory _jsonConverterFactory;
            readonly IResourceContractResolver _resourceContractResolver;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="jsonConverterFactory">The JSON converter factory.</param>
            /// <param name="resourceContractResolver">The resource contract resolver.</param>
            internal Deserializer(IJsonConverterFactory jsonConverterFactory, IResourceContractResolver resourceContractResolver)
            {
                _jsonConverterFactory = jsonConverterFactory;
                _resourceContractResolver = resourceContractResolver;
            }

            /// <summary>
            /// Deserialize a JSON object into a CLR type.
            /// </summary>
            /// <param name="jsonObject">The JSON object to deserialize into a CLR type.</param>
            /// <returns>The instance that was created.</returns>
            internal object DeserializeEntity(JsonObject jsonObject)
            {
                var typeAttribute = jsonObject.Members.Single(member => member.Name.Value == "type");
                
                IResourceContract resourceContract;
                if (_resourceContractResolver.TryResolve(((JsonString)typeAttribute.Value).Value, out resourceContract) == false)
                {
                    throw new JsonApiException("Could not find a type for '{0}'.", ((JsonString)typeAttribute.Value).Value);
                }

                return DeserializeEntity(resourceContract, jsonObject);
            }

            /// <summary>
            /// Deserialize an object.
            /// </summary>
            /// <param name="resourceContract">The resource contract of the object to deserialize.</param>
            /// <param name="jsonObject">The JSON object that represents the object to deserialize.</param>
            /// <returns>The instance that was deserialized.</returns>
            object DeserializeEntity(IResourceContract resourceContract, JsonObject jsonObject)
            {
                var entity = resourceContract.CreateInstance();

                DeserializeEntity(resourceContract, jsonObject, entity);

                return entity;
            }

            /// <summary>
            /// Deserialize an object.
            /// </summary>
            /// <param name="resourceContract">The resource contract type of the object to deserialize.</param>
            /// <param name="jsonObject">The JSON object that represents the object to deserialize.</param>
            /// <param name="entity">The entity instance to deserialize the fields into.</param>
            internal void DeserializeEntity(IResourceContract resourceContract, JsonObject jsonObject, object entity)
            {
                var attribute = jsonObject.Members.SingleOrDefault(member => member.Name.Value == "id");

                if (attribute != null)
                {
                    var field = resourceContract.Fields(FieldOptions.Id).SingleOrDefault();
                    if (field != null && field.Is(FieldOptions.CanDeserialize))
                    {
                        DeserializeField(field, attribute.Value, entity);
                    }
                }

                var attributes = jsonObject.Members.SingleOrDefault(member => member.Name.Value == "attributes");
                if (attributes?.Value is JsonObject)
                {
                    DeserializeFields(resourceContract.Fields(f => ShouldDeserializeField(resourceContract, f)).ToList(), ((JsonObject)attributes.Value).Members, entity);
                }

                var relationships = jsonObject.Members.SingleOrDefault(member => member.Name.Value == "relationships");
                if (relationships?.Value is JsonObject)
                {
                    DeserializeRelationships(resourceContract.Relationships, ((JsonObject)relationships.Value).Members, entity);
                }
            }

            /// <summary>
            /// Deserialize a list of fields.
            /// </summary>
            /// <param name="fields">The list of allowable fields for deserialization.</param>
            /// <param name="members">The list of members to deserialize the values from.</param>
            /// <param name="entity">The entity to deserialize the values to.</param>
            void DeserializeFields(IReadOnlyList<IField> fields, IEnumerable<JsonMember> members, object entity)
            {
                foreach (var member in members)
                {
                    var field = fields.SingleOrDefault(f => String.Equals(f.Name, member.Name.Value.Camelize(), StringComparison.OrdinalIgnoreCase));

                    if (field != null)
                    {
                        DeserializeField(field, member.Value, entity);
                    }
                }
            }

            /// <summary>
            /// Deserialize a field.
            /// </summary>
            /// <param name="field">The field to set on the entity.</param>
            /// <param name="value">The JSON value to set on the entity.</param>
            /// <param name="entity">The entity to set the value on.</param>
            void DeserializeField(IField field, JsonValue value, object entity)
            {
                if (field.Is(FieldOptions.CanDeserialize) == false || value == JsonNull.Instance)
                {
                    return;
                }

                field.SetValue(entity, DeserializeValue(field.ClrType, value));
            }

            /// <summary>
            /// Deserialize a list of relationships.
            /// </summary>
            /// <param name="relationships">The list of allowable relationships for deserialization.</param>
            /// <param name="members">The list of members to deserialize the values from.</param>
            /// <param name="entity">The entity to deserialize the values to.</param>
            void DeserializeRelationships(IReadOnlyList<IRelationship> relationships, IEnumerable<JsonMember> members, object entity)
            {
                foreach (var member in members)
                {
                    var relationship = relationships.SingleOrDefault(r => String.Equals(r.Name, member.Name.Value.Camelize(), StringComparison.OrdinalIgnoreCase));

                    if (relationship == null || !(member.Value is JsonObject))
                    {
                        continue;
                    }

                    var data = ((JsonObject)member.Value).Members.SingleOrDefault(m => m.Name.Value == "data");

                    if (data != null)
                    {
                        DeserializeRelationship(relationship, (JsonObject)data.Value, entity);
                    }
                }
            }

            /// <summary>
            /// Deserialize a relationship.
            /// </summary>
            /// <param name="relationship">The relationship to set on the entity.</param>
            /// <param name="value">The JSON value to set on the entity.</param>
            /// <param name="entity">The entity to set the value on.</param>
            void DeserializeRelationship(IRelationship relationship, JsonObject value, object entity)
            {
                if (relationship.Type == RelationshipType.BelongsTo)
                {
                    DeserializeBelongsTo(relationship, value, entity);
                    return;
                }

                throw new NotImplementedException();
            }

            /// <summary>
            /// Deserialize a BelongsTo relationship.
            /// </summary>
            /// <param name="relationship">The relationship to set on the entity.</param>
            /// <param name="value">The JSON value to set on the entity.</param>
            /// <param name="entity">The entity to set the value on.</param>
            void DeserializeBelongsTo(IRelationship relationship, JsonObject value, object entity)
            {
                var member = value.Members.SingleOrDefault(m => m.Name.Value == "id");

                if (member == null)
                {
                    return;
                }

                DeserializeField(relationship.Field, member.Value, entity);
            }

            /// <summary>
            /// Deserialize the given JSON value according to the specified CLR type.
            /// </summary>
            /// <param name="type">The CLR type to deserialize the JSON value to.</param>
            /// <param name="jsonValue">The JSON value to deserialize.</param>
            /// <returns>The CLR object that the JSON value was deserialized from.</returns>
            object DeserializeValue(Type type, JsonValue jsonValue)
            {
                if (jsonValue == JsonNull.Instance)
                {
                    return null;
                }

                // unwrap the nullable types
                type = Nullable.GetUnderlyingType(type) ?? type;

                IJsonConverter jsonConverter;
                if (_jsonConverterFactory.TryCreateInstance(type, out jsonConverter))
                {
                    return jsonConverter.Deserialize(type, jsonValue);
                }

                if (type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEnumerable)))
                {
                    return DeserializeArray(type, (JsonArray)jsonValue);
                }

                return DeserializeObject(type, (JsonObject)jsonValue);
            }

            /// <summary>
            /// Deserialize a JSON object.
            /// </summary>
            /// <param name="type">The type of the object to deserialize to.</param>
            /// <param name="jsonObject">The JSON object to deserialize from.</param>
            /// <returns>The CLR object that represents the JSON object.</returns>
            object DeserializeObject(Type type, JsonObject jsonObject)
            {
                var entity = Activator.CreateInstance(type);

                DeserializeFields(RuntimeResourceContract.CreateRuntimeFields(type), jsonObject.Members, entity);

                return entity;
            }

            /// <summary>
            /// Deserialize a JSON array.
            /// </summary>
            /// <param name="type">The type of the collection to deserialize to.</param>
            /// <param name="jsonArray">The JSON array to deserialize from.</param>
            /// <returns>The collection that represents the JSON array.</returns>
            ICollection DeserializeArray(Type type, JsonArray jsonArray)
            {
                Type collectionType;
                if (TypeHelper.TryGetCollectionType(type, out collectionType) == false)
                {
                    throw new JsonApiException("Can not deserialize a JSON array to a type that doesnt support ICollection<T>.");
                }

                var method = collectionType
                    .GetTypeInfo()
                        .DeclaredMethods
                            .FirstOrDefault(m => m.DeclaringType == collectionType && m.Name == "Add");

                var elementType = collectionType.GenericTypeArguments[0];

                var collection = Activator.CreateInstance(type) as ICollection;

                foreach (var jsonValue in jsonArray)
                {
                    var value = DeserializeValue(elementType, jsonValue);

                    method.Invoke(collection, new[] { value });
                }

                return collection;
            }

            /// <summary>
            /// Returns a value indicating whether or not the given field should be included when deserializing.
            /// </summary>
            /// <param name="resourceContract">The resource contract that the field belongs to.</param>
            /// <param name="field">The field to determine whether or not it should be included.</param>
            /// <returns>true if the field should be included, false if not.</returns>
            static bool ShouldDeserializeField(IResourceContract resourceContract, IField field)
            {
                if (resourceContract.Relationships.Any(relationship => relationship.Field == field))
                {
                    return false;
                }

                return field.Is(FieldOptions.Id) == false && field.Is(FieldOptions.CanDeserialize);
            }
        }

        #endregion
    }
}