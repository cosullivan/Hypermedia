namespace Hypermedia.Configuration
{
    public interface ITypeDiscovery
    {
        /// <summary>
        /// Discover the details about the given type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="parent">The parent builder to add to.</param>
        /// <returns>An entity builder to continue builder.</returns>
        ResourceBuilder<TEntity> Discover<TEntity>(IBuilder parent);
    }
}
