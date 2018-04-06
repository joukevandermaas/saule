using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Filters;
using Saule.Serialization;

namespace Saule.Http
{
    /// <summary>
    /// Processes JSON API responses
    /// </summary>
    public class JsonApiProcessor
    {
        /// <summary>
        /// process request as JSON API response
        /// </summary>
        /// <param name="context">context</param>
        public static void ProcessRequest(HttpActionExecutedContext context)
        {
            var statusCode = (int)context.Response.StatusCode;
            if (statusCode >= 400 && statusCode < 500)
            {
                // probably malformed request or not found
                return;
            }

            var value = context.Response.Content as ObjectContent;

            var config = new JsonApiConfiguration();

            var content = PreprocessingDelegatingHandler.PreprocessRequest(value?.Value, context.Request, config);

            if (content.ErrorContent != null)
            {
                context.Response.StatusCode = ApiError.IsClientError(content.ErrorContent)
                    ? HttpStatusCode.BadRequest
                    : HttpStatusCode.InternalServerError;
            }

            context.Request.Properties.Add(Constants.PropertyNames.PreprocessResult, content);

            if (context.Exception != null)
            {
                return;
            }

            var responseContent = context.Response.Content as ObjectContent;
            if (responseContent == null)
            {
                return;
            }

            var formatter = new JsonApiMediaTypeFormatter(config).GetPerRequestFormatterInstance(
                typeof(string),
                context.Request,
                new MediaTypeHeaderValue(Constants.MediaType));

            context.Response.Content = new ObjectContent(responseContent.ObjectType, responseContent.Value, formatter);
        }
    }
}
