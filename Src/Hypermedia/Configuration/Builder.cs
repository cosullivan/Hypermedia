using System;
using System.Collections.Generic;
using System.Linq;
using Hypermedia.Metadata;

namespace Hypermedia.Configuration
{
    public sealed class Builder : IBuilder
    {
        readonly List<IContractBuilder> _builders = new List<IContractBuilder>();

        /// <summary>
        /// Build a resource contract resolver with the known types.
        /// </summary>
        /// <returns>The resource contract resolver that is aware of the types that were configured through the builder.</returns>
        public IContractResolver Build()
        {
            return new ContractResolver(_builders.Select(builder => builder.CreateContract()));
        }

        /// <summary>
        /// Returns a Resource Builder for a resource type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the resource to return the builder for.</typeparam>
        /// <param name="discovery">The type discovery mechanism.</param>
        /// <returns>The resource builder to configure.</returns>
        public ContractBuilder<TEntity> With<TEntity>(ITypeDiscovery discovery)
        {
            if (discovery == null)
            {
                throw new ArgumentNullException(nameof(discovery));
            }

            var builder = discovery.Discover<TEntity>(this);

            _builders.Add(builder);

            return builder;
        }
    }
}
