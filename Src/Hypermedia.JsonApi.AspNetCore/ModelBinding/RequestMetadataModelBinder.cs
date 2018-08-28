using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Hypermedia.Metadata;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hypermedia.JsonApi.AspNetCore.ModelBinding
{
    public sealed class RequestMetadataModelBinder : IModelBinder
    {
        readonly IContractResolver _contractResolver;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contractResolver">The contract resolver.</param>
        public RequestMetadataModelBinder(IContractResolver contractResolver)
        {
            _contractResolver = contractResolver;
        }

        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            if (_contractResolver.TryResolve(bindingContext.ModelType.GenericTypeArguments[0], out var root) == false)
            {
                return Task.CompletedTask;
            }

            var type = typeof(JsonApiRequestMetadata<>).MakeGenericType(bindingContext.ModelType.GenericTypeArguments[0]);

            var constructor = type.GetConstructor(new[] { typeof(IContractResolver), typeof(IContract), typeof(HttpRequest) });
            Debug.Assert(constructor != null);

            bindingContext.Result = ModelBindingResult.Success(constructor.Invoke(new object[] { _contractResolver, root, bindingContext.HttpContext.Request }));

            return Task.CompletedTask;
        }
    }
}