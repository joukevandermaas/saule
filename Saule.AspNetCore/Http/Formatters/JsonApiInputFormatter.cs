using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saule.Serialization;

namespace Saule.Http.Formatters
{
    /// <summary>
    /// Custom media type formatter for Json Api (1.0) responses and requests.
    /// </summary>
    public class JsonApiInputFormatter : TextInputFormatter
    {
        internal JsonApiInputFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(Constants.MediaType));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        /// <inheritdoc/>
        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        => ReadRequestBodyAsync(context, Encoding.UTF8);

        /// <inheritdoc/>
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            var readStream = context.HttpContext.Request.Body;

            using (var reader = new StreamReader(readStream, encoding))
            {
                try
                {
                    var json = JToken.Parse(await reader.ReadToEndAsync());
                    var obj = new ResourceDeserializer(json, context.ModelType).Deserialize();
                    return InputFormatterResult.Success(obj);
                }
                catch (JsonApiException ex)
                {
                    context.ModelState.AddModelError("Request content", ex.Message);
                    return InputFormatterResult.Failure();
                }
                catch (JsonReaderException ex)
                {
                    context.ModelState.AddModelError("Request content", ex.Message);
                    return InputFormatterResult.Failure();
                }
            }
        }

        /// <inheritdoc/>
        protected override bool CanReadType(Type type)
        {
            return true;
        }
    }
}