using System;
using Hypermedia.Metadata;

namespace Hypermedia.Configuration
{
    public interface IContractBuilder
    {
        /// <summary>
        /// Build the resource contract.
        /// </summary>
        /// <returns>The resource ontract.</returns>
        IContract CreateContract();
    }

    public interface IContractBuilder<T> : IBuilder
    {
        /// <summary>
        /// Returns a field.
        /// </summary>
        /// <param name="name">The name of the field to return.</param>
        /// <returns>The field builder build the field.</returns>
        FieldBuilder<T> Field(string name);

        /// <summary>
        /// Returns a BelongsTo relationship.
        /// </summary>
        /// <param name="name">The name of the relationship to return.</param>
        /// <returns>The relationship builder build the relationship.</returns>
        RelationshipBuilder<T> BelongsTo<TOther>(string name);

        /// <summary>
        /// Returns a HasMany relationship.
        /// </summary>
        /// <param name="name">The name of the relationship to return.</param>
        /// <returns>The relationship builder build the relationship.</returns>
        RelationshipBuilder<T> HasMany<TOther>(string name);
    }

    public static class ContractBuilderExtensions
    {
        /// <summary>
        /// Sets the given field as being the primary ID field.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="builder">The builder to perform the operation on.</param>
        /// <param name="name">The name of the field to set as the primary ID field.</param>
        /// <returns>The field builder to continue building on.</returns>
        public static FieldBuilder<TEntity> Id<TEntity>(this IContractBuilder<TEntity> builder, string name)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.Field(name).Id();
        }

        /// <summary>
        /// Sets the given field as being ignored.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="builder">The builder to perform the operation on.</param>
        /// <param name="name">The name of the field to ignore.</param>
        /// <returns>The field builder to continue building on.</returns>
        public static FieldBuilder<TEntity> Ignore<TEntity>(this IContractBuilder<TEntity> builder, string name)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.Field(name).Ignore();
        }

        /// <summary>
        /// Sets the given field as being read only.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="builder">The builder to perform the operation on.</param>
        /// <param name="name">The name of the field to set as read only.</param>
        /// <returns>The field builder to continue building on.</returns>
        public static FieldBuilder<TEntity> ReadOnly<TEntity>(this IContractBuilder<TEntity> builder, string name)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.Field(name).ReadOnly();
        }

        /// <summary>
        /// Sets the given field as being write-only.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="builder">The builder to perform the operation on.</param>
        /// <param name="name">The name of the field to set as write-only.</param>
        /// <returns>The field builder to continue building on.</returns>
        public static FieldBuilder<TEntity> WriteOnly<TEntity>(this IContractBuilder<TEntity> builder, string name)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.Field(name).WriteOnly();
        }

        /// <summary>
        /// Creates a caclulated field.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="builder">The builder to perform the operation on.</param>
        /// <param name="name">The name of the field to create as a calculated field.</param>
        /// <param name="expression">The expression to return the calculation.</param>
        /// <returns>The calculated field builder to continue building on.</returns>
        public static CalculatedFieldBuilder<TEntity, TValue> Calculated<TEntity, TValue>(
            this IContractBuilder<TEntity> builder, 
            string name, 
            Func<TEntity, TValue> expression)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.Field(name).Calculated(expression);
        }
    }
}
