using System.Web.Http;

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
            config.Formatters.Clear();
#pragma warning disable 618
            config.Formatters.Add(new JsonApiMediaTypeFormatter(jsonApiConfiguration));
#pragma warning restore 618
        }
    }
}
