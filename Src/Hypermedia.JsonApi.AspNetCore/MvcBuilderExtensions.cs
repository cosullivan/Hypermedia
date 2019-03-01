using System;
using Hypermedia.AspNetCore.Json.Formatters;
using Hypermedia.Json;
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
        public static IMvcBuilder AddHypermediaFormatters(this IMvcBuilder builder, IContractResolver contractResolver)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contractResolver == null)
            {
                throw new ArgumentNullException(nameof(contractResolver));
            }

            return builder.AddHypermediaFormatters(contractResolver, DasherizedFieldNamingStrategy.Instance);
        }

        /// <summary>
        /// Configures MVC to use the Hypermedia formatters.
        /// </summary>
        /// <param name="builder">The builder to configure the options on.</param>
        /// <param name="contractResolver">The contract resolver to use for the formatters.</param>
        /// <param name="fieldNamingStrategy">The field naming strategy to use by default.</param>
        /// <returns>The builder to continue building on.</returns>
        public static IMvcBuilder AddHypermediaFormatters(this IMvcBuilder builder, IContractResolver contractResolver, IFieldNamingStrategy fieldNamingStrategy)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contractResolver == null)
            {
                throw new ArgumentNullException(nameof(contractResolver));
            }

            return AddHypermediaFormatters(builder,
                options =>
                {
                    options.ContractResolver = contractResolver;
                    options.FieldNamingStrategy = fieldNamingStrategy;
                });
        }

        /// <summary>
        /// Configures MVC to use the Hypermedia formatters.
        /// </summary>
        /// <param name="builder">The builder to configure the options on.</param>
        /// <param name="configure">The action to configure the hypermedia formatting.</param>
        /// <returns>The builder to continue building on.</returns>
        public static IMvcBuilder AddHypermediaFormatters(this IMvcBuilder builder, Action<HypermediaFormattingOptions> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var options = new HypermediaFormattingOptions
            {
                FieldNamingStrategy = DefaultFieldNamingStrategy.Instance,
                JsonApiSerializerOptions = new JsonApiSerializerOptions { }
            };

            configure(options);

            // ensure the defaults are set for the JsonApiSerializerOptions
            options.JsonApiSerializerOptions.ContractResolver = options.JsonApiSerializerOptions.ContractResolver ?? options.ContractResolver;
            options.JsonApiSerializerOptions.FieldNamingStrategy = options.JsonApiSerializerOptions.FieldNamingStrategy ?? options.FieldNamingStrategy;
            
            return builder.AddMvcOptions(mvcOptions =>
            {
                // note that the order these formatters are registered is very important due to the way they are selected
                // the selection is based on Content-Type specificity so that when we output the application/json outputer
                // wont attempt to write the output for application/vnd.api+json because it's less specific
                mvcOptions.OutputFormatters.Insert(0, new JsonOutputFormatter(options.ContractResolver, options.FieldNamingStrategy));
                mvcOptions.OutputFormatters.Insert(1, new JsonApiOutputFormatter(options.JsonApiSerializerOptions));

                // the order of the input formatters needs to be reversed from the output formatters
                mvcOptions.InputFormatters.Insert(0, new JsonApiInputFormatter(options.JsonApiSerializerOptions));
                mvcOptions.InputFormatters.Insert(1, new JsonInputFormatter(options.ContractResolver, options.FieldNamingStrategy));

                mvcOptions.ModelBinderProviders.Insert(0, new RequestMetadataModelBinderProvider(options.ContractResolver));

                mvcOptions.FormatterMappings.SetMediaTypeMappingForFormat(
                    "json", 
                    MediaTypeHeaderValue.Parse(JsonOutputFormatter.JsonMediaTypeName));

                mvcOptions.FormatterMappings.SetMediaTypeMappingForFormat(
                    "jsonapi", 
                    MediaTypeHeaderValue.Parse(JsonApiOutputFormatter.JsonApiMediaTypeName));
            });
        }
    }
}