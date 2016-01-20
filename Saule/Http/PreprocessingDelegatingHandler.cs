using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Saule.Queries;
using Saule.Serialization;

namespace Saule.Http
{
    internal class PreprocessingDelegatingHandler : DelegatingHandler
    {
        private readonly JsonApiConfiguration _config;

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
            var httpError = content as HttpError;
            if (httpError == null)
            {
                if (request.Properties.ContainsKey(Constants.RequestPropertyName))
                {
                    resource = (ApiResource) request.Properties[Constants.RequestPropertyName];
                }
                else
                {
                    content = new JsonApiException(ErrorType.Server,
                        "You must add a [ReturnsResourceAttribute] to action methods.")
                    {
                        HelpLink = "https://github.com/joukevandermaas/saule/wiki"
                    };
                }
            }

            PrepareUrlPathBuilder(jsonApi, request, config);

            return jsonApi.PreprocessContent(content, resource, request.RequestUri);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var result = await base.SendAsync(request, cancellationToken);

            var statusCode = (int) result.StatusCode;
            if (statusCode >= 400 && statusCode < 500)
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

            request.Properties.Add(Constants.PreprocessResultPropertyName, content);

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
            else if (!request.Properties.ContainsKey(Constants.WebApiRequestContextPropertyName))
            {
                jsonApiSerializer.UrlPathBuilder = new DefaultUrlPathBuilder();
            }
            else
            {
                var requestContext = request.Properties[Constants.WebApiRequestContextPropertyName]
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
            if (!request.Properties.ContainsKey(Constants.QueryContextPropertyName))
            {
                return;
            }

            var queryContext = (QueryContext) request.Properties[Constants.QueryContextPropertyName];

            if (queryContext.Filtering != null)
            {
                queryContext.Filtering.QueryFilters = config.QueryFilterExpressions;
            }

            jsonApiSerializer.QueryContext = queryContext;
        }
    }
}