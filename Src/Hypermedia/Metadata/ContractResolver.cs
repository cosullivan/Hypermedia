using System;
using System.Collections.Generic;
using System.Linq;

namespace Hypermedia.Metadata
{
    public sealed class ContractResolver : IContractResolver
    {
        readonly IReadOnlyList<IResourceContract> _contracts;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contracts">The list of contracts available to the resolver.</param>
        public ContractResolver(IEnumerable<IResourceContract> contracts)
        {
            _contracts = contracts.ToList();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contract">The contract available to the resolver.</param>
        public ContractResolver(IResourceContract contract) : this(new [] { contract }) { }

        /// <summary>
        /// Attempt to resolve the resource contract from a CLR type.
        /// </summary>
        /// <param name="type">The CLR type of the resource contract to resolve.</param>
        /// <param name="contract">The resource contract that was associated with the given CLR type.</param>
        /// <returns>true if the resource contract could be resolved, false if not.</returns>
        public bool TryResolve(Type type, out IResourceContract contract)
        {
            contract = _contracts.SingleOrDefault(t => t.ClrType == type);

            return contract != null;
        }

        /// <summary>
        /// Attempt to resolve the resource contract from a resource type name.
        /// </summary>
        /// <param name="name">The resource type name of the resource contract to resolve.</param>
        /// <param name="contract">The resource contract that was associated with the given resource type name.</param>
        /// <returns>true if the resource contract could be resolved, false if not.</returns>
        public bool TryResolve(string name, out IResourceContract contract)
        {
            contract = _contracts.SingleOrDefault(t => String.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));

            return contract != null;
        }
    }
}
