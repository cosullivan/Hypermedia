using System.Collections.Generic;
using System.Linq;
using Hypermedia.Metadata;
using Hypermedia.Metadata.Runtime;

namespace Hypermedia.Configuration
{
    public sealed class ContractBuilder<T> : IContractBuilder<T>, IContractBuilder
    {
        readonly IBuilder _builder;
        readonly RuntimeContract _contract;
        readonly IDictionary<string, RuntimeField> _fields = new Dictionary<string, RuntimeField>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The parent builder.</param>
        internal ContractBuilder(IBuilder builder)
        {
            _builder = builder;
            _contract = new RuntimeContract { Name = typeof(T).Name, ClrType = typeof(T) };
        }

        /// <summary>
        /// Build a resource contract resolver with the known types.
        /// </summary>
        /// <returns>The resource contract resolver that is aware of the types that were configured through the builder.</returns>
        public IContractResolver Build()
        {
            return _builder.Build();
        }

        /// <summary>
        /// Build the entity type.
        /// </summary>
        /// <returns>The entity type.</returns>
        IContract IContractBuilder.CreateContract()
        {
            _contract.Fields = _fields.Values.OfType<IField>().ToList();

            return _contract;
        }

        /// <summary>
        /// Returns a Resource Builder for a resource type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the resource to return the builder for.</typeparam>
        /// <param name="discovery">The type discovery mechanism.</param>
        /// <returns>The resource builder to configure.</returns>
        public ContractBuilder<TEntity> With<TEntity>(ITypeDiscovery discovery)
        {
            return _builder.With<TEntity>(discovery);
        }

        /// <summary>
        /// Sets the name of the type.
        /// </summary>
        /// <param name="name">The name of the type.</param>
        /// <returns>The metadata builder to configure.</returns>
        public ContractBuilder<T> Name(string name)
        {
            _contract.Name = name;

            return this;
        }

        /// <summary>
        /// Returns a field.
        /// </summary>
        /// <param name="name">The name of the field to return.</param>
        /// <returns>The field builder build the field.</returns>
        public FieldBuilder<T> Field(string name)
        {
            RuntimeField field;
            if (_fields.TryGetValue(name, out field) == false)
            {
                _fields.Add(name, RuntimeField<T>.CreateRuntimeField(name));
            }

            return new FieldBuilder<T>(this, _fields[name]);
        }

        /// <summary>
        /// Returns a BelongsTo relationship.
        /// </summary>
        /// <param name="name">The name of the relationship to return.</param>
        /// <returns>The relationship builder build the relationship.</returns>
        public RelationshipBuilder<T> BelongsTo<TOther>(string name)
        {
            return Relationship<TOther>(name, RelationshipType.BelongsTo);
        }

        /// <summary>
        /// Returns a HasMany relationship.
        /// </summary>
        /// <param name="name">The name of the relationship to return.</param>
        /// <returns>The relationship builder build the relationship.</returns>
        public RelationshipBuilder<T> HasMany<TOther>(string name)
        {
            return Relationship<TOther>(name, RelationshipType.HasMany);
        }
        
        /// <summary>
        /// Returns a relationship.
        /// </summary>
        /// <param name="name">The name of the relationship to return.</param>
        /// <param name="type">The type of relationship.</param>
        /// <returns>The relationship builder build the relationship.</returns>
        RelationshipBuilder<T> Relationship<TOther>(string name, RelationshipType type)
        {
            RuntimeField field;
            if (_fields.TryGetValue(name, out field))
            {
                // promote the field to a relationship
                _fields[name] = new RuntimeRelationship(type, field) { RelatedTo = typeof(TOther) };
            }
            else
            {
                _fields.Add(name, RuntimeRelationship<T>.CreateRuntimeField(type, name));
            }

            return new RelationshipBuilder<T>(this, (RuntimeRelationship) _fields[name]);
        }
    }
}