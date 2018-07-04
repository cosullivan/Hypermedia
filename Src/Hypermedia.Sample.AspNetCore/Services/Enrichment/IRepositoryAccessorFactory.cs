using System;
using Hypermedia.Metadata;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.AspNetCore.Services.Enrichment
{
    public interface IRepositoryAccessorFactory
    {
        /// <summary>
        /// Returns an instance of a resource repository accessor for a Belongs To relationship.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to return.</typeparam>
        /// <param name="relationship">The relationship that defines the accessor to return.</param>
        /// <returns>The resource repository accessor instance.</returns>
        Func<IDatabase, IRepositoryAccessor<TEntity>> GetOrCreateBelongsToAccessor<TEntity>(IBelongsToRelationship relationship);

        /// <summary>
        /// Returns an instance of a resource repository accessor for a Has Many relationship.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to return.</typeparam>
        /// <param name="relationship">The relationship that defines the accessor to return.</param>
        /// <returns>The resource repository accessor instance.</returns>
        Func<IDatabase, IRepositoryAccessor<TEntity>> GetOrCreateHasManyAccessor<TEntity>(IHasManyRelationship relationship);
    }
}