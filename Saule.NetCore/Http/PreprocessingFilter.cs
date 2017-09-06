using System;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Saule.Queries;
using Saule.Serialization;

namespace Saule.Http
{
    using System.Linq;

    /// <summary>
    /// Processes JSON API responses to enable filtering, pagination and sorting.
    /// </summary>
    public class PreprocessingFilter : IActionFilter
    {
        private readonly JsonApiConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreprocessingFilter"/> class.
        /// </summary>
        /// <param name="config">The configuration parameters for JSON API serialization.</param>
        public PreprocessingFilter(JsonApiConfiguration config)
        {
            _config = config;
        }

        /// <inheritdoc />
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        /// <inheritdoc />
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var requestHeaders = context.HttpContext.Request.GetTypedHeaders();
            var hasMediaType = requestHeaders.Accept?.Any(x => x.MediaType == Constants.MediaType) ?? false;

            var statusCode = context.HttpContext.Response.StatusCode;
            if (!hasMediaType || (statusCode >= 400 && statusCode < 500))
            {
                // probably malformed request or not found
                return;
            }

            if (context.Exception != null)
            {
                context.Result = new ObjectResult(new ApiError(context.Exception));
                context.ExceptionHandled = true;
            }

            var value = context.Result as ObjectResult;

            var content = PreprocessRequest(value?.Value, context, _config);

            if (content.ErrorContent != null)
            {
                context.HttpContext.Response.StatusCode = (int)(ApiError.AreClientErrors(content.ErrorContent)
                    ? HttpStatusCode.BadRequest
                    : HttpStatusCode.InternalServerError);
            }

            context.HttpContext.Items.Add(Constants.PropertyNames.PreprocessResult, content);
        }

        internal static PreprocessResult PreprocessRequest(
            object content,
            ActionExecutedContext resultContext,
            JsonApiConfiguration config)
        {
            var jsonApi = new JsonApiSerializer();
            jsonApi.JsonConverters.AddRange(config.JsonConverters);

            PrepareQueryContext(jsonApi, resultContext, config);

            ApiResource resource = null;
            if (resultContext.HttpContext.Items.ContainsKey(Constants.PropertyNames.ResourceDescriptor))
            {
                resource = (ApiResource)resultContext.HttpContext.Items[Constants.PropertyNames.ResourceDescriptor];
            }
            else if (content != null && !(content is SerializableError))
            {
                content = new JsonApiException(
                    ErrorType.Server,
                    "You must add a [ReturnsResourceAttribute] to action methods.")
                {
                    HelpLink = "https://github.com/joukevandermaas/saule/wiki"
                };
            }

            if (!(content is SerializableError) && jsonApi.QueryContext?.Pagination?.PerPage > jsonApi.QueryContext?.Pagination?.PageSizeLimit)
            {
                content = new JsonApiException(ErrorType.Client, "Page size exceeds page size limit for queries.");
            }

            PrepareUrlPathBuilder(jsonApi, resultContext, config);

            var requestUri = new Uri(resultContext.HttpContext.Request.GetEncodedUrl());

            return jsonApi.PreprocessContent(content, resource, requestUri);
        }

        private static void PrepareUrlPathBuilder(
            JsonApiSerializer jsonApiSerializer,
            ActionExecutedContext resultContext,
            JsonApiConfiguration config)
        {
            if (config.UrlPathBuilder != null)
            {
                jsonApiSerializer.UrlPathBuilder = config.UrlPathBuilder;
            }
            else
            {
                var routeTemplate = resultContext.ActionDescriptor.AttributeRouteInfo.Template;
                var virtualPathRoot = resultContext.HttpContext.Request.PathBase.Value ?? "/";

                jsonApiSerializer.UrlPathBuilder = new DefaultUrlPathBuilder(
                    virtualPathRoot, routeTemplate);
            }
        }

        private static void PrepareQueryContext(
            JsonApiSerializer jsonApiSerializer,
            ActionExecutedContext resultContext,
            JsonApiConfiguration config)
        {
            if (!resultContext.HttpContext.Items.ContainsKey(Constants.PropertyNames.QueryContext))
            {
                return;
            }

            var queryContext = (QueryContext)resultContext.HttpContext.Items[Constants.PropertyNames.QueryContext];

            if (queryContext.Filtering != null)
            {
                queryContext.Filtering.QueryFilters = config.QueryFilterExpressions;
            }

            jsonApiSerializer.QueryContext = queryContext;
        }
    }
}
