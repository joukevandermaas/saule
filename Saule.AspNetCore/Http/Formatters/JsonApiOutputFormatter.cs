using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Saule.Http.Formatters
{
    /// <summary>
    /// Custom media type formatter for Json Api (1.0) responses and requests.
    /// </summary>
    public class JsonApiOutputFormatter : TextOutputFormatter
    {
        internal JsonApiOutputFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(Constants.MediaType));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        /// <inheritdoc/>
        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var preprocessed = context.HttpContext.Items[Constants.PropertyNames.PreprocessResult]
                as PreprocessResult;

            var json = JsonApiSerializer.Serialize(preprocessed);

            var response = context.HttpContext.Response;

            IServiceProvider serviceProvider = context.HttpContext.RequestServices;
            var config = serviceProvider.GetService(typeof(JsonApiConfiguration)) as JsonApiConfiguration;

            return WriteJsonToStream(json, response.Body, config?.JsonConverters.ToArray());
        }

        /// <inheritdoc/>
        protected override bool CanWriteType(Type type)
        {
            return true;
        }

        private async Task WriteJsonToStream(JToken json, Stream stream, JsonConverter[] jsonConverters)
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 2048, true))
            {
                await writer.WriteAsync(json.ToString(Formatting.None, jsonConverters));
            }
        }
    }
}