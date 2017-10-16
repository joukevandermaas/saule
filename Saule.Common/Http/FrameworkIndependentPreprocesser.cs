using System;
using System.Linq;
using System.Net;
using Saule.Serialization;

namespace Saule.Http
{

    /// <summary>
    /// Processes JSON API responses to enable filtering, pagination and sorting.
    /// </summary>
    internal class FrameworkIndependentPreprocesser
    {
        private readonly JsonApiConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkIndependentPreprocesser"/> class.
        /// </summary>
        /// <param name="config">The configuration parameters for JSON API serialization.</param>
        public FrameworkIndependentPreprocesser(JsonApiConfiguration config)
        {
            _config = config;
        }

        public PreprocessResult ProcessAfterActionMethod(PreprocessInput input)
        {
            if (input.StatusCode >= 400 && input.StatusCode < 500)
            {
                // probably malformed request or not found
                return new PreprocessResult
                {
                    StatusCode = input.StatusCode
                };
            }

            if (input.Content == null && !input.ErrorContent.Any())
            {
                // probably no JSON api output
                return new PreprocessResult
                {
                    StatusCode = input.StatusCode
                };
            }

            var processed = PreprocessRequest(input);

            if (processed.ErrorContent != null)
            {
                processed.StatusCode = (int) (ApiError.AnyClientError(processed.ErrorContent)
                    ? HttpStatusCode.BadRequest
                    : HttpStatusCode.InternalServerError);
            }
            else
            {
                processed.StatusCode = input.StatusCode;
            }

            return processed;
        }

        private PreprocessResult PreprocessRequest(PreprocessInput input)
        {
            var jsonApi = new JsonApiSerializer();
            jsonApi.JsonConverters.AddRange(_config.JsonConverters);

            PrepareQueryContext(jsonApi, input);

            object content = input.Content;

            if (!input.ErrorContent.Any() && input.ResourceDescriptor == null)
            {
                content = new JsonApiException(
                    ErrorType.Server,
                    "You must add a [ReturnsResourceAttribute] to action methods.")
                {
                    HelpLink = "https://github.com/joukevandermaas/saule/wiki"
                };
            }
            else if (!input.ErrorContent.Any() && jsonApi.QueryContext?.Pagination?.PerPage > jsonApi.QueryContext?.Pagination?.PageSizeLimit)
            {
                content = new JsonApiException(ErrorType.Client, "Page size exceeds page size limit for queries.");
            } else if (input.ErrorContent.Any())
            {
                content = input.ErrorContent;
            }

            PrepareUrlPathBuilder(jsonApi, input);

            return jsonApi.PreprocessContent(content, input.ResourceDescriptor, input.RequestUri);
        }

        private void PrepareUrlPathBuilder(JsonApiSerializer jsonApiSerializer, PreprocessInput input)
        {
            if (_config.UrlPathBuilder != null)
            {
                jsonApiSerializer.UrlPathBuilder = _config.UrlPathBuilder;
            }
            else
            {
                var routeTemplate = input.RouteTemplate;
                var virtualPathRoot = input.VirtualPathRoot ?? "/";

                jsonApiSerializer.UrlPathBuilder = new DefaultUrlPathBuilder(
                    virtualPathRoot, routeTemplate);
            }
        }

        private void PrepareQueryContext(
            JsonApiSerializer jsonApiSerializer,
            PreprocessInput input)
        {
            var queryContext = input.QueryContext;

            if (queryContext == null)
            {
                return;
            }

            if (queryContext.Filtering != null)
            {
                queryContext.Filtering.QueryFilters = _config.QueryFilterExpressions;
            }

            jsonApiSerializer.QueryContext = queryContext;
        }
    }
}
