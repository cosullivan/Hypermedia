using System.Linq;
using System.Reflection;
using Hypermedia.Metadata.Runtime;

namespace Hypermedia.Configuration
{
    public sealed class ReflectionTypeDiscovery : ITypeDiscovery
    {
        readonly IFieldDiscovery _fieldDiscovery;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReflectionTypeDiscovery() : this(FieldDiscovery.Deep) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fieldDiscovery">The field discovery to use.</param>
        public ReflectionTypeDiscovery(IFieldDiscovery fieldDiscovery)
        {
            _fieldDiscovery = fieldDiscovery;
        }

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
        void Discover<TEntity>(ContractBuilder<TEntity> builder, TypeInfo type)
        {
            bool isRecordType = typeof(TEntity).GetRuntimeMethods().Any(m => m.Name == "<Clone>$");
            foreach (var property in _fieldDiscovery.Discover(type))
            {
                if (isRecordType && property.Name == "EqualityContract") continue;
                builder
                    .Field(property.Name)
                    .Accessor(new RuntimeFieldAccessor(property))
                    .Options(RuntimeField.CreateDefaultOptions(property));
            }
        }
    }
}