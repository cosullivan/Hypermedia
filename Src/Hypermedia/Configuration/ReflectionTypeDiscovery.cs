using System.Reflection;

namespace Hypermedia.Configuration
{
    public sealed class ReflectionTypeDiscovery : ITypeDiscovery
    {
        /// <summary>
        /// Discover the details about the given type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="parent">The parent builder to add to.</param>
        /// <returns>An entity builder to continue builder.</returns>
        public ContractBuilder<TEntity> Discover<TEntity>(IBuilder parent)
        {
            var builder = new ContractBuilder<TEntity>(parent);

            Discover(builder, typeof (TEntity).GetTypeInfo());

            return builder;
        }

        /// <summary>
        /// Discover the property from the given type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="builder">The parent builder to add to.</param>
        /// <param name="type">The type to discover the properties from.</param>
        static void Discover<TEntity>(ContractBuilder<TEntity> builder, TypeInfo type)
        {
            foreach (var property in type.DeclaredProperties)
            {
                builder.Field(property.Name);
            }

            if (type.BaseType != null && type.BaseType != typeof(object))
            {
                Discover(builder, type.BaseType.GetTypeInfo());
            }
        }
    }
}
