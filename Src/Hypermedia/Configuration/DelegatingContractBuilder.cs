using Hypermedia.Metadata;

namespace Hypermedia.Configuration
{
    public abstract class DelegatingContractBuilder<T, TBuilder> : IContractBuilder<T> where TBuilder : IContractBuilder<T>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The builder instance to delegate to.</param>
        protected DelegatingContractBuilder(TBuilder builder)
        {
            Builder = builder;
        }

        /// <summary>
        /// Build a resource contract resolver with the known types.
        /// </summary>
        /// <returns>The resource contract resolver that is aware of the types that were configured through the builder.</returns>
        public IContractResolver Build()
        {
            return Builder.Build();
        }

        /// <summary>
        /// Returns a Resource Builder for a resource type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the resource to return the builder for.</typeparam>
        /// <param name="discovery">The type discovery mechanism.</param>
        /// <returns>The resource builder to configure.</returns>
        public ContractBuilder<TEntity> With<TEntity>(ITypeDiscovery discovery)
        {
            return Builder.With<TEntity>(discovery);
        }

        /// <summary>
        /// Returns a field.
        /// </summary>
        /// <param name="name">The name of the field to return.</param>
        /// <returns>The field builder build the field.</returns>
        public FieldBuilder<T> Field(string name)
        {
            return Builder.Field(name);
        }

        /// <summary>
        /// Returns a BelongsTo relationship.
        /// </summary>
        /// <param name="name">The name of the relationship to return.</param>
        /// <returns>The relationship builder build the relationship.</returns>
        public BelongsToRelationshipBuilder<T> BelongsTo<TOther>(string name)
        {
            return Builder.BelongsTo<TOther>(name);
        }

        /// <summary>
        /// Returns a HasMany relationship.
        /// </summary>
        /// <param name="name">The name of the relationship to return.</param>
        /// <returns>The relationship builder build the relationship.</returns>
        public HasManyRelationshipBuilder<T> HasMany<TOther>(string name)
        {
            return Builder.HasMany<TOther>(name);
        }

        /// <summary>
        /// The builder instance.
        /// </summary>
        protected TBuilder Builder { get; }
    }

    public abstract class DelegatingContractBuilder<T> : DelegatingContractBuilder<T, IContractBuilder<T>>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The builder instance to delegate to.</param>
        protected DelegatingContractBuilder(IContractBuilder<T> builder) : base(builder) { }
    }
}