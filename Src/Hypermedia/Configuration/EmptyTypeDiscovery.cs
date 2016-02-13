namespace Hypermedia.Configuration
{
    public sealed class EmptyTypeDiscovery : ITypeDiscovery
    {
        /// <summary>
        /// Discover the details about the given type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="parent">The parent builder to add to.</param>
        /// <returns>An entity builder to continue builder.</returns>
        public ResourceBuilder<TEntity> Discover<TEntity>(IBuilder parent)
        {
            return new ResourceBuilder<TEntity>(parent);
        }
    }
}
