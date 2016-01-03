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
            config.MessageHandlers.Add(new PreprocessingDelegatingHandler(jsonApiConfiguration));
            config.Formatters.Clear();
            config.Formatters.Add(new JsonApiMediaTypeFormatter(jsonApiConfiguration));
        }
    }
}
