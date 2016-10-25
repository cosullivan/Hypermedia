using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;
using Hypermedia.Metadata;

namespace Hypermedia.JsonApi.WebApi
{
    public sealed class JsonApiRequestMetadataParameterBinding : HttpParameterBinding
    {
        readonly IContractResolver _contractResolver;
        readonly Type _resourceType;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Http.Controllers.HttpParameterBinding" /> class.
        /// </summary>
        /// <param name="descriptor">An <see cref="T:System.Web.Http.Controllers.HttpParameterDescriptor" /> that describes the parameters.</param>
        /// <param name="contractResolver">The contract resolver.</param>
        /// <param name="resourceType">The resource type that is being bound.</param>
        public JsonApiRequestMetadataParameterBinding(HttpParameterDescriptor descriptor, IContractResolver contractResolver, Type resourceType) : base(descriptor)
        {
            _contractResolver = contractResolver;
            _resourceType = resourceType;
        }

        /// <summary>
        /// Asynchronously executes the binding for the given request.
        /// </summary>
        /// <param name="metadataProvider">Metadata provider to use for validation.</param>
        /// <param name="actionContext">The action context for the binding. The action context contains the parameter dictionary that will get populated with the parameter.</param>
        /// <param name="cancellationToken">Cancellation token for cancelling the binding operation.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public override Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider, HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            IContract root;
            if (_contractResolver.TryResolve(_resourceType, out root) == false)
            {
                return Task.FromResult(0);
            }

            var type = typeof(JsonApiRequestMetadata<>).MakeGenericType(_resourceType);

            var constructor = type.GetConstructor(new[] { typeof(IContractResolver), typeof(IContract), typeof(HttpRequestMessage) });
            Debug.Assert(constructor != null);

            actionContext.ActionArguments[Descriptor.ParameterName] = constructor.Invoke(new object[] { _contractResolver, root, actionContext.Request });

            return Task.FromResult(0);
        }
    }
}