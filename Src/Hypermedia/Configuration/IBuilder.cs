using System;
using Hypermedia.Metadata;

namespace Hypermedia.Configuration
{
    public interface IBuilder
    {
        /// <summary>
        /// Build a resource contract resolver with the known types.
        /// </summary>
        /// <returns>The resource contract resolver that is aware of the types that were configured through the builder.</returns>
        IContractResolver Build();

        /// <summary>
        /// Returns a Resource Builder for a resource type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the resource to return the builder for.</typeparam>
        /// <param name="discovery">The type discovery mechanism.</param>
        /// <returns>The resource builder to configure.</returns>
        ContractBuilder<TEntity> With<TEntity>(ITypeDiscovery discovery);
    }

    public static class BuilderExtensions
    {
        /// <summary>
        /// Returns a Resource Builder for a resource type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the resource to return the builder for.</typeparam>
        /// <param name="builder">The builder to perform the operation on.</param>
        /// <returns>The resource builder to configure.</returns>
        public static ContractBuilder<TEntity> With<TEntity>(this IBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.With<TEntity>(typeof(TEntity).Name);
        }

        /// <summary>
        /// Returns a Resource Builder for a resource type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the resource to return the builder for.</typeparam>
        /// <param name="builder">The builder to perform the operation on.</param>
        /// <param name="name">The name of the resource type.</param>
        /// <returns>The resource builder to configure.</returns>
        public static ContractBuilder<TEntity> With<TEntity>(this IBuilder builder, string name)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.With<TEntity>(new ReflectionTypeDiscovery()).Name(name);
        }
    }
}