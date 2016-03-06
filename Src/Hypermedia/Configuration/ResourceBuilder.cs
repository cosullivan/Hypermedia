using System;
using System.Collections.Generic;
using System.Linq;
using Hypermedia.Metadata;
using Hypermedia.Metadata.Runtime;

namespace Hypermedia.Configuration
{
    public sealed class ResourceBuilder<T> : IResourceBuilder<T>, IResourceBuilder
    {
        readonly IBuilder _builder;
        readonly List<FieldBuilder<T>> _fields = new List<FieldBuilder<T>>();
        readonly IDictionary<string, RelationshipBuilder<T>> _relationships = new Dictionary<string, RelationshipBuilder<T>>();
        string _name = typeof(T).Name;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The parent builder.</param>
        internal ResourceBuilder(IBuilder builder)
        {
            _builder = builder;
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
        IResourceContract IResourceBuilder.CreateRuntimeContract()
        {
            var fields = _fields.Select(field => field.CreateRuntimeField()).ToList();
            var relationships = _relationships.Values.Select(relationship => relationship.CreateRuntimeRelationship(fields)).ToList();

            return new RuntimeContract<T>(_name, fields, relationships);
        }

        /// <summary>
        /// Returns a Resource Builder for a resource type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the resource to return the builder for.</typeparam>
        /// <param name="discovery">The type discovery mechanism.</param>
        /// <returns>The resource builder to configure.</returns>
        public ResourceBuilder<TEntity> With<TEntity>(ITypeDiscovery discovery)
        {
            return _builder.With<TEntity>(discovery);
        }

        /// <summary>
        /// Sets the name of the type.
        /// </summary>
        /// <param name="name">The name of the type.</param>
        /// <returns>The metadata builder to configure.</returns>
        public ResourceBuilder<T> Name(string name)
        {
            _name = name;

            return this;
        }

        /// <summary>
        /// Returns a field.
        /// </summary>
        /// <param name="name">The name of the field to return.</param>
        /// <returns>The field builder build the field.</returns>
        public FieldBuilder<T> Field(string name)
        {
            var builder = _fields.SingleOrDefault(field => String.Equals(field.Name, name, StringComparison.OrdinalIgnoreCase));

            if (builder == null)
            {
                builder = new FieldBuilder<T>(this, name);

                _fields.Add(builder);
            }

            return builder;
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
            RelationshipBuilder<T> builder;
            if (_relationships.TryGetValue(name, out builder))
            {
                return builder;
            }

            _relationships.Add(name, new RelationshipBuilder<T>(this, name, typeof(TOther), type));

            return _relationships[name];
        }
    }
}
