using System;
using System.Collections.Generic;
using System.Text;
using Saule;
using Saule.Http;
using Saule.Http.Formatters;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extension methods for the <see cref="IServiceCollection"/> class.
    /// </summary>
    public static class JsonApiServiceCollectionExtensions
    {
        /// <summary>
        /// Sets up serialization and deserialization of Json Api resources.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> that is used in the setup of the application.</param>
        /// <returns>The <see cref="IMvcBuilder"/> returned by the wrapped AddMvc() call</returns>
        public static IMvcBuilder AddJsonApi(this IServiceCollection services)
        {
            return AddJsonApi(services, null);
        }

        /// <summary>
        /// Sets up serialization and deserialization of Json Api resources.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> that is used in the setup of the application.</param>
        /// <param name="config">Configuration parameters for Json Api serialization.</param>
        /// <returns>The <see cref="IMvcBuilder"/> returned by the wrapped AddMvc() call</returns>
        public static IMvcBuilder AddJsonApi(this IServiceCollection services, JsonApiConfiguration config)
        {
            return AddJsonApi(services, config, false);
        }

        /// <summary>
        /// Sets up serialization and deserialization of Json Api resources.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> that is used in the setup of the application.</param>
        /// <param name="config">Configuration parameters for Json Api serialization.</param>
        /// <param name="overwriteOtherFormatters">
        /// If true, other formatters will be cleared. Otherwise, the JSON API formatter
        /// will be inserted at the start of the collection.
        /// </param>
        /// <returns>The <see cref="IMvcBuilder"/> returned by the wrapped AddMvc() call</returns>
        public static IMvcBuilder AddJsonApi(
            this IServiceCollection services,
            JsonApiConfiguration config,
            bool overwriteOtherFormatters)
        {
            if (overwriteOtherFormatters)
            {
                return AddJsonApi(services, config, FormatterPriority.OverwriteOtherFormatters);
            }
            else
            {
                return AddJsonApi(services, config, FormatterPriority.AddFormatterToStart);
            }
        }

        /// <summary>
        /// Sets up serialization and deserialization of Json Api resources.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> that is used in the setup of the application.</param>
        /// <param name="config">Configuration parameters for Json Api serialization.</param>
        /// <param name="formatterPriority"> Determines the relative position of the JSON API formatter.</param>
        /// <returns>The <see cref="IMvcBuilder"/> returned by the wrapped AddMvc() call</returns>
        public static IMvcBuilder AddJsonApi(this IServiceCollection services, JsonApiConfiguration config, FormatterPriority formatterPriority)
        {
            if (config != null)
            {
                services.AddSingleton<JsonApiConfiguration>(config);
            }

            return services.AddMvc(options =>
            {
                var inputFormatter = new JsonApiInputFormatter();
                var outputFormatter = new JsonApiOutputFormatter();

                if (formatterPriority == FormatterPriority.OverwriteOtherFormatters)
                {
                    options.InputFormatters.Clear();
                    options.OutputFormatters.Clear();

                    options.InputFormatters.Add(inputFormatter);
                    options.OutputFormatters.Add(outputFormatter);
                }
                else if (formatterPriority == FormatterPriority.AddFormatterToEnd)
                {
                    options.InputFormatters.Add(inputFormatter);
                    options.OutputFormatters.Add(outputFormatter);
                }
                else if (formatterPriority == FormatterPriority.AddFormatterToStart)
                {
                    options.InputFormatters.Insert(0, inputFormatter);
                    options.OutputFormatters.Insert(0, outputFormatter);
                }

                options.ReturnHttpNotAcceptable = true;

                options.Filters.Add<PreprocessingFilter>();
            });
        }
    }
}
