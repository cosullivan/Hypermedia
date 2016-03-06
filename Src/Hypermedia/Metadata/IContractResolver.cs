using System;

namespace Hypermedia.Metadata
{
    public interface IContractResolver
    {
        /// <summary>
        /// Attempt to resolve the resource contract from a CLR type.
        /// </summary>
        /// <param name="type">The CLR type of the resource contract to resolve.</param>
        /// <param name="contract">The resource contract that was associated with the given CLR type.</param>
        /// <returns>true if the resource contract could be resolved, false if not.</returns>
        bool TryResolve(Type type, out IResourceContract contract);

        /// <summary>
        /// Attempt to resolve the resource contract from a resource type name.
        /// </summary>
        /// <param name="name">The resource type name of the resource contract to resolve.</param>
        /// <param name="contract">The resource contract that was associated with the given resource type name.</param>
        /// <returns>true if the resource contract could be resolved, false if not.</returns>
        bool TryResolve(string name, out IResourceContract contract);
    }

    public static class ContractResolverExtensions
    {
        /// <summary>
        /// Returns a value indicating whether or not the given type can be resolved.
        /// </summary>
        /// <param name="resolver">The contract resolver to perform the operation on.</param>
        /// <param name="type">The CLR type to test whether it can be resolved.</param>
        /// <returns>true if the given CLR type can be resolved, false if not.</returns>
        public static bool CanResolve(this IContractResolver resolver, Type type)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            IResourceContract resourceContract;
            return resolver.TryResolve(type, out resourceContract);
        }
    }
}
