using System.Linq;
using System.Net;
using System.Net.Http;
using Saule.Serialization;

namespace Saule.Http
{
    /// <summary>
    /// Processes JSON API responses
    /// </summary>
    internal static class JsonApiProcessor
    {
        internal static void ProcessRequest(HttpRequestMessage request, HttpResponseMessage response, JsonApiConfiguration config, bool requiresMediaType)
        {
            var hasMediaType = request.Headers.Accept.Any(x => x.MediaType == Constants.MediaType);

            var statusCode = (int)response.StatusCode;
            if ((requiresMediaType && !hasMediaType) || (statusCode >= 400 && statusCode < 500))
            {
                // probably malformed request or not found
                return;
            }

            var value = response.Content as ObjectContent;

            if (config == null)
            {
                config = new JsonApiConfiguration();
            }

            var content = PreprocessingDelegatingHandler.PreprocessRequest(value?.Value, request, config);

            if (content.ErrorContent != null)
            {
                response.StatusCode = ApiError.IsClientError(content.ErrorContent)
                    ? HttpStatusCode.BadRequest
                    : HttpStatusCode.InternalServerError;
            }

            request.Properties.Add(Constants.PropertyNames.PreprocessResult, content);
        }
    }
}
