using System;
using Hypermedia.AspNetCore;
using Hypermedia.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hypermedia.JsonApi.AspNetCore.ModelBinding
{
    public sealed class RequestMetadataModelBinderProvider : IModelBinderProvider
    {
        readonly IContractResolver _contractResolver;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contract resolver.</param>
        public RequestMetadataModelBinderProvider(IContractResolver contractResolver)
        {
            _contractResolver = contractResolver;
        }

        /// <inheritdoc />
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType.IsGenericType && context.Metadata.ModelType.GetGenericTypeDefinition() == typeof(IRequestMetadata<>))
            {
                return new RequestMetadataModelBinder(_contractResolver);
            }

            return null;
        }
    }
}