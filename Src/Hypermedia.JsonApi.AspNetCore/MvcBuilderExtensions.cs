using Hypermedia.AspNetCore.Json.Formatters;
using Hypermedia.AspNetCore.Mvc.Formatters;
using Hypermedia.JsonApi.AspNetCore.Formatters;
using Hypermedia.JsonApi.AspNetCore.ModelBinding;
using Hypermedia.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace Hypermedia.JsonApi.AspNetCore
{
    public static class MvcBuilderExtensions
    {
        /// <summary>
        /// Configures MVC to use the Hypermedia formatters.
        /// </summary>
        /// <param name="builder">The builder to configure the options on.</param>
        /// <param name="contractResolver">The contract resolver to use for the formatters.</param>
        /// <returns>The builder to continue building on.</returns>
        public static IMvcBuilder UseHypermediaFormatters(this IMvcBuilder builder, IContractResolver contractResolver)
        {
            if (builder == null)
            {
                throw new System.ArgumentNullException(nameof(builder));
            }

            if (contractResolver == null)
            {
                throw new System.ArgumentNullException(nameof(contractResolver));
            }

            return builder.AddMvcOptions(options =>
            {
                options.OutputFormatters.Insert(0, new JsonOutputFormatter(contractResolver));
                options.OutputFormatters.Insert(0, new JsonApiOutputFormatter(contractResolver));

                options.InputFormatters.Insert(0, new JsonInputFormatter(contractResolver));
                options.InputFormatters.Insert(0, new JsonApiInputFormatter(contractResolver));

                options.ModelBinderProviders.Insert(0, new RequestMetadataModelBinderProvider(contractResolver));
                //options.ModelBinderProviders.Insert(1, new JsonApiPatchModelBinderProvider(contractResolver));

                options.FormatterMappings.SetMediaTypeMappingForFormat(
                    "json", 
                    MediaTypeHeaderValue.Parse(JsonOutputFormatter.JsonMediaTypeName));

                options.FormatterMappings.SetMediaTypeMappingForFormat(
                    "jsonapi", 
                    MediaTypeHeaderValue.Parse(JsonApiOutputFormatter.JsonApiMediaTypeName));
            });
        }
    }
}