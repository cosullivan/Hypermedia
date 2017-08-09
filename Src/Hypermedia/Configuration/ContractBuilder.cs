using System;
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
        readonly List<RuntimeField> _fields = new List<RuntimeField>();

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
            _contract.Fields = _fields.ToList();

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
        /// Attempt to find the field with the given name.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        /// <param name="field">The field that was found with the given name, or null if not field could be found.</param>
        /// <returns>true if a field with the given name was found, false if not.</returns>
        bool TryFindField(string name, out RuntimeField field)
        {
            field = _fields.SingleOrDefault(f => String.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));

            return field != null;
        }

        /// <summary>
        /// Create a new field.
        /// </summary>
        /// <param name="name">The name of the field to create.</param>
        /// <returns>The field that was created.</returns>
        RuntimeField CreateField(string name)
        {
            var field = new RuntimeField(name);

            _fields.Add(field);

            return field;
        }

        /// <summary>
        /// Returns a field.
        /// </summary>
        /// <param name="name">The name of the field to return.</param>
        /// <returns>The field builder build the field.</returns>
        public FieldBuilder<T> Field(string name)
        {
            if (TryFindField(name, out RuntimeField field))
            {
                return new FieldBuilder<T>(this, field);
            }

            return new FieldBuilder<T>(this, CreateField(name));
        }

        /// <summary>
        /// Add the relationship to the fields list.
        /// </summary>
        /// <typeparam name="TRelationship">The instance type to add and return.</typeparam>
        /// <param name="relationship">The relationship to add.</param>
        /// <returns>The instance that was added.</returns>
        TRelationship Add<TRelationship>(TRelationship relationship) where TRelationship : RuntimeField
        {
            _fields.Add(relationship);

            return relationship;
        }

        /// <summary>
        /// Returns a BelongsTo relationship.
        /// </summary>
        /// <param name="name">The name of the relationship to return.</param>
        /// <returns>The relationship builder build the relationship.</returns>
        public BelongsToRelationshipBuilder<T> BelongsTo<TOther>(string name)
        {
            RuntimeField field;
            if (TryFindField(name, out field))
            {
                _fields.Remove(field);

                return new BelongsToRelationshipBuilder<T>(
                    this,
                    Add(new RuntimeBelongsToRelationship(field) { RelatedTo = typeof(TOther) }));
            }

            return new BelongsToRelationshipBuilder<T>(
                this,
                Add(new RuntimeBelongsToRelationship(name) { RelatedTo = typeof(TOther) }));
        }

        /// <summary>
        /// Returns a HasMany relationship.
        /// </summary>
        /// <param name="name">The name of the relationship to return.</param>
        /// <returns>The relationship builder build the relationship.</returns>
        public HasManyRelationshipBuilder<T> HasMany<TOther>(string name)
        {
            RuntimeField field;
            if (TryFindField(name, out field))
            {
                _fields.Remove(field);

                return new HasManyRelationshipBuilder<T>(
                    this, 
                    Add(new RuntimeHasManyRelationship(field) { RelatedTo = typeof(TOther) }));
            }

            return new HasManyRelationshipBuilder<T>(
                this, 
                Add(new RuntimeHasManyRelationship(name) { RelatedTo = typeof(TOther) }));
        }
    }
}