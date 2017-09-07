using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Saule.Queries;
using Saule.Queries.Pagination;
using Saule.Serialization;

namespace Saule.Http
{
    using System.Linq;

    /// <summary>
    /// Processes JSON API responses to enable filtering, pagination and sorting.
    /// </summary>
    public class PreprocessingDelegatingHandler : DelegatingHandler
    {
        private readonly JsonApiConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreprocessingDelegatingHandler"/> class.
        /// </summary>
        /// <param name="config">The configuration parameters for JSON API serialization.</param>
        public PreprocessingDelegatingHandler(JsonApiConfiguration config)
        {
            _config = config;
        }

        internal static PreprocessResult PreprocessRequest(
            object content,
            HttpRequestMessage request,
            JsonApiConfiguration config)
        {
            var jsonApi = new JsonApiSerializer();
            jsonApi.JsonConverters.AddRange(config.JsonConverters);

            PrepareQueryContext(jsonApi, request, config);

            ApiResource resource = null;
            if (request.Properties.ContainsKey(Constants.PropertyNames.ResourceDescriptor))
            {
                resource = (ApiResource)request.Properties[Constants.PropertyNames.ResourceDescriptor];
            }
            else if (content != null && !(content is HttpError))
            {
                content = new JsonApiException(
                    ErrorType.Server,
                    "You must add a [ReturnsResourceAttribute] to action methods.")
                {
                    HelpLink = "https://github.com/joukevandermaas/saule/wiki"
                };
            }

            if (!(content is HttpError) && jsonApi.QueryContext?.Pagination?.PerPage > jsonApi.QueryContext?.Pagination?.PageSizeLimit)
            {
                content = new JsonApiException(ErrorType.Client, "Page size exceeds page size limit for queries.");
            }

            PrepareUrlPathBuilder(jsonApi, request, config);

            return jsonApi.PreprocessContent(content, resource, request.RequestUri);
        }

        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = await base.SendAsync(request, cancellationToken);
            var hasMediaType = request.Headers.Accept.Any(x => x.MediaType == Constants.MediaType);

            var statusCode = (int)result.StatusCode;
            if (!hasMediaType || (statusCode >= 400 && statusCode < 500))
            {
                // probably malformed request or not found
                return result;
            }

            var value = result.Content as ObjectContent;

            var content = PreprocessRequest(value?.Value, request, _config);

            if (content.ErrorContent != null)
            {
                result.StatusCode = ApiError.IsClientError(content.ErrorContent)
                    ? HttpStatusCode.BadRequest
                    : HttpStatusCode.InternalServerError;
            }

            request.Properties.Add(Constants.PropertyNames.PreprocessResult, content);

            return result;
        }

        private static void PrepareUrlPathBuilder(
            JsonApiSerializer jsonApiSerializer,
            HttpRequestMessage request,
            JsonApiConfiguration config)
        {
            if (config.UrlPathBuilder != null)
            {
                jsonApiSerializer.UrlPathBuilder = config.UrlPathBuilder;
            }
            else if (!request.Properties.ContainsKey(Constants.PropertyNames.WebApiRequestContext))
            {
                jsonApiSerializer.UrlPathBuilder = new DefaultUrlPathBuilder();
            }
            else
            {
                var requestContext = request.Properties[Constants.PropertyNames.WebApiRequestContext]
                    as HttpRequestContext;
                var routeTemplate = requestContext?.RouteData.Route.RouteTemplate;
                var virtualPathRoot = requestContext?.VirtualPathRoot ?? "/";

                jsonApiSerializer.UrlPathBuilder = new DefaultUrlPathBuilder(
                    virtualPathRoot, routeTemplate);
            }
        }

        private static void PrepareQueryContext(
            JsonApiSerializer jsonApiSerializer,
            HttpRequestMessage request,
            JsonApiConfiguration config)
        {
            if (!request.Properties.ContainsKey(Constants.PropertyNames.QueryContext))
            {
                return;
            }

            var queryContext = (QueryContext)request.Properties[Constants.PropertyNames.QueryContext];

            if (queryContext.Filtering != null)
            {
                queryContext.Filtering.QueryFilters = config.QueryFilterExpressions;
            }

            jsonApiSerializer.QueryContext = queryContext;
        }
    }
}
