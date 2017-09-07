using Saule;
using Saule.Http;
using Saule.Http.Formatters;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Provides extension methods for the <see cref="MvcOptions"/> class.
    /// </summary>
    public static class JsonApiMvcOptionsExtensions
    {
        /// <summary>
        /// Sets up serialization and deserialization of Json Api resources.
        /// </summary>
        /// <param name="options">The <see cref="MvcOptions"/> that is used in the setup of the application.</param>
        /// <returns>The <see cref="MvcOptions"/> for chaining.</returns>
        public static MvcOptions AddJsonApi(this MvcOptions options)
        {
            return AddJsonApi(options, new JsonApiConfiguration());
        }

        /// <summary>
        /// Sets up serialization and deserialization of Json Api resources.
        /// </summary>
        /// <param name="options">The <see cref="MvcOptions"/> that is used in the setup of the application.</param>
        /// <param name="jsonApiConfiguration">Configuration parameters for Json Api serialization.</param>
        /// <returns>The <see cref="MvcOptions"/> for chaining.</returns>
        public static MvcOptions AddJsonApi(this MvcOptions options, JsonApiConfiguration jsonApiConfiguration)
        {
            return AddJsonApi(options, jsonApiConfiguration, false);
        }

        /// <summary>
        /// Sets up serialization and deserialization of Json Api resources.
        /// </summary>
        /// <param name="options">The <see cref="MvcOptions"/> that is used in the setup of the application.</param>
        /// <param name="jsonApiConfiguration">Configuration parameters for Json Api serialization.</param>
        /// <param name="overwriteOtherFormatters">
        /// If true, other formatters will be cleared. Otherwise, the JSON API formatter
        /// will be inserted at the start of the collection.
        /// </param>
        /// <returns>The <see cref="MvcOptions"/> for chaining.</returns>
        public static MvcOptions AddJsonApi(
            this MvcOptions options,
            JsonApiConfiguration jsonApiConfiguration,
            bool overwriteOtherFormatters)
        {
            if (overwriteOtherFormatters)
            {
                return AddJsonApi(options, jsonApiConfiguration, FormatterPriority.OverwriteOtherFormatters);
            }
            else
            {
                return AddJsonApi(options, jsonApiConfiguration, FormatterPriority.AddFormatterToStart);
            }
        }

        /// <summary>
        ///  Sets up serialization and deserialization of Json Api resources.
        /// </summary>
        /// <param name="options">The <see cref="MvcOptions"/> that is used in the setup of the application.</param>
        /// <param name="jsonApiConfiguration">Configuration parameters for Json Api serialization.</param>
        /// <param name="formatterPriority"> Determines the relative position of the JSON API formatter.</param>
        /// <returns>The <see cref="MvcOptions"/> for chaining.</returns>
        public static MvcOptions AddJsonApi(
          this MvcOptions options,
          JsonApiConfiguration jsonApiConfiguration,
          FormatterPriority formatterPriority)
        {
            var inputFormatter = new JsonApiInputFormatter(jsonApiConfiguration);
            var outputFormatter = new JsonApiOutputFormatter(jsonApiConfiguration);

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

            options.Filters.Add<PreprocessingFilter>();

            return options;
        }
    }
}
