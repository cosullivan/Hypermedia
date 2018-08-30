using System;
using Hypermedia.Metadata;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Hypermedia.AspNetCore.Mvc.Formatters
{
    public abstract class HypermediaInputFormatter : TextInputFormatter
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contract resolver to use.</param>
        protected HypermediaInputFormatter(IContractResolver contractResolver)
        {
            ContractResolver = contractResolver;
        }

        /// <summary>
        /// Determines whether this <see cref="T:Microsoft.AspNetCore.Mvc.Formatters.InputFormatter" /> can deserialize an object of the given
        /// <paramref name="type" />.
        /// </summary>
        /// <param name="type">The <see cref="T:System.Type" /> of object that will be read.</param>
        /// <returns><c>true</c> if the <paramref name="type" /> can be read, otherwise <c>false</c>.</returns>
        protected override bool CanReadType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return ContractResolver.CanResolve(TypeHelper.GetUnderlyingType(type));
        }

        /// <summary>
        /// The contract resolver to use for the formatter.
        /// </summary>
        public IContractResolver ContractResolver { get; }
    }
}