using System;
using Hypermedia.Metadata;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Hypermedia.AspNetCore.Mvc.Formatters
{
    public abstract class HypermediaOutputFormatter : TextOutputFormatter
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contract resolver to use.</param>
        protected HypermediaOutputFormatter(IContractResolver contractResolver)
        {
            ContractResolver = contractResolver;
        }

        /// <summary>
        /// Returns a value indicating whether or not the given type can be written by this serializer.
        /// </summary>
        /// <param name="type">The object type.</param>
        /// <returns><c>true</c> if the type can be written, otherwise <c>false</c>.</returns>
        protected override bool CanWriteType(Type type)
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