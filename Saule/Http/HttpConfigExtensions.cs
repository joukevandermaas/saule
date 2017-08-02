using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Saule.Queries;
using Saule.Serialization;

namespace Saule.Http
{
    /// <summary>
    /// Provides extension methods for the <see cref="HttpConfiguration"/> class.
    /// </summary>
    public static class HttpConfigExtensions
    {
        /// <summary>
        /// Sets up serialization and deserialization of Json Api resources.
        /// </summary>
        /// <param name="config">The <see cref="HttpConfiguration"/> that is used in the setup of the application.</param>
        public static void ConfigureJsonApi(this HttpConfiguration config)
        {
            ConfigureJsonApi(config, new JsonApiConfiguration());
        }

        /// <summary>
        /// Sets up serialization and deserialization of Json Api resources.
        /// </summary>
        /// <param name="config">The <see cref="HttpConfiguration"/> that is used in the setup of the application.</param>
        /// <param name="jsonApiConfiguration">Configuration parameters for Json Api serialization.</param>
        public static void ConfigureJsonApi(this HttpConfiguration config, JsonApiConfiguration jsonApiConfiguration)
        {
            ConfigureJsonApi(config, jsonApiConfiguration, false);
        }

        /// <summary>
        /// Sets up serialization and deserialization of Json Api resources.
        /// </summary>
        /// <param name="config">The <see cref="HttpConfiguration"/> that is used in the setup of the application.</param>
        /// <param name="jsonApiConfiguration">Configuration parameters for Json Api serialization.</param>
        /// <param name="overwriteOtherFormatters">
        /// If true, other formatters will be cleared. Otherwise, the JSON API formatter
        /// will be inserted at the start of the collection.
        /// </param>
        public static void ConfigureJsonApi(
            this HttpConfiguration config,
            JsonApiConfiguration jsonApiConfiguration,
            bool overwriteOtherFormatters)
        {
            if (overwriteOtherFormatters)
            {
                ConfigureJsonApi(config, jsonApiConfiguration, ConfigureFormattersEnum.OverwriteOtherFormatters);
            }
            else
            {
                ConfigureJsonApi(config, jsonApiConfiguration, ConfigureFormattersEnum.AddFormatterToStart);
            }
        }

        /// <summary>
        ///  Sets up serialization and deserialization of Json Api resources.
        /// </summary>
        /// <param name="config">The <see cref="HttpConfiguration"/> that is used in the setup of the application.</param>
        /// <param name="jsonApiConfiguration">Configuration parameters for Json Api serialization.</param>
        /// <param name="configureFormatters">
        /// AddFormatterToStart - Formatter will be inserted at the start of the collection.
        /// AddFormatterToEnd - Formatter will be inserted at the end of the collection.
        /// OverwriteOtherFormatters -  Other formatters will be cleared.
        /// </param>
        public static void ConfigureJsonApi(
          this HttpConfiguration config,
          JsonApiConfiguration jsonApiConfiguration,
          ConfigureFormattersEnum configureFormatters)
        {
            config.MessageHandlers.Add(new PreprocessingDelegatingHandler(jsonApiConfiguration));
            var formatter = new JsonApiMediaTypeFormatter(jsonApiConfiguration);

            if (configureFormatters == ConfigureFormattersEnum.OverwriteOtherFormatters)
            {
                config.Formatters.Clear();
                config.Formatters.Add(formatter);
            }
            else if (configureFormatters == ConfigureFormattersEnum.AddFormatterToEnd)
            {
                config.Formatters.Add(formatter);
            }
            else if (configureFormatters == ConfigureFormattersEnum.AddFormatterToStart)
            {
                config.Formatters.Insert(0, formatter);
            }
        }
    }
}
