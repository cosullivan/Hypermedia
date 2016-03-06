using System;
using System.Collections.Generic;
using System.Linq;

namespace Hypermedia.Metadata
{
    public interface IContract
    {
        /// <summary>
        /// Gets the name of the resource.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the CLR type that the resource maps to.
        /// </summary>
        Type ClrType { get; }

        /// <summary>
        /// Gets a list of the fields that are available on the resource.
        /// </summary>
        IReadOnlyList<IField> Fields { get; }

        /// <summary>
        /// Gets a list of relationships that are available on the resource.
        /// </summary>
        IReadOnlyList<IRelationship> Relationships { get; }
    }

    public static class ContractExtensions
    {
        /// <summary>
        /// Creates an instance of the CLR type.
        /// </summary>
        /// <param name="type">The resource to create the CLR instance type for.</param>
        /// <returns>The CLR instance type that is mapped to the resource type.</returns>
        public static object CreateInstance(this IContract type)
        {
            return Activator.CreateInstance(type.ClrType);
        }

        /// <summary>
        /// Returns the field with the given name.
        /// </summary>
        /// <param name="contract">The type to return the field from.</param>
        /// <param name="name">The name of the field to return.</param>
        /// <returns>The field with the given name.</returns>
        public static IField Field(this IContract contract, string name)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(nameof(name));
            }

            return contract.Fields.Single(field => String.Equals(field.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns all fields that have the supplied set of options.
        /// </summary>
        /// <param name="contract">The contract to return the fields from.</param>
        /// <param name="option">The option to return the fields with.</param>
        /// <returns>The list of fields that have the given option.</returns>
        public static IEnumerable<IField> Fields(this IContract contract, FieldOptions option)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            return contract.Fields.Where(field => field.Is(option)).ToList();
        }

        /// <summary>
        /// Returns all fields that match the predicate.
        /// </summary>
        /// <param name="contract">The contract to return the fields from.</param>
        /// <param name="predicate">The predicate to apply to the fields to determine if they should be returned.</param>
        /// <returns>The list of fields that match the predicate.</returns>
        public static IEnumerable<IField> Fields(this IContract contract, Func<IField, bool> predicate)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            return contract.Fields.Where(predicate).ToList();
        }

        /// <summary>
        /// Gets the ID for the entity.
        /// </summary>
        /// <param name="contract">The contract to perform the operation on.</param>
        /// <param name="instance">The instance to return the ID from.</param>
        /// <returns>The ID for the given instance.</returns>
        public static object GetId(this IContract contract, object instance)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            return contract.Fields(FieldOptions.Id).Single().GetValue(instance);
        }
    }
}
