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
            var jsonApi = new JsonApiSerializer
            {
                UrlPathBuilder = config.UrlPathBuilder
            };
            jsonApi.JsonConverters.AddRange(config.JsonConverters);

            if (request.Properties.ContainsKey(Constants.QueryContextPropertyName))
            {
                var queryContext = (QueryContext)request.Properties[Constants.QueryContextPropertyName];

                if (queryContext.Filtering != null)
                {
                    queryContext.Filtering.QueryFilters = config.QueryFilterExpressions;
                }

                jsonApi.QueryContext = queryContext;
            }

            ApiResource resource = null;
            if (request.Properties.ContainsKey(Constants.RequestPropertyName))
            {
                resource = (ApiResource)request.Properties[Constants.RequestPropertyName];
            }
            else
            {
                content = new JsonApiException(ErrorType.Server, "You must add a [ReturnsResourceAttribute] to action methods.")
                {
                    HelpLink = "https://github.com/joukevandermaas/saule/wiki"
                };
            }

            return jsonApi.PreprocessContent(content, resource, request.RequestUri);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = await base.SendAsync(request, cancellationToken);

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
    }
}
